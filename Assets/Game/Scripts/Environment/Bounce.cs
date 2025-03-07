using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    [Header("Bounce Forces")]
    [Tooltip("Vertical Force for the bounce")]
    public float vForce = -20.0f;
    [Tooltip("Horizontal Force for the bounce")]
    public float hForce = -20.0f;

    private GameObject player; 

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update(){
        //Vector3 pos = new Vector3(player.transform.position.x, player.transform.position.y-1, player.transform.position.z);
        //Vector3 dir = (this.transform.position - player.transform.position).normalized;
        //Debug.DrawLine (pos, pos + dir * 10, Color.red, Mathf.Infinity);
        //Debug.Log("NORMALIZED " + dir);
    }

    public void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
            Vector3 pos = new Vector3(player.transform.position.x, player.transform.position.y-1, player.transform.position.z);
            Vector3 dir = (this.transform.position - player.transform.position).normalized;
            //Vector3 distance = (transform.position - player.transform.position).Distance;
            //Debug.Log("DISTANCE: " + distance);
            //float xForce = Mathf.Abs(player.transform.position.x - transform.position.x);
            //float yForce = Mathf.Abs(player.transform.position.y - transform.position.y);
            //Debug.Log("BOunce force : " + xForce + "   " + yForce);
            Debug.Log(gameObject.name + " - XSPEED: " + dir.x*hForce + " YSPEED: " + dir.y*vForce);
            player.GetComponent<SkateboardMovementRigid>().addSpeed(dir.x*hForce, dir.y*vForce);
            //player.GetComponent<SkateboardMovement>().setDirection(direction);
        }
    }
}
