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
            uiManager.trickGet(Trick.ToString());
            Destroy(this.gameObject);
        }
    }
}
