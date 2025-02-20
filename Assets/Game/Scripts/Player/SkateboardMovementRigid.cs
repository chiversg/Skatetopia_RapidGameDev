using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkateboardMovementRigid : MonoBehaviour
{
    [SerializeField] private Rigidbody player;

    [Header("Player Movement")]
    [SerializeField] private float acceleration = 1;
    [SerializeField] private float maxManualSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speed = 1;
    [SerializeField] private float drag;
    [SerializeField] private float deceleration = 1;

    [Header("Gravity")]
    [SerializeField][Tooltip("Gravity that affects the player's fall speed")] private float gravStrength = 9.8f;
    [SerializeField][Tooltip("Gravity gained by going down slopes")] private float gravPotentialStrength = 9.8f;

    [Header("Tricks")]
    [SerializeField] private float maxOllieAngle = 30;
    [SerializeField] private float maxBackflipAngle = 30;
    [SerializeField] private float maxUturnAngle = 30;
    [SerializeField] private float ollieStrength = 4;
    [SerializeField] private float backflipStrength = 8;

    [Header("Collision")]
    [SerializeField] private Transform floorCheck;
    [SerializeField] private Transform LeftCheck;
    [SerializeField] private Transform RightCheck;
    [SerializeField] private Transform CeilingCheck;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask floorObjects;

    [Header("Miscellaneous")]
    public bool debug;
    private bool isGrounded;
    private bool isJumping;

    public TextMeshProUGUI debugText;

    private float direction;
    private float vSpeed;
    private float xSpeed;

    private Vector2 moveVector = new Vector2(0f, 0f);

    public Transform railEnd;
    public bool onRail;

    private Transform prevPos;
    private enum state
    {
        GROUNDED,
        JUMPING,
        FALLING,
        GRINDING
    };
    private state playerState = state.FALLING;

    Vector2 momentumGain;   //Extra momemtum gained by gravity
    Vector2 velocity;       //The player's total velocity
    Vector2 rotatedVelocity; //The player's velocity rotated to slope

    Ray downRay;
    Ray leftRay;
    Ray rightRay;



    Vector3 surfaceNormal = Vector3.up;

    private float playerAngle;


    void Start()
    {
        Application.targetFrameRate = 60;
        prevPos = player.transform;
        downRay = new Ray(player.transform.position, Vector3.down);
        leftRay = new Ray(player.transform.position, Vector3.left);
        rightRay = new Ray(player.transform.position, Vector3.right);
    }
    private void Update()
    {
        findPlayerAngle();
        //collidedSurfaces.Clear();
        if (onRail) playerState = state.GRINDING;
        updateRays();

        updateStates();
        updateCurrentSurface();
        rotatePlayerToTarget(surfaceNormal);

        if (debug) updateDebugText();
    }
    void FixedUpdate()
    {
        checkCollisions();

        switch (playerState)
        {
            case state.GROUNDED:

                movePlayer(1);
                applyGravity();
                addMomentum();
                performTricks();
                break;
            case state.JUMPING:
                movePlayer(0.5f);
                applyGravity();
                break;
            case state.FALLING:
                movePlayer(0.5f);
                applyGravity();
                break;
            case state.GRINDING:
                performTricks();
                movePlayerTowards();
                break;
        }
        move_and_slide();
    }

    //-----------------------------------------------------------------------[Movement Methods]
    private void move_and_slide()
    {
        velocity = new Vector2(xSpeed, vSpeed);
        velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        rotatedVelocity = adjustVelocityToTarget(velocity, surfaceNormal);
        // Debug.Log(player.transform.rotation.x);
        player.velocity = rotatedVelocity;

        if (player.velocity.x == 0 && Mathf.Abs(xSpeed) > 1f) xSpeed = 0f;
    }
    private void movePlayer(float dampening)
    {
        //Determine the input direction
        //If the direction held is the same direction the player is moving, move normal
        //Else if the direction held is opposite to player movement, multiply movement by deceleration as well
        //Else apply drag opposite to the direction the player is moving

        direction = Input.GetAxisRaw("Horizontal");
        if ((direction < 0) == (xSpeed < 0) && direction != 0)
        {
            // Debug.Log("Player dir" + direction);
            xSpeed += direction * speed * dampening * acceleration * Time.deltaTime;
            xSpeed = Mathf.Clamp(xSpeed, -maxManualSpeed, maxManualSpeed);
        }
        else if ((direction < 0) != (xSpeed < 0) && direction != 0)
        {
            direction *= deceleration;
            // Debug.Log("Player dir" + direction);
            xSpeed += direction * speed * dampening * acceleration * Time.deltaTime;
            xSpeed = Mathf.Clamp(xSpeed, -maxManualSpeed, maxManualSpeed);
        }
        else if (playerState == state.GROUNDED)
        {
            if (Mathf.Abs(xSpeed) < drag)
            {
                xSpeed = 0f;
            }
            else
            {
                xSpeed += -velocity.normalized.x * drag;
            }

        }
    }
    private void performTricks()
    {
        if (Input.GetButton("Jump") && isGrounded && Mathf.Abs(playerAngle) < maxOllieAngle)
        {
            onRail = false;
            isJumping = true;
            vSpeed += ollieStrength;
        }
        if (Input.GetButton("Crouch") && isGrounded && Mathf.Abs(playerAngle) < maxBackflipAngle)
        {
            onRail = false;
            isJumping = true;
            vSpeed += backflipStrength;
        }
    }
    private void applyGravity()
    {
        //If the player is in the air apply the strength of gravity to the player
        //Otherwise set isJumping to false, and set vertical speeds to zero
        if (isGrounded == false)
        {
            vSpeed -= gravStrength * Time.deltaTime;
        }
        else
        {
            isJumping = false;
            vSpeed = 0f;
        }
    }
    private void addMomentum()
    {
        //Debug.Log("Player Rotation: " + player.transform.rotation.z);
        momentumGain = findComponents(playerAngle, gravPotentialStrength);
        xSpeed += -momentumGain.y * Time.deltaTime;
        // Debug.Log("Horizonal Momentum Gain: " + momentumGain.y);
    }
    //-----------------------------------------------------------------------[Movement Helper Methods]
    private void rotatePlayerToTarget(Vector3 target)
    {
        player.transform.rotation = Quaternion.FromToRotation(Vector3.up, target);
    }
    private Vector2 adjustVelocityToTarget(Vector2 velocity, Vector3 target)
    {
        //Takes target normal vector and converts it into a quaternion
        //Multiply the calculated quaternion by input vector to rotate vector to match target
        //Return calculated vector
        var slopeRotation = Quaternion.FromToRotation(Vector3.up, target);
        var adjustedVelocity = slopeRotation * velocity;
        return adjustedVelocity;
    }

    private Vector2 findComponents(float angle, float magnitude)
    {
        float rad = angle * Mathf.Deg2Rad;
        float opposite = magnitude * Mathf.Sin(rad);
        float adjacent = magnitude * Mathf.Cos(rad);

        return new Vector2(adjacent, opposite);
    }
    private void updateCurrentSurface()
    {
        //Shoots a raycast directly below the player 5 units down. Raycast is relative to player rotation\
        //Raycast is used if the player is in any state other than juming
        //OR
        //If the ground detection sphere detects ground below the player
        if (playerState != state.JUMPING || Physics.OverlapSphere(floorCheck.position, 0.2f, floorObjects).Length > 0)
        {
            if (Physics.Raycast(downRay, out RaycastHit hitInfo, 5f))
            {
                if (hitInfo.collider.gameObject.tag == "Floor")
                {
                    surfaceNormal = hitInfo.normal;
                }
            }
        }
        player.velocity = Quaternion.FromToRotation(Vector3.up, surfaceNormal) * player.velocity;
    }
    private void checkCollisions()
    {
        isGrounded = Physics.OverlapSphere(floorCheck.position, checkRadius, floorObjects).Length > 0;
        if(Physics.OverlapSphere(CeilingCheck.position, checkRadius, floorObjects).Length > 0)
        {
            vSpeed = 0;
        }
        if(Physics.OverlapSphere(LeftCheck.position, checkRadius, floorObjects).Length > 0)
        {
            xSpeed = 0;
        }
        if (Physics.OverlapSphere(RightCheck.position, checkRadius, floorObjects).Length > 0)
        {
            xSpeed = 0;
        }
    }
    private void updateStates()
    {
        if (onRail)
        {
            playerState = state.GRINDING;
        }
        if (isGrounded)
        {
            playerState = state.GROUNDED;
        }
        else if (isJumping)
        {
            playerState = state.JUMPING;
            surfaceNormal = Vector3.up;
        }
        else
        {
            playerState = state.FALLING;
            surfaceNormal = Vector3.up;
        }
    }
    private void updateRays()
    {
        downRay.origin = player.transform.position;
        downRay.direction = player.transform.rotation * Vector2.down;

        leftRay.origin = player.transform.position;
        leftRay.direction = player.transform.rotation * Vector2.left;

        rightRay.origin = player.transform.position;
        rightRay.direction = player.transform.rotation * Vector2.right;

        Debug.DrawRay(downRay.origin, downRay.direction, Color.yellow);
        Debug.DrawRay(leftRay.origin, leftRay.direction, Color.blue);
        Debug.DrawRay(rightRay.origin, rightRay.direction, Color.red);
    }
    private void findPlayerAngle()
    {
        playerAngle = player.transform.rotation.eulerAngles.z;
        if (playerAngle > 180)
        {
            playerAngle -= 360;
        }
    }
    private void movePlayerTowards()
    {
        transform.position = Vector3.MoveTowards(transform.position, railEnd.position, Mathf.Abs(xSpeed) / 100);
        if (Vector3.Distance(transform.position, railEnd.position) < 0.01f)
        {
            playerState = state.FALLING;
            onRail = false;
        }
    }
    //-----------------------------------------------------------------------[Public Methods]
    public void boardRail(Transform target)
    {
        vSpeed = 0;
        railEnd = target;
        onRail = true;
    }
    public void addSpeed(float xs, float ys)
    {
        Debug.Log("BOunce");
        xSpeed += xs;
        vSpeed += ys;
        //velocity.Set(velocity.x += xs, velocity.y += ys);   
    }
    public void setDirection(float dir)
    {
        direction = dir;
    }
    public float getXSpeed()
    {
        return xSpeed;
    }
    public void hitHazard()
    {
        xSpeed = 0;
        vSpeed = 0;
    }
    //-----------------------------------------------------------------------[Debug Methods]
    private void updateDebugText()
    {
        debugText.text =
            "\nVelocity: " + player.velocity +
            "\nxSpeed: " + xSpeed +
            "\nvSpeed: " + vSpeed +
            "\nState: " + playerState +
            "\nRotation: " + playerAngle;
    }
}

