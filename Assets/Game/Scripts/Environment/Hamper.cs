using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hamper : MonoBehaviour
{
    private bool inTrigger;
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
            if (Input.GetKey(KeyCode.E) && Time.timeScale == 1)
            {
                uiManager.disablePopupText();
                uiManager.enableHamper();
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
}
