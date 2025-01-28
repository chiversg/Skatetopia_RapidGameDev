using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class RigidPlayerMovement : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    float acceleration;

    Boolean isJumping;
    private void Start()
    {
        
    }
    void Update()
    {
        float direction = Input.GetAxis("Horizontal");
        Vector2 velocity = new Vector2(direction * acceleration, 0f);
        velocity = adjustVelocityToSlope(velocity);
        rb.AddForce(velocity, ForceMode.Acceleration);
        Debug.Log(isGrounded());
        matchRotation();
        StartCoroutine(moveToFloor());

    }
    private void matchRotation()
    {
        var ray = new Ray(transform.position, transform.rotation * Vector2.down);

        if(Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f))
        {
            rb.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            Debug.Log(rb.rotation);
        }
    }
    IEnumerator moveToFloor()
    {
        var ray = new Ray(transform.position, rb.rotation * Vector2.down);
      
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 5f)) 
        {
            while (isGrounded() == false)
            {
                Vector3 movePos = rb.transform.rotation * Vector3.down;
                rb.AddForce(movePos,ForceMode.Force);
                yield return null;
                
            }
        }
    }
    private Boolean isGrounded()
    {
        var ray = new Ray(transform.position, transform.rotation * Vector2.down);
        return Physics.Raycast(ray, 1.1f);
    }
    private Vector2 adjustVelocityToSlope(Vector2 velocity)//Rotates the velocity to be parallel to the slope the player is on
    {
        var ray = new Ray(transform.position, transform.rotation * Vector2.down);//Create a new raycast at player position and pointing down

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 5f))//Shoot the raycast out 5 units and return whatever is hit as hitInfo
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * velocity;//From my understanding multiplying a vector by a quaternion rotates that vector into the direction of the quaternion
            return adjustedVelocity;
        }
        return velocity;
    }
}
