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

    private void Awake()
    {
        GameManager.gameState = GameManager.GameState.StartMenu;
    }
    void Start(){
        gameManager = FindObjectOfType<GameManager>();
        if(gameManager==null) Debug.LogError("Game Manager missing from scene");
        if(!gameManager.enabled) Debug.LogError("Game Manager disabled");
    }

    void Update()
    {
        if ((Input.GetAxis("ControllerX") != 0 || Input.GetAxis("ControllerY") != 0) && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(playButton);
        }
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
    }

    public void quitCredits()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(playButton);
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
