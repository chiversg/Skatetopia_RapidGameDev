using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkateboardMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController player;
    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float speed = 1;
    [SerializeField]
    private float originalStepOffset;
    [SerializeField]
    private float jumpSpeed;

    private float direction;
    private float ySpeed;
    private Vector2 moveVector = new Vector2(0f,0f);
    Vector2 velocity;

    void Start()
    {
        Vector2 ans = findComponents(37, 14);
        Debug.Log(ans.x + " x");
        Debug.Log(ans.y + " y");
    }
    void Update()
    {
        float direction = Input.GetAxis("Horizontal");
        Vector2 movementDirection = new Vector2(direction, 0f);
        

        ySpeed += Physics.gravity.y * Time.deltaTime;

        if(player.isGrounded)
        {
            player.stepOffset = originalStepOffset;
            ySpeed = -0.5f;

            if (Input.GetButtonDown("Jump"))
            {
                ySpeed = jumpSpeed;
            }
        }
        else
        {
            player.stepOffset = 0;
        }

        velocity += movementDirection * speed * acceleration;
        velocity = adjustVelocityToSlope(velocity);
        velocity.y += ySpeed;

       
        player.Move(velocity * Time.deltaTime);
    }
    private Vector2 adjustVelocityToSlope(Vector2 velocity)
    {
        var ray = new Ray(transform.position, Vector2.down);

        if(Physics.Raycast(ray, out RaycastHit hitInfo, 10f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * velocity;
            Debug.Log(adjustedVelocity);
            if(adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }
        return velocity;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (player != null)
        {
            //player.rotation = collision.transform.rotation;
        }
    }
    private Vector2 findComponents(float angle, float magnitude)
    {
        float rad = angle * Mathf.Deg2Rad;
        float opposite = magnitude * Mathf.Sin(rad);
        float adjacent = magnitude * Mathf.Cos(rad);

        return new Vector2(adjacent, opposite);
    }
}

