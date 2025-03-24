using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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

    public void quitGame(){
        Debug.Log("QUIT GAME!!!");
        Application.Quit();
    }

    public void playGameDebug(){
        gameManager.setDebug();
        SceneManager.LoadScene("01_Hub");
    }
}
