using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject playButton;
    public GameObject creditsQuitButton;

    private GameManager gameManager;

    private GameObject currButton;
    private void Awake()
    {
        GameManager.gameState = GameManager.GameState.StartMenu;
    }
    void Start(){
        gameManager = FindObjectOfType<GameManager>();
        if(gameManager==null) Debug.LogError("Game Manager missing from scene");
        if(!gameManager.enabled) Debug.LogError("Game Manager disabled");
        currButton = playButton;
        Debug.Log("STARTUP");
    }

    void Update()
    {
        //Debug.Log("CurrButton: " + currButton.name);
        //Debug.Log("Horizontal: " + Input.GetAxis("Horizontal") + ", Vertical: " + Input.GetAxis("Vertical"));
        if (Mathf.Abs(Input.GetAxis("Horizontal")) >= 0.01 || Mathf.Abs(Input.GetAxis("Vertical")) >= 0.01)
        {
            //Debug.Log("LAYER ONE");
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                Debug.Log("EVENT SYSTEM ADDED: " + currButton.name);
                EventSystem.current.SetSelectedGameObject(currButton);
            }
        }
        else if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                Debug.Log("EVENT SYSTEM REMOVED: " + currButton.name);
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        /*if ((Input.GetAxis("ControllerX") != 0 || Input.GetAxis("ControllerY") != 0) && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(playButton);
        }
        if (Input.anyKey || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }*/
    }

    public void playGame(){
        if (GameManager.gameProg == 0){
            GameManager.gameState = GameManager.GameState.InCutscene;
            SceneManager.LoadScene("Cutscene");
        }
        else if(GameManager.gameProg == 1)
        {
            GameManager.gameState = GameManager.GameState.InLevel;
            SceneManager.LoadScene("00_Tutorial");
        }
        else
        {
            GameManager.gameState = GameManager.GameState.InHub;
            SceneManager.LoadScene("01_Hub");
        }
    }

    public void credits()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(creditsQuitButton);
        currButton = creditsQuitButton;
        Debug.Log(currButton.name + " AND " + creditsQuitButton.name);
    }

    public void quitCredits()
    {
        Debug.Log("CREDITS HAVE NBEEN QUOT");
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(playButton);
        currButton = playButton;
    }

    public void quitGame(){
        Debug.Log("QUIT GAME!!!");
        Application.Quit();
    }

    public void playGameDebug(){
        gameManager.setDebug();
        SceneManager.LoadScene("01_Hub");
    }
}
