using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.gameState = GameManager.GameState.InHub;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
