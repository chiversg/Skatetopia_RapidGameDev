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
        /*if ((Input.GetAxis("ControllerX") != 0 || Input.GetAxis("ControllerY") != 0) && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(resumeButton);
        }*/
        if ((Mathf.Abs(Input.GetAxisRaw("Horizontal")) >= 0.01 || Mathf.Abs(Input.GetAxisRaw("Vertical")) >= 0.01) && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(resumeButton);
        }
        else if ((Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) && EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
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
