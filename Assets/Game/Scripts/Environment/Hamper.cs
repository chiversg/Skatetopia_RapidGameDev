using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hamper : MonoBehaviour
{
    private bool inTrigger;
    private bool hampDelay;
    private UIManager uiManager;
    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inTrigger)
        {
            if (Input.GetButtonDown("Interact") && Time.timeScale == 1)
            {
                uiManager.enableHamper();
                uiManager.disablePopupText();
                Debug.Log("Hamper Activated");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            uiManager.updatePopupText("Press E to view Sock Collection ");
            uiManager.enablePopupText();
            inTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            uiManager.disablePopupText();
            inTrigger = false;
        }
    }

    public void setInTrigger(bool inT)
    {
        inTrigger = inT; 
    }
}
