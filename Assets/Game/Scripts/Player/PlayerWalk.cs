using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerWalk : MonoBehaviour
{
    [SerializeField] private Rigidbody player;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer character;

    private float xSpeed;
    private bool lookUp;
    private float direction;
    private float lastFacedDirection;
    private bool isListening = false;
    void Start()
    {

    }

    private void Update()
    {
        if (!isListening)
        {
            direction = Input.GetAxis("Horizontal");
            lastFacedDirection = direction != 0 ? direction : lastFacedDirection;
            xSpeed = player.velocity.x;
            flipSprite();
            checkInputs();
            //Debug.Log("Ger");
            animator.SetFloat("Speed", Mathf.Abs(xSpeed));
            animator.SetBool("heldUp", lookUp);
        }
        else
        {
            animator.SetBool("listening", true);
        }
    }
    private void FixedUpdate()
    {
        if (!isListening)
        {
            player.AddForce(Vector3.right * direction * 1.3f, ForceMode.Impulse);
        }
    }
    private void checkInputs()
    {
        if (Input.GetButton("LookUp"))
        {
            lookUp = true;
        }
        else
        {
            lookUp = false;
        }
    }
    private void flipSprite()
    {
        //Debug.Log(xSpeed);
        if (lastFacedDirection < 0)
        {
            character.flipX = false;
        }
        else
        {
            character.flipX = true;
        }
    }

    public void enterDialogue()
    {
        isListening = true;
    }
    public void exitDialogue()
    {
        isListening = false;
        animator.SetBool("listening", false);
    }
}
