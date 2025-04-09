using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    private bool inTrigger;
    private bool hampDelay;
    private bool buttonPressed;
    private UIManager uiManager;
    private int interactSwitch = -1;
    public Animator dogAnim;
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
                dogAnim.SetTrigger("Pet");
                //transform.localScale = new Vector3(2, 2, 2);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            uiManager.updatePopupText("Pet Daisy");
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
}