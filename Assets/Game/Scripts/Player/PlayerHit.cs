using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    [Tooltip("How many seconds the player should be immune to hazards for")]
    public float invincibilityTimer = 2.5f;

    private float timer;
    private bool invincible = false;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        timer = invincibilityTimer;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(invincible == true){
            timer -= Time.deltaTime;
            if(timer <= 0.0f){
                timerEnded();
            }
        }
    }

    private void timerEnded(){
        Debug.Log("Timer Ended 123");
        invincible = false;
        timer = invincibilityTimer;
    }

    public void playerHitHazard(){
        Debug.Log("Timer TEST");
        if(!invincible){
            Debug.Log("Timer Has Started");
            player.GetComponent<SkateboardMovement>().hitHazard();
            invincible = true;
        }
    }
}
