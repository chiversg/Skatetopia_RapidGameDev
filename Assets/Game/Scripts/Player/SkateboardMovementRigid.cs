using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
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
    [SerializeField] private Transform floorProxCheck;
    [SerializeField] private Transform LeftCheck;
    [SerializeField] private Transform groundedLeftCheck;
    [SerializeField] private Transform RightCheck;
    [SerializeField] private Transform groundedRightCheck;
    [SerializeField] private Transform CeilingCheck;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask floorObjects;
    private bool isCloseToGround;

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpAudio;
    [SerializeField] private AudioClip landAudio;
    [SerializeField] private AudioClip grindRailAudio;
    [SerializeField] private AudioClip skateboardRollAudio;
    [SerializeField] private AudioClip uTurnAudio;
    [SerializeField] private AudioClip playerKnockedBack;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float rollVolume;

    [Header("Miscellaneous")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer character;
    public bool debug;
    private bool isGrounded;
    private bool isJumping;
    private bool uTurn;

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
    private float kickFlipMaxTime = 0.5f;

    private float kickoffTime;
    private float kickFlipTime;
    private bool lockRotation;
    private bool isCrouching;
    private bool canWallride = true;

    private float grindSpeed;
    private float airTime = 0;

    private Vector2 moveVector = new Vector2(0f, 0f);
    public Transform railEnd;
    public bool onRail;
    private bool turning;
    private bool canControl = true;
    [HideInInspector] public Vector3 spriteRotation;

    private Transform prevPos;
    private Vector3 previousPos;
    private Vector3 prevDeltaPos;
    private enum state
    {
        GROUNDED,
        JUMPING,
        FALLING,
        GRINDING,
        TURNING,
        CHARGING,
        LISTENING
    };
    private state playerState = state.FALLING;
    private state prevPlayerState = state.FALLING;

    private float listenXPos = 0f;

    Vector2 momentumGain;   //Extra momemtum gained by gravity
    Vector2 velocity;       //The player's total velocity
    Vector2 rotatedVelocity; //The player's velocity rotated to slope

    Ray downRay;
    Ray leftRay;
    Ray rightRay;

    Vector3 localVelocity;

    Vector3 surfaceNormal = Vector3.up;
    private bool isCharging;

    private float playerAngle;
    private Quaternion defaultRotation;

    public SpriteRenderer kickoffIndicator;
    public Transform indicatorTransform;
    public PlayerHit PlayerHit;



    void Start()
    {
        Application.targetFrameRate = 60;
        defaultRotation = player.rotation;
        prevPos = player.transform;
        previousPos = player.position;
        downRay = new Ray(player.transform.position, Vector3.down);
        leftRay = new Ray(player.transform.position, Vector3.left);
        rightRay = new Ray(player.transform.position, Vector3.right);

        StartCoroutine(playMovementAudio());
    }
    private void Update()
    {
        //Debug.Log(isGrounded);
        findPlayerAngle();
        //collidedSurfaces.Clear();
        if (onRail) playerState = state.GRINDING;
        //updateRays();

        if (playerState != state.LISTENING)
        {
            updateStates();
            updateCurrentSurface();
        }
        if (playerState != state.GRINDING) rotatePlayerToTarget(surfaceNormal);
        flipSprite();
        animate();

        if (debug) updateDebugText();
        direction = Input.GetAxisRaw("Horizontal");
        lastFacedDirection = direction != 0 ? direction : lastFacedDirection;

        if (playerState == state.GROUNDED && playerState != prevPlayerState && airTime > 0.2f)
        {
            audioSource.PlayOneShot(landAudio);
        }
        prevPlayerState = playerState;

        if (Input.GetButtonDown("U-Turn") && (isGrounded) && GameManager.uturn) uTurn = true;

        //performTricks();
    }
    void FixedUpdate()
    {
        checkCollisions();
        checkForWallCollision();
        if (playerState != state.LISTENING) localVelocity = Quaternion.FromToRotation(surfaceNormal, Vector3.up) * player.velocity;
        else xSpeed = 0;

        switch (playerState)
        {
            case state.GROUNDED:
                airTime = 0;
                movePlayer(1);
                applyGravity();
                addMomentum();
                performTricks();
                break;
            case state.JUMPING:
                airTime += Time.deltaTime;
                movePlayer(airDampening);
                applyGravity();
                break;
            case state.FALLING:
                airTime += Time.deltaTime;
                movePlayer(airDampening);
                applyGravity();
                break;
            case state.GRINDING:
                movePlayerTowards();
                performTricks();
                InstantiateDust dust = gameObject.AddComponent<InstantiateDust>();
                dust.makeDust(transform.position);
                break;
            case state.TURNING:
                uturn();
                break;
            case state.CHARGING:
                //kickoff();
                chargeJump();
                movePlayer(1);
                break;
            case state.LISTENING:
                setListenPos();
                break;
        }
        if (playerState != state.LISTENING)
        {
            move_and_slide();
        }
    }

    //-----------------------------------------------------------------------[Movement Methods]
    private void move_and_slide()
    {
        velocity = new Vector2(xSpeed, vSpeed);
        //if (playerState == state.GRINDING) velocity = new Vector2(grindSpeed, vSpeed);
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
        //Debug.Log("MOVE PLAYER IS BEING CALLED");
        //Debug.Log(player.transform.rotation.z);
        direction = Input.GetAxisRaw("Horizontal");
        if ((direction < 0) == (xSpeed < 0) && direction != 0 && Mathf.Abs(xSpeed) < maxManualSpeed && canControl)
        {
            if (Mathf.Abs(player.rotation.eulerAngles.z) != 90)
            {
                // Debug.Log("Player dir" + direction);
                xSpeed += direction * speed * dampening * acceleration * Time.deltaTime;
                //xSpeed = Mathf.Clamp(xSpeed, -maxManualSpeed, maxManualSpeed);
            }
        }
        else if ((direction < 0) != (xSpeed < 0) && direction != 0 && canControl)
        {
            if (Mathf.Abs(player.rotation.eulerAngles.z) != 90)
            {
                direction *= deceleration;
                // Debug.Log("Player dir" + direction);
                xSpeed += direction * speed * dampening * acceleration * Time.deltaTime;
                xSpeed = Mathf.Clamp(xSpeed, -maxManualSpeed, maxManualSpeed);
            }
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
            if (onRail)
            {
                onRail = false;
                xSpeed = grindSpeed;
            }
            isJumping = true;
            vSpeed += ollieStrength;
            animator.SetBool("isJumping", true);
            audioSource.PlayOneShot(jumpAudio);
            playerState = state.JUMPING;
        }
        if (Input.GetButton("Backflip") && (isGrounded || onRail) && Mathf.Abs(playerAngle) <= maxBackflipAngle && GameManager.flip)
        {
            if (onRail)
            {
                onRail = false;
                xSpeed = grindSpeed;
            }
            isJumping = true;
            vSpeed += backflipStrength;
            animator.SetBool("isJumping", true);
            animator.SetBool("isKickFlip", true);
            audioSource.PlayOneShot(jumpAudio);
            playerState = state.JUMPING;
            //playerState = state.CHARGING;
            //kickFlipTime = 0f;
        }
        if (uTurn && (isGrounded) && Mathf.Abs(playerAngle) <= maxBackflipAngle && GameManager.uturn)
        {
            uTurn = false;
            if (onRail)
            {
                onRail = false;
                xSpeed = grindSpeed;
            }
            isJumping = false;
            turning = true;
            uturnTargetSpeed = -xSpeed;
            uturnInitalSpeed = xSpeed;
            uturnSpeed = (uturnTargetSpeed - xSpeed) / (uturnDuration);
            uturnTime = 0f;
            animator.SetBool("playerSwitch", true);
            audioSource.PlayOneShot(uTurnAudio);
        }
        else
        {
            uTurn = false;
        }
        if (Input.GetButtonDown("Kickoff") && (isGrounded || onRail) && Mathf.Abs(playerAngle) <= maxKickoffAngle)
        {
            Debug.Log("tired ot kick");
            onRail = false;
            kickoffTime = 0f;
            playerState = state.CHARGING;
        }
        if (Input.GetButton("Crouch") || Input.GetAxis("ControllerY") >= 0.3)
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
            animator.SetBool("playerSwitch", false);
        }
    }
    private void chargeJump()
    {
        if (Input.GetButton("Backflip"))
        {
            kickFlipTime += Time.deltaTime;
            if (kickFlipTime > kickFlipMaxTime)
            {
                kickFlipTime = kickFlipMaxTime;
            }
        }
        else
        {
            playerState = state.JUMPING;
            var launchForce = backflipStrength * (kickFlipTime / kickFlipMaxTime);
            if (onRail)
            {
                onRail = false;
                xSpeed = grindSpeed;
            }
            if (launchForce < ollieStrength)
            {
                vSpeed = ollieStrength;
            }
            else
            {
                vSpeed = launchForce;
            }
            isJumping = true;
            isGrounded = false;
            animator.SetBool("isKickFlip", true);
            animator.SetBool("isJumping", true);

            isCharging = false;
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

            if (kickoffTime >= kickoffTimeToCharge)
            {
                kickoffIndicator.color = Color.green;
                kickoffTime = kickoffTimeToCharge;
            }
            else
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
        if (!lockRotation && canControl)
        {
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
            if (Physics.Raycast(downRay, out RaycastHit hitInfo2, 2f))
            {
                if (hitInfo2.collider.gameObject.tag == "Floor")
                {
                    spriteRotation = hitInfo2.normal;
                }
            }
            else
            {
                spriteRotation = Vector3.up;
            }
            player.velocity = Quaternion.FromToRotation(Vector3.up, surfaceNormal) * player.velocity;
        }
    }
    private void checkForWallCollision()
    {
        if (canControl)
        {
            //Debug.Log(isCloseToGround);
            Vector3 boxHalfExtends = new Vector3(0.05f, 1f, 1f);
            Transform left = LeftCheck;
            Transform right = RightCheck;
            if (isCloseToGround)
            {
                left = groundedLeftCheck;
                right = groundedRightCheck;
                boxHalfExtends = new Vector3(0.05f, 0.5f, 1f);
            }
            else
            {
                left = groundedLeftCheck;
                right = groundedRightCheck;
                boxHalfExtends = new Vector3(0.05f, 0.5f, 1f);
            }

            if (Physics.OverlapBox(left.position, boxHalfExtends, left.transform.rotation, floorObjects).Length > 0)
            {
                Debug.Log("Collided will wall on left side");
                audioSource.PlayOneShot(playerKnockedBack);
                animator.SetTrigger("playerHit");
                player.transform.position = previousPos +  new Vector3(0.1f, 0f, 0f);
                surfaceNormal = Vector3.up;
                xSpeed = 10 * Mathf.Abs(xSpeed / maxSpeed);
                StartCoroutine(removePlayerControl(0.5f));
            }
            if (Physics.OverlapBox(right.position, boxHalfExtends, right.transform.rotation, floorObjects).Length > 0)
            {
                Debug.Log("Collided with wall on right side");
                audioSource.PlayOneShot(playerKnockedBack);
                animator.SetTrigger("playerHit");
                player.transform.position = previousPos - new Vector3(0.1f, 0f, 0f);
                surfaceNormal = Vector3.up;
                xSpeed = -10 * Mathf.Abs(xSpeed / maxSpeed);
                StartCoroutine(removePlayerControl(0.5f));
            }

            if (Physics.OverlapBox(CeilingCheck.position, new Vector3(0.50f, 0.5f, 1f), CeilingCheck.rotation, floorObjects).Length > 0)
            {
                Debug.Log("Collided with ceiling");
                audioSource.PlayOneShot(playerKnockedBack);
                player.transform.position = previousPos - new Vector3(0f, 0.1f, 0f);
                vSpeed = -2f;
                StartCoroutine(removePlayerControl(0.1f));
            }

        }
        previousPos = player.position;
    }
    IEnumerator removePlayerControl(float time)
    {
        canControl = false;
        yield return new WaitForSeconds(time);
        canControl = true;
    }
    private void checkCollisions()
    {
        //isGrounded = Physics.Raycast(downRay.origin, downRay.direction, 1.1f);
        isGrounded = Physics.OverlapSphere(floorCheck.position, 1f, floorObjects).Length > 0;
        isCloseToGround = Physics.OverlapBox(floorProxCheck.position, new Vector3(1f, 0.5f, 1f), player.rotation, floorObjects).Length > 0;
        if (isGrounded)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isKickFlip", false);
        }
        Vector3 boxSize = new Vector3(0.26f * Mathf.Abs(player.transform.lossyScale.x), 0.40f * Mathf.Abs(player.transform.lossyScale.y), 1f * Mathf.Abs(player.transform.lossyScale.z));
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
            if (playerState != state.GROUNDED)
            {
                //justGrounded();
            }
            playerState = state.GROUNDED;
        }
        else if (isJumping)
        {
            playerState = state.JUMPING;
            if (!lockRotation) surfaceNormal = Vector3.up;
        }
        else if (playerState == state.LISTENING)
        {
            playerState = state.LISTENING;
        }
        else
        {
            playerState = state.FALLING;
            if (!lockRotation) surfaceNormal = Vector3.up;
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
        if (transform.position.x < railEnd.position.x) direction = 1;
        else if (transform.position.x > railEnd.position.x) direction = -1;
        if (grindSpeed * direction < (maxManualSpeed * 4) / 5)
        {
            grindSpeed += (maxManualSpeed / 200) * direction;
        }
        Debug.Log("Speed: " + grindSpeed);

        transform.position = Vector3.MoveTowards(transform.position, railEnd.position, Mathf.Abs(grindSpeed) / 50);

        if (transform.position.x * grindSpeed >= railEnd.position.x * grindSpeed)
        {
            xSpeed = grindSpeed;
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
                character.flipX = false;
            }
            else if (xSpeed > 0)
            {
                character.flipX = true;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "LockRotate")
        {
            Debug.Log("locked");
            lockRotation = true;
        }
        if (other.gameObject.tag == "DeactivateWallride")
        {
            canWallride = false;
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
        if (other.gameObject.tag == "DeactivateWallride")
        {
            canWallride = true;
        }
    }
    private void animate()
    {
        Debug.Log(isGrounded + " is grounded");
        Debug.Log(isCloseToGround + " is close");
        Debug.Log(isJumping + " is jumping");
        animator.SetFloat("Speed", Mathf.Abs(xSpeed));
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isCrouching", isCrouching);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isCloseToGround", isCloseToGround);
        animator.SetBool("onRail", onRail);
        animator.SetBool("Invincible", PlayerHit.isInvincible());
        if (isJumping && vSpeed <= 0)
        {
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }
    }

    private void setListenPos()
    {
        transform.position = new Vector3(listenXPos, transform.position.y, transform.position.z);
    }
    //-----------------------------------------------------------------------[Public Methods]
    public void BoardRail(Transform target)
    {
        playerState = state.GRINDING;
        surfaceNormal = Vector3.up;
        spriteRotation = Vector3.up;
        rotatePlayerToTarget(surfaceNormal);
        Debug.Log("RWARWARR");
        Debug.Log("XSpeed:" + xSpeed);
        grindSpeed = xSpeed;
        xSpeed = 0;
        vSpeed = 0;
        railEnd = target;
        onRail = true;
        StartCoroutine(playRailAudio());
    }
    IEnumerator playRailAudio()
    {
        while (onRail)
        {
            surfaceNormal = Vector3.up;
            Debug.Log("playing rail stuff");
            audioSource.PlayOneShot(grindRailAudio);
            yield return new WaitForSeconds(0.5f);
        }
        audioSource.Stop();
    }
    IEnumerator playMovementAudio()
    {
        while (true)
        {
            if (playerState == state.GROUNDED && xSpeed != 0) audioSource.PlayOneShot(skateboardRollAudio, rollVolume);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void addSpeed(float xs, float ys, bool reset, bool isBouncePad)
    {
        if (reset)
        {
            vSpeed = 0;
        }
        //Debug.Log("BOunce");
        xSpeed += xs;
        vSpeed += ys;
        if (isBouncePad)
        {
            animator.SetTrigger("bounce");
            isJumping = true;
        }
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
    public void enterDialogue()
    {
        if (listenXPos == 0)
        {
            listenXPos = transform.position.x;
            xSpeed = -1;
        }
        else xSpeed = 0;
        direction = -1;

        playerState = state.LISTENING;
    }
    public void exitDialogue()
    {
        playerState = state.GROUNDED;
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

    public float getSpeed()
    {
        if (playerState == state.GRINDING) return grindSpeed;
        else return xSpeed;
    }

    public float getMaxManualSpeed()
    {
        return maxManualSpeed;
    }

    public bool getGrounded()
    {
        //bool temp = false;
        //if(isGrounded) temp = true;
        //if(isCloseToGround) temp = true;
        return isGrounded;
    }
    public bool CloseToGround()
    {
        return isCloseToGround;
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public Quaternion getRotation()
    {
        return transform.rotation;
    }

    public string getState()
    {
        return playerState.ToString();
    }
}

