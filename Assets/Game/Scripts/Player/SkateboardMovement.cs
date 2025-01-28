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
    private float maxManualSpeed;
    [SerializeField]
    private float speed = 1;
    [SerializeField]
    private float originalStepOffset;
    [SerializeField]
    private float jumpSpeed;

    private float direction;
    private float vSpeed;
    private float gravStrength = 9.8f;
    private Vector2 moveVector = new Vector2(0f, 0f);

    Vector2 inputVelocity;  //Speed gained by holding down an input
    Vector2 dragVelocity;   //The resistance from the ground slowing the player down
    Vector2 momentumGain;   //Extra momemtum gained by gravity
    Vector2 velocity;       //The player's total velocity
    Vector2 gravity;

    void Start()
    {
        Vector2 ans = findComponents(37, 14);
        Debug.Log(ans.x + " x");
        Debug.Log(ans.y + " y");
    }
    void Update()
    {
        movePlayer();
        applyGravity();
        velocity = inputVelocity + gravity;
        velocity = adjustVelocityToSlope(velocity);
        matchRotation();
        player.Move(velocity * Time.deltaTime);
    }
    private void movePlayer()
    {
        float direction = Input.GetAxis("Horizontal");
        inputVelocity += new Vector2(direction * speed, 0f);
        inputVelocity.x = Mathf.Clamp(inputVelocity.x, -maxManualSpeed, maxManualSpeed);
    }
    private void applyGravity()
    {
        //Debug.Log(player.isGrounded);
        if(player.isGrounded == false)
        {
            vSpeed -= gravStrength * Time.deltaTime; 
        } 
        else
        {
            vSpeed = 0f;
        }
        gravity = new Vector2(0f, vSpeed);
    }
    private void matchRotation()
    {
        var ray = new Ray(player.transform.position, player.transform.rotation * Vector2.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 5f) && hitInfo.collider.gameObject.tag == "Floor")
        {
            player.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            Debug.Log(hitInfo.collider.gameObject.name);
        }
    }
    private Vector2 adjustVelocityToSlope(Vector2 velocity)
    {
        var ray = new Ray(player.transform.position, player.transform.rotation * Vector2.down);

        if(Physics.Raycast(ray, out RaycastHit hitInfo, 10f) && hitInfo.collider.gameObject.tag == "Floor")
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * velocity;
            return adjustedVelocity;   
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
    private void addMomentum()
    {
        Vector2 addedMomentum = findComponents(player.transform.rotation.eulerAngles.z, 9.8f); ;
        addedMomentum.x = addedMomentum.y;
        addedMomentum.y = 0f;
        //addedMomentum = rb.rotation * addedMomentum;
        //rb.AddForce(addedMomentum, ForceMode.Acceleration);
    }
    private Vector2 findComponents(float angle, float magnitude)
    {
        float rad = angle * Mathf.Deg2Rad;
        float opposite = magnitude * Mathf.Sin(rad);
        float adjacent = magnitude * Mathf.Cos(rad);

        return new Vector2(adjacent, opposite);
    }
}

