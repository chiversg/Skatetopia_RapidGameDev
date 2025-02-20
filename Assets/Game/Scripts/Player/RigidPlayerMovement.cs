using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class RigidPlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Rigidbody player;
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
    [SerializeField]
    [Tooltip("Gravity that affects the player's fall speed")]
    private float gravStrength = 9.8f;
    [SerializeField]
    [Tooltip("Gravity gained by going down slopes")]
    private float gravPotentialStrength = 9.8f;

    public bool debug;

    public TextMeshProUGUI debugText;

    private float direction;
    private float vSpeed;
    private float xSpeed;
    private float xJump;
    private float yJump;

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

    Vector2 surfaceNormal = Vector2.up;

    private void Start()
    {
        gravity = new Vector2(0f, -gravStrength);
        downRay = new Ray(player.transform.position, Vector3.down);
        leftRay = new Ray(player.transform.position, Vector3.left);
        rightRay = new Ray(player.transform.position, Vector3.right);
    }
    void Update()
    {
        velocity = new Vector2(0f,0f);
        if (onRail) playerState = state.GRINDING;
        //updateRays();
        if (debug) updateDebugText();

        switch (playerState)
        {
            case state.GROUNDED:

                movePlayer();
                applyGravity();
                applyDrag();
                addMomentum();
                performTricks();
                break;
            case state.JUMPING:
                movePlayer();
                applyGravity();
                break;
            case state.FALLING:
                movePlayer();
                applyGravity();
                break;
            case state.GRINDING:
                performTricks();
                movePlayerTowards();
                //matchRotation();
                break;
        }
        velocity = inputVelocity + gravity;
        player.velocity = velocity;
    }
    private void movePlayer()
    {
        float direction = Input.GetAxis("Horizontal");
        inputVelocity = new Vector2(direction * speed, 0f);
    }
    private void performTricks()
    {

    }
    private void applyGravity()
    {
        velocity += gravity * surfaceNormal; 
    }
    private void applyDrag()
    {

    }
    private Vector2 adjustVelocityToSlope(Vector2 velocity)
    {
        return velocity;
    }
    private void addMomentum()
    {

    }
    private bool isGrounded()
    {
        return false;
    }
    private void updateDebugText()
    {
        debugText.text =
            "\nVelocity: " + player.velocity +
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
    
}