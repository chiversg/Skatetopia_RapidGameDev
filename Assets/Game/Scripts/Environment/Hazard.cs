using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour 
{
	private GameObject player;
	private PlayerHit playerScript;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		playerScript = player.GetComponent<PlayerHit>();
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player"){
			Debug.Log("Player has entered Hazard area");
			playerScript.playerHitHazard();
		}
	}
}

/* NOTE: a nice feature of unity is that the trigger enter check works with a child object trigger
 * so you might have a physical collider on the actual object, then a child trigger for the damage area
 * for example: a lawnmower which the player can stand on, and a blade on the front which damages objects */