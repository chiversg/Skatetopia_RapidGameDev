using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hamper : MonoBehaviour
{
    private bool inTrigger;
    private bool hampDelay;
    private UIManager uiManager;
    private int interactSwitch = -1;
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
            if (Input.GetButtonUp("Interact") && Time.timeScale == 1 && interactSwitch == 1)
            {
                Debug.Log("Hamper Activated");
                Time.timeScale = 0;
                uiManager.enableHamper();
                FindObjectOfType<UIManager>().enableHamper();
                uiManager.disablePopupText();
                //transform.localScale = new Vector3(2, 2, 2);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            uiManager.updatePopupText("View Sock Collection");
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

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            if (Input.GetButtonDown("Interact") || Input.GetButtonDown("Pause"))
            {
                interactSwitch *= -1;
                Debug.Log(interactSwitch);
            }
        }
    }

    public void setInTrigger(bool inT)
    {
        inTrigger = inT; 
    }
}
