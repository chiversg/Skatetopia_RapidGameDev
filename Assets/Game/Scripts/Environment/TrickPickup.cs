using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickPickup : MonoBehaviour
{
    public enum trick {
        ollie,
        uturn,
        flip
    }

    [Tooltip("What trick is the pickup?")]
    public trick Trick;

    private UIManager uiManager;
    void Start(){
        uiManager = FindObjectOfType<UIManager>();
        if(uiManager==null) Debug.LogError("UI Manager is not applied to Player_UI");
        if(!uiManager.enabled) Debug.LogError("UI Manager disabled");
    }

    private void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
            if (Trick == trick.ollie) GameManager.ollie = true;
            else if (Trick == trick.uturn) GameManager.uturn = true;
            else if(Trick == trick.flip) GameManager.flip = true;
            uiManager.trickGet(Trick.ToString());
            Destroy(this.gameObject);
        }
    }
}
