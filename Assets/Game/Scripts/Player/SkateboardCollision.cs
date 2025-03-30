using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkateboardCollision : MonoBehaviour
{
    private GameObject player;
    private SkateboardMovementRigid playerScript;

    public bool garden;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<SkateboardMovementRigid>();
    }

    // Update is called once per frame
    void Update()
    {
        if(garden) transform.position = new Vector3(playerScript.getPosition().x, playerScript.getPosition().y - 1.25f, playerScript.getPosition().z);
        else transform.position = new Vector3(playerScript.getPosition().x, playerScript.getPosition().y-1, playerScript.getPosition().z);
        transform.rotation = playerScript.getRotation();
    }
}
