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
    private bool[] bounce = new bool[5];
    private int bounceIndex = 0;
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
        
    }

    private void OnTriggerEnter(Collider other){
        Debug.Log("Trigger has been Entered: "  + gameObject.name);
        if(other.tag == "Player" && usable){
            plyrEnterY = other.transform.position.y;
        }
    }
    
    private void OnTriggerStay(Collider other){
        if(other.tag == "Player"){
            Debug.Log(bounce);
            bounce[bounceIndex] = player.GetComponent<SkateboardMovementRigid>().getGrounded();
            bounceIndex++;
            if(bounceIndex>=5) bounceIndex = 0;
        }
    }

    private void OnTriggerExit(Collider other){
        Debug.Log("Trigger has been Exited: " + gameObject.name + ", " + bounce);
        if(other.tag == "Player"){
            bool bounceFinal = false;
            foreach(bool b in bounce){
                if(b) bounceFinal = true;
            }
            plyrExitY = other.transform.position.y;
            if (usable && bounceFinal)
            {
                //Debug.Log("TESTINGTESTING");
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