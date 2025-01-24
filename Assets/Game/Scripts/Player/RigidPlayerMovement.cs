using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RigidPlayerMovement : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    float acceleration;
    void Update()
    {
        float direction = Input.GetAxis("Horizontal");
        Vector2 velocity = new Vector2(direction * acceleration, 0f);
        velocity = adjustVelocityToSlope(velocity);
        Debug.Log(velocity);
        rb.AddForce(velocity, ForceMode.Acceleration);
    }
    private void OnCollisionEnter(Collision collision)
    {
         rb.rotation = collision.transform.rotation;
    }
    private Vector2 adjustVelocityToSlope(Vector2 velocity)//Rotates the velocity to be parallel to the slope the player is on
    {
        var ray = new Ray(transform.position, Vector2.down);//Create a new raycast at player position and pointing down

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 5f))//Shoot the raycast out 5 units and return whatever is hit as hitInfo
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * velocity;//From my understanding multiplying a vector by a quaternion rotates that vector into the direction of the quaternion
            return adjustedVelocity;  
        }
        return velocity;
    }   
}
