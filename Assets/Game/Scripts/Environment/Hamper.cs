using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hamper : MonoBehaviour
{
    private bool inTrigger;
    private bool hampDelay;
    private bool buttonPressed;
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
        if ((Input.GetButtonDown("Interact") || Input.GetButtonDown("Pause")) && inTrigger) buttonPressed = true;
        //Debug.Log(interactSwitch);
        if (inTrigger)
        {
            if (Input.GetButtonDown("Interact") && Time.timeScale == 1)// && interactSwitch == 1)
            {
                Timer t = gameObject.AddComponent<Timer>();
                t.TimerEnded.AddListener(enableHamper);
                t.setTimer(0.1f);
                t.startTimer();
                Time.timeScale = 0.0f;
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
        //Debug.Log("IN HAMPER");
        if(other.tag == "Player")
        {
            //Debug.Log("player");
            if (buttonPressed)
            {
                interactSwitch *= -1;
                //Debug.Log(interactSwitch);
            }
        }
    }

    private void enableHamper()
    {
        Debug.Log("Hamper Activated");
        Time.timeScale = 0;
        uiManager.enableHamper();
        FindObjectOfType<UIManager>().enableHamper();
        uiManager.disablePopupText();
    }

    public void setInTrigger(bool inT)
    {
        inTrigger = inT; 
    }
}
