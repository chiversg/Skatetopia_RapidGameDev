using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    public void continueButton()
    {
        SceneManager.LoadScene("01_Hub");
    }

    public void quitButton()
    {
        Debug.Log("QUIT GAME!!!");
        Application.Quit();
    }
}
