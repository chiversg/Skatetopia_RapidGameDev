using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour
{
    public enum direction {Left,Right}
    [Tooltip("Direction the player approches ramp from")]
    public direction Direction;
    private int dir;
    [Tooltip("Angle of the ramp")]
    public float ang;

    [Header("Ramp Colliders")]
    [Tooltip("Mesh collider for the ramp")]
    public Collider meshCol;
    [Tooltip("Box collider for the ramp")]
    public Collider boxCol;

    private bool usable;
    private bool bounce;
    private float plyrEnterY;
    private float plyrExitY;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(Direction == direction.Left){
            dir = 1;
            usable = true;
        }
        else if(Direction == direction.Right){
            dir = -1;
            usable = false;
        }
    }

    private void Update()
    {
        bounce = player.GetComponent<SkateboardMovementRigid>().getGrounded();
    }
    private void OnTriggerEnter(Collider other){
        Debug.Log("Trigger has been Entered: "  + gameObject.name);
        if(other.tag == "Player" && usable){
            plyrEnterY = other.transform.position.y;
        }
    }

    private void OnTriggerExit(Collider other){
        Debug.Log("Trigger has been Exited: " + gameObject.name);
        if(other.tag == "Player"){
            plyrExitY = other.transform.position.y;
            if (usable && bounce)
            {
                Debug.Log("TESTINGTESTING");
                player.GetComponent<SkateboardMovementRigid>().addSpeed(0, (plyrExitY - plyrEnterY) * 2, false);
            }
            if (transform.position.x*dir < other.transform.position.x*dir){
                meshCol.enabled = false;
                usable = false;
            }
            else{
                meshCol.enabled = true;
                usable = true;
            }
        }
    }
}