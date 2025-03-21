using TMPro;
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
    [SerializeField] private float airDampening = 0.75f;

    [Header("Gravity")]
    [SerializeField][Tooltip("Strength of gravity while the player is moving downwards")] private float downwardsGravityStrength = 19.6f;
    [SerializeField][Tooltip("Strength of gravity while the playerr is moving upwards")] private float upwardsGravityStrength = 9.8f;
    [SerializeField][Tooltip("Gravity gained by going down slopes")] private float gravPotentialStrength = 9.8f;

    [Header("Tricks")]
    [SerializeField] private float maxOllieAngle = 30;
    [SerializeField] private float maxBackflipAngle = 30;
    [SerializeField] private float maxUturnAngle = 30;
    [SerializeField] private float maxKickoffAngle = 30;
    [SerializeField][Tooltip("The maximum speed the player can be moving to start kickoff")] private float kickoffMaxEntranceSpeed = 3;
    [SerializeField] private float ollieStrength = 4;
    [SerializeField] private float backflipStrength = 8;
    [SerializeField] private float uturnDuration = 0.5f;
    [SerializeField] private float kickoffMaxSpeed = 10;
    [SerializeField] private float kickoffTimeToCharge = 3;
    [SerializeField] private float crouchingGravityMultiplier = 2;

    [Header("Collision")]
    [SerializeField] private float knockbackStrength;
    [SerializeField] private Transform floorCheck;
    [SerializeField] private Transform LeftCheck;
    [SerializeField] private Transform RightCheck;
    [SerializeField] private Transform CeilingCheck;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask floorObjects;

    [Header("Miscellaneous")]
    [SerializeField] private Animator animator;
    public bool debug;
    private bool isGrounded;
    private bool isJumping;

    public TextMeshProUGUI debugText;

    private float direction;
    private float lastFacedDirection;
    private float vSpeed;
    private float xSpeed;
    private Vector3 lastPos;

    private float uturnTargetSpeed;
    private float uturnInitalSpeed;
    private float uturnSpeed;
    private float uturnTime;

    private float kickoffTime;
    private bool lockRotation;
    private bool isCrouching;

    private Vector2 moveVector = new Vector2(0f, 0f);

    public Transform railEnd;
    public bool onRail;
    private bool turning;

    private Transform prevPos;
    private Vector3 prevDeltaPos;
    private enum state
    {
        GROUNDED,
        JUMPING,
        FALLING,
        GRINDING,
        TURNING,
        CHARGING
    };
    private state playerState = state.FALLING;

    Vector2 momentumGain;   //Extra momemtum gained by gravity
    Vector2 velocity;       //The player's total velocity
    Vector2 rotatedVelocity; //The player's velocity rotated to slope

    Ray downRay;
    Ray leftRay;
    Ray rightRay;

    Vector3 localVelocity;

    Vector3 surfaceNormal = Vector3.up;

    private float playerAngle;
    private Quaternion defaultRotation;

    public SpriteRenderer kickoffIndicator;
    public Transform indicatorTransform;


    void Start()
    {
        Application.targetFrameRate = 60;
        defaultRotation = player.rotation;
        prevPos = player.transform;
        downRay = new Ray(player.transform.position, Vector3.down);
        leftRay = new Ray(player.transform.position, Vector3.left);
        rightRay = new Ray(player.transform.position, Vector3.right);
    }
    private void Update()
    {
        //Debug.Log(isGrounded);
        findPlayerAngle();
        //collidedSurfaces.Clear();
        if (onRail) playerState = state.GRINDING;
        //updateRays();

        updateStates();
        updateCurrentSurface();
        if(playerState != state.GRINDING) rotatePlayerToTarget(surfaceNormal);
        flipSprite();
        animate();
            
        if (debug) updateDebugText();

        direction = Input.GetAxisRaw("Horizontal");
        lastFacedDirection = direction != 0 ? direction : lastFacedDirection;
    }
    void FixedUpdate()
    {
        checkCollisions();
        localVelocity = Quaternion.FromToRotation(surfaceNormal, Vector3.up) * player.velocity;

        switch (playerState)
        {
            case state.GROUNDED:

                movePlayer(1);
                applyGravity();
                addMomentum();
                performTricks();
                break;
            case state.JUMPING:
                movePlayer(airDampening);
                applyGravity();
                break;
            case state.FALLING:
                movePlayer(airDampening);
                applyGravity();
                break;
            case state.GRINDING:
                performTricks();
                movePlayerTowards();
                InstantiateDust dust = gameObject.AddComponent<InstantiateDust>();
                dust.makeDust(transform.position);
                break;
            case state.TURNING:
                uturn();
                break;
            case state.CHARGING:
                kickoff();
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
        player.velocity = rotatedVelocity;
    }
    private void movePlayer(float dampening)
    {
        //Determine the input direction
        //If the direction held is the same direction the player is moving, move normal
        //Else if the direction held is opposite to player movement, multiply movement by deceleration as well
        //Else apply drag opposite to the direction the player is moving

        direction = Input.GetAxisRaw("Horizontal");
        if ((direction < 0) == (xSpeed < 0) && direction != 0 && Mathf.Abs(xSpeed) < maxManualSpeed)
        {
            // Debug.Log("Player dir" + direction);
            xSpeed += direction * speed * dampening * acceleration * Time.deltaTime;
            //xSpeed = Mathf.Clamp(xSpeed, -maxManualSpeed, maxManualSpeed);
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
        if (Input.GetButton("Jump") && (isGrounded || onRail) && Mathf.Abs(playerAngle) <= maxOllieAngle && GameManager.ollie)
        {
            onRail = false;
            isJumping = true;
            vSpeed += ollieStrength;
            animator.SetBool("isJumping", true);
        }
        if (Input.GetButton("Backflip") && (isGrounded || onRail) && Mathf.Abs(playerAngle) <= maxBackflipAngle && GameManager.flip)
        {
            onRail = false;
            isJumping = true;
            vSpeed += backflipStrength;
            animator.SetBool("isJumping", true);
        }
        if (Input.GetButtonDown("U-Turn") && (isGrounded || onRail) && Mathf.Abs(playerAngle) <= maxBackflipAngle && GameManager.uturn)
        {
            onRail = false;
            isJumping = false;
            turning = true;
            uturnTargetSpeed = -xSpeed;
            uturnInitalSpeed = xSpeed;
            uturnSpeed = (uturnTargetSpeed - xSpeed) / (uturnDuration);
            uturnTime = 0f;
        }
        if (Input.GetButtonDown("Kickoff") && (isGrounded || onRail) && Mathf.Abs(playerAngle) <= maxKickoffAngle)
        {
            Debug.Log("tired ot kick");
            onRail = false;
            kickoffTime = 0f;
            playerState = state.CHARGING;
        }
        if (Input.GetButton("Crouch"))
        {
            animator.SetBool("isCrouching", true);
            isCrouching = true;
        }
        else
        {
            animator.SetBool("isCrouching", false);
            isCrouching = false;
        }
    }
    private void applyGravity()
    {
        //If the player is in the air apply the strength of gravity to the player
        //Otherwise set isJumping to false, and set vertical speed to zero
        if (isGrounded == false)
        {
            if (vSpeed >= 0f)
            {
                vSpeed -= upwardsGravityStrength * Time.deltaTime;
            }
            else
            {
                vSpeed -= downwardsGravityStrength * Time.deltaTime;
            }
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
        if (isCrouching) momentumGain *= crouchingGravityMultiplier;
        xSpeed += -momentumGain.y * Time.deltaTime;
        // Debug.Log("Horizonal Momentum Gain: " + momenatumGain.y);
    }
    private void uturn()
    {
        uturnTime += Time.deltaTime;

        if (uturnTime < uturnDuration)
        {
            xSpeed = uturnSpeed * uturnTime + uturnInitalSpeed;
        }
        else
        {
            xSpeed = uturnTargetSpeed;
            turning = false;
        }
    }
    private void kickoff()
    {
        if (Input.GetButton("Kickoff"))
        {
            animator.SetBool("isKickoff", true);
            if (Mathf.Abs(xSpeed) < 1)
            {
                xSpeed = 0f;
            }
            else
            {
                xSpeed += -velocity.normalized.x * 1;
            }

            if(kickoffTime >= kickoffTimeToCharge)
            {
                kickoffIndicator.color = Color.green;
                kickoffTime = kickoffTimeToCharge;
            } else
            {
                kickoffIndicator.color = Color.red;
                kickoffTime = kickoffTime + Time.deltaTime;
            }
            
        }
        else
        {
            kickoffIndicator.color = Color.red;
            xSpeed = kickoffMaxSpeed * (kickoffTime / kickoffTimeToCharge) * lastFacedDirection;
            playerState = state.GROUNDED;
            animator.SetBool("isKickoff", false);
        }
    }
    //-----------------------------------------------------------------------[Movement Helper Methods]
    private void rotatePlayerToTarget(Vector3 target)
    {
        player.transform.rotation = Quaternion.FromToRotation(Vector3.up, target);
        updateRays();
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
        if (!lockRotation) {
            //Shoots a raycast directly below the player 5 units down. Raycast is relative to player rotation
            //Raycast is used if the player is in any state other than jumping
            //OR
            //If the ground detection sphere detects ground below the player
            if (playerState != state.JUMPING || Physics.OverlapSphere(floorCheck.position, 0.4f, floorObjects).Length > 0)
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
    }
    private void checkCollisions()
    {
       //isGrounded = Physics.Raycast(downRay.origin, downRay.direction, 1.1f);
        isGrounded = Physics.OverlapSphere(floorCheck.position, 1 * player.transform.lossyScale.y, floorObjects).Length > 0;
        if (isGrounded) animator.SetBool("isJumping", false);
        Vector3 boxSize = new Vector3(0.26f * Mathf.Abs(player.transform.lossyScale.x), 0.40f * Mathf.Abs(player.transform.lossyScale.y), 1f * Mathf.Abs(player.transform.lossyScale.z));
        if (Physics.OverlapBox(LeftCheck.position, boxSize, player.rotation, floorObjects).Length > 0)
        {
            xSpeed = 0f;
        }
        if (Physics.OverlapBox(RightCheck.position, boxSize, player.rotation, floorObjects).Length > 0)
        {
            xSpeed = 0f;
        }
        if(Physics.OverlapSphere(CeilingCheck.position, 1 * player.transform.lossyScale.y, floorObjects).Length > 0)
        {
            vSpeed = 0f;
        }
    }
        private void updateStates()
    {
        if (onRail)
        {
            playerState = state.GRINDING;
        }
        else if (turning)
        {
            playerState = state.TURNING;
        }
        else if (playerState == state.CHARGING)
        {
            playerState = state.CHARGING;
        }
        else if (isGrounded)
        {
            if(playerState != state.GROUNDED)
            {
                //justGrounded();
            }
            playerState = state.GROUNDED;
        }
        else if (isJumping)
        {
            playerState = state.JUMPING;
            if(!lockRotation) surfaceNormal = Vector3.up;
        }
        else
        {
            playerState = state.FALLING;
            if(!lockRotation) surfaceNormal = Vector3.up;
        }
    }
    private void justGrounded()
    {
        var adj = adjustVelocityToTarget(velocity, surfaceNormal);
        Debug.Log(adj);
        xSpeed = adj.x;
        vSpeed = adj.y;
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
        playerAngle = Mathf.RoundToInt(player.transform.rotation.eulerAngles.z);
        if (playerAngle > 180)
        {
            playerAngle -= 360;
        }
    }
    private void movePlayerTowards()
    {
        //rotatePlayerToTarget(surfaceNormal);
        //Debug.Log("Pre Speed: " + xSpeed);
        if (xSpeed * lastFacedDirection < (maxManualSpeed * 4) / 5)
        {
            //Debug.Log("Equation: " + (maxManualSpeed / 500) * lastFacedDirection);
            xSpeed += (maxManualSpeed / 200) * lastFacedDirection;
        }
        Debug.Log("Speed: " + xSpeed);
        //player.AddRelativeForce(Vector3.forward * xSpeed);

        transform.position = Vector3.MoveTowards(transform.position, railEnd.position, Mathf.Abs(xSpeed) / 100);

        //Using xSpeed to ensure that the it doesn't matter which way the player/rail is facing, could probably change to Math.absolute
        //Debug.Log(transform.position.x * xSpeed + " " + railEnd.position.x * xSpeed);
        if (transform.position.x * xSpeed > railEnd.position.x * xSpeed)
        {
            playerState = state.FALLING;
            onRail = false;
        }
    }
    private void flipSprite()
    {
        if (xSpeed != 0)
        {
            if (xSpeed < 0)
            {
                player.transform.localScale = new Vector3(-Mathf.Abs(player.transform.lossyScale.x), player.transform.lossyScale.y, player.transform.lossyScale.z);
            }
            else if (xSpeed > 0)
            {
                player.transform.localScale = new Vector3(Mathf.Abs(player.transform.lossyScale.x), player.transform.lossyScale.y, player.transform.lossyScale.z);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "LockRotate")
        {
            Debug.Log("locked");
            lockRotation = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "LockRotate")
        {
            Debug.Log("unlocked");
            xSpeed = player.velocity.x;
            vSpeed = player.velocity.y;
            lockRotation = false;
        }
    }
    private void animate()
    {
        //Debug.Log(isCrouching);
        animator.SetFloat("Speed", Mathf.Abs(xSpeed));
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isCrouching", isCrouching);
        if (isJumping && vSpeed <= 0) {
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }
        
    }
    //-----------------------------------------------------------------------[Public Methods]
    public void boardRail(Transform target, Transform rail)
    {
        Debug.Log(player.transform.rotation.z);
        vSpeed = 0;
        railEnd = target;
        onRail = true;
    }
    public void addSpeed(float xs, float ys, bool reset)
    {
        if (reset){
            vSpeed = 0;
        }
        //Debug.Log("BOunce");
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
        Vector2 vel = new Vector2(xSpeed, vSpeed);
        vel = vel.normalized * -knockbackStrength;
        xSpeed = vel.x;
        vSpeed = vel.y;
        animator.SetTrigger("playerHit");
    }
    //-----------------------------------------------------------------------[Debug Methods]
    private void updateDebugText()
    {
        debugText.text =
            "\nVelocity: " + player.velocity +
            "\nLocal Velocity: " + localVelocity +
            "\nxSpeed: " + xSpeed +
            "\nvSpeed: " + vSpeed +
            "\nState: " + playerState +
            "\nRotation: " + playerAngle +
            "\nLast: " + lastFacedDirection +
            "\nclip: " + animator.GetCurrentAnimatorClipInfo(0)[0].clip.name; ;
    }

    public float getSpeed(){
        return xSpeed;
    }

    public float getMaxManualSpeed(){
        return maxManualSpeed;
    }

    public bool getGrounded(){
        return isGrounded;
    }
}

