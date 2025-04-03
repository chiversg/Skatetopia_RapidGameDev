using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject resumeButton;
    private void Update()
    {
        if ((Input.GetAxis("ControllerX") != 0 || Input.GetAxis("ControllerY") != 0) && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(resumeButton);
        }
    }

    public void continueButton()
    {
        GameManager.gameState = GameManager.GameState.InHub;
        SceneManager.LoadScene("01_Hub");
    }

    public void quitButton()
    {
        Debug.Log("QUIT GAME!!!");
        Application.Quit();
    }
}
