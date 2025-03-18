using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    [Header("Bounce Forces")]
    [Tooltip("Vertical Force for the bounce")]
    public float force = 5.0f;

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
            if(player.transform.position.y > this.transform.position.y) player.GetComponent<SkateboardMovementRigid>().addSpeed(0, force, true);
        }
    }
}
