using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GameManager gameManager;

    void Start(){
        gameManager = FindObjectOfType<GameManager>();
        if(gameManager==null) Debug.LogError("Game Manager missing from scene");
        if(!gameManager.enabled) Debug.LogError("Game Manager disabled");
    }

    public void playGame(){
        SceneManager.LoadScene("Cutscene");
    }

    public void quitGame(){
        Debug.Log("QUIT GAME!!!");
        Application.Quit();
    }

    public void playGameDebug(){
        gameManager.setGameProg(2);
        SceneManager.LoadScene("01_Hub");
    }
}
