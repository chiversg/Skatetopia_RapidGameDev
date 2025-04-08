using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Swap : MonoBehaviour
{
    [Tooltip("Sprite for Keyboard UI")]
    public GameObject keyboardObject;
    [Tooltip("Sprite for Controller UI")]
    public GameObject controllerObject;

    //public GameObject go1;
    //public GameObject go2;

    private GameManager.InputMode thisInput;
    // Start is called before the first frame update
    void Start()
    {
        thisInput = GameManager.currentInput;
    }

    // Update is called once per frame
    void Update()
    {
        if (thisInput != GameManager.currentInput)
        {
            if (GameManager.currentInput == GameManager.InputMode.Controller)
            {
                //Controller is being used
                //Debug.Log("Controller");
                if (controllerObject.active == true || keyboardObject.active == true)
                {
                    controllerObject.SetActive(true);
                    keyboardObject.SetActive(false);
                }
                thisInput = GameManager.currentInput;
            }
            if (GameManager.currentInput == GameManager.InputMode.Keyboard)
            {
                //Keyboard is being used
                //Debug.Log("Keyboard");
                if (controllerObject.active == true || keyboardObject.active == true)
                {
                    controllerObject.SetActive(false);
                    keyboardObject.SetActive(true);
                }
                thisInput = GameManager.currentInput;
            }
        }
        if (GameManager.currentInput == GameManager.InputMode.Controller)
        {
            //Controller is being used
            //Debug.Log("Controller");
            if (controllerObject.active == true || keyboardObject.active == true)
            {
                controllerObject.SetActive(true);
                keyboardObject.SetActive(false);
            }
            thisInput = GameManager.currentInput;
        }
    }
}
