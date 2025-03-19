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
            SceneManager.LoadScene("Cutscene");
            GameManager.gameState = GameManager.GameState.InCutscene;
        }
        else
        {
            SceneManager.LoadScene("01_Hub");
            GameManager.gameState = GameManager.GameState.InHub;
        }
       
    }

    public void quitGame(){
        Debug.Log("QUIT GAME!!!");
        Application.Quit();
    }

    public void playGameDebug(){
        gameManager.setGameProg(4);
        SceneManager.LoadScene("01_Hub");
    }
}
