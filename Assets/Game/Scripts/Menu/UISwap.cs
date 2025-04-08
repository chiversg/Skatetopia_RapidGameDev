using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISwap : MonoBehaviour
{
    [Tooltip("Is this image actually on the Player UI canvas or not")]
    public bool onCanvas;
    [Tooltip("Sprite for Keyboard UI")]
    public Sprite keyboardSprite;
    [Tooltip("Sprite for Controller UI")]
    public Sprite controllerSprite;

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
                if(onCanvas) GetComponent<Image>().sprite = controllerSprite;
                else GetComponent<SpriteRenderer>().sprite = controllerSprite;
                thisInput = GameManager.currentInput;
            }
            if (GameManager.currentInput == GameManager.InputMode.Keyboard)
            {
                //Keyboard is being used
                //Debug.Log("Keyboard");
                if (onCanvas) GetComponent<Image>().sprite = keyboardSprite;
                else GetComponent<SpriteRenderer>().sprite = keyboardSprite;
                thisInput = GameManager.currentInput;
            }
        }
        if(thisInput == GameManager.InputMode.Controller)
        {
            if (onCanvas) GetComponent<Image>().sprite = controllerSprite;
            else GetComponent<SpriteRenderer>().sprite = controllerSprite;
        }
    }
}
