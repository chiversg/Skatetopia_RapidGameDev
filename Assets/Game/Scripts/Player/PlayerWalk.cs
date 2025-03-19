using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerWalk : MonoBehaviour
{
    [SerializeField] private Rigidbody player;
    [SerializeField] private Animator animator;

    private float xSpeed;
    private bool lookUp;
    void Start()
    {
        
    }

    private void Update()
    {
        xSpeed = player.velocity.x;
        flipSprite();
        checkInputs();
        //Debug.Log("Ger");
        animator.SetFloat("Speed", Mathf.Abs(xSpeed));
        animator.SetBool("heldUp", lookUp);
    }
    private void FixedUpdate()
    {
        player.AddForce(Vector3.right * Input.GetAxis("Horizontal") * 1.3f, ForceMode.VelocityChange);
    }
    private void checkInputs()
    {
        if (Input.GetButton("LookUp"))
        {
            lookUp = true;
        } else
        {
            lookUp = false;
        }
    }
    private void flipSprite()
    {
        //Debug.Log(xSpeed);
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
}
