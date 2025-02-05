using TMPro;
using UnityEngine;

public class SkateboardMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController player;
    [SerializeField]
    private float acceleration = 1;
    [SerializeField]
    private float maxManualSpeed;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float speed = 1;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private float drag;
    [SerializeField]
    private float deceleration = 1;
    public TextMeshProUGUI debugText;

    private float direction;
    private float vSpeed;
    private float xSpeed;
    private float xJump;
    private float yJump;
    private float gravStrength = 9.8f;
    private Vector2 moveVector = new Vector2(0f, 0f);

    public Transform railEnd;
    public bool onRail;

    private enum state
    {
        GROUNDED,
        JUMPING,
        FALLING,
        GRINDING
    };
    private state playerState = state.FALLING;

    Vector2 inputVelocity;  //Speed gained by holding down an input
    Vector2 dragVelocity;   //The resistance from the ground slowing the player down
    Vector2 momentumGain;   //Extra momemtum gained by gravity
    Vector2 velocity;       //The player's total velocity
    Vector2 rotatedVelocity; //The player's velocity rotated to slope
    Vector2 gravity;

    Ray downRay;
    Ray leftRay;
    Ray rightRay;

    void Start()
    {
        downRay = new Ray(player.transform.position, Vector3.down);
        leftRay = new Ray(player.transform.position, Vector3.left);
        rightRay = new Ray(player.transform.position, Vector3.right);
    }
    void Update()
    {
        if (onRail) playerState = state.GRINDING;
        updateRays();
        updateDebugText();

        switch (playerState)
        {
            case state.GROUNDED:

                movePlayer(1);
                applyGravity();
                applyDrag();
                addMomentum();
                performTricks();
                matchRotation();
                break;
            case state.JUMPING:
                movePlayer(0.5f);
                applyGravity();
                matchRotation();
                break;
            case state.FALLING:
                movePlayer(0.5f);
                applyGravity();
                matchRotation();
                break;
            case state.GRINDING:
                performTricks();
                movePlayerTowards();
                //matchRotation();
                break;
        }
        velocity.x = xSpeed;
        velocity.y = vSpeed;
        velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        rotatedVelocity = adjustVelocityToSlope(velocity);
        Debug.Log(player.transform.rotation.x);
        player.Move(rotatedVelocity * Time.deltaTime);

    }
    private void movePlayer(float dampening)
    {
        float direction = Input.GetAxisRaw("Horizontal");
        if ((direction < 0) != (xSpeed < 0))
        {
            direction *= deceleration;
        }
        Debug.Log("Player dir" + direction);
        xSpeed += direction * speed * dampening * acceleration * Time.deltaTime;
        xSpeed = Mathf.Clamp(xSpeed, -maxManualSpeed, maxManualSpeed);
    }
    private void performTricks()
    {
        if (Input.GetButtonDown("Jump") && (playerState == state.GROUNDED || playerState == state.GRINDING))
        {
            onRail = false;
            playerState = state.JUMPING;
            vSpeed += jumpSpeed;
        }
    }
    private void applyGravity()
    {
        //Debug.Log(player.isGrounded);
        if (player.isGrounded == false)
        {
            vSpeed -= gravStrength * Time.deltaTime;
        }
        else
        {
            playerState = state.GROUNDED;
            vSpeed = 0f;
            yJump = 0f;
        }
        gravity = new Vector2(0f, vSpeed);
    }
    private void applyDrag()
    {
        xSpeed += -velocity.normalized.x * drag * Time.deltaTime;

    }
    private void matchRotation()
    {
        if (Physics.Raycast(downRay, out RaycastHit hitInfo, 2f) && hitInfo.collider.gameObject.tag == "Floor" && playerState == state.GROUNDED)
        {
            player.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            //Debug.Log(hitInfo.collider.gameObject.name);
        }
        else if (Physics.Raycast(leftRay, out RaycastHit lhitInfo, 2f) && lhitInfo.collider.gameObject.tag == "Floor")
        {
            player.transform.rotation = Quaternion.FromToRotation(Vector3.up, lhitInfo.normal);
        }
        else if (Physics.Raycast(rightRay, out RaycastHit rhitInfo, 2f) && rhitInfo.collider.gameObject.tag == "Floor")
        {
            player.transform.rotation = Quaternion.FromToRotation(Vector3.up, rhitInfo.normal);
        }
        else
        {
            player.transform.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.up);
        }
    }
    private Vector2 adjustVelocityToSlope(Vector2 velocity)
    {
        var ray = new Ray(player.transform.position, player.transform.rotation * Vector2.down);

        if (Physics.Raycast(downRay, out RaycastHit hitInfo, 2f) && hitInfo.collider.gameObject.tag == "Floor" && playerState == state.GROUNDED)
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * velocity;
            return adjustedVelocity;
        }
        if (playerState != state.JUMPING) playerState = state.FALLING;
        return velocity;

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (player != null)
        {
            //player.rotation = collision.transform.rotation;
        }
    }
    private void addMomentum()
    {
        Debug.Log("Player Rotation: " + player.transform.rotation.z);
        momentumGain = findComponents(player.transform.rotation.eulerAngles.z, 9.8f);
        xSpeed += -momentumGain.y * Time.deltaTime;
        Debug.Log("Horizonal Momentum Gain: " + momentumGain.y);
    }
    private Vector2 findComponents(float angle, float magnitude)
    {
        float rad = angle * Mathf.Deg2Rad;
        float opposite = magnitude * Mathf.Sin(rad);
        float adjacent = magnitude * Mathf.Cos(rad);

        return new Vector2(adjacent, opposite);
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

    private void updateDebugText()
    {
        debugText.text =
            "\nVelocity: " + rotatedVelocity +
            "\nxSpeed: " + xSpeed +
            "\nvSpeed: " + vSpeed +
            "\nState: " + playerState;
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
    public void boardRail(Transform target)
    {
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
}

