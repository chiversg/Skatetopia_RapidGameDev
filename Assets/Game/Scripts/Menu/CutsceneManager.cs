using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Tooltip ("Empty Gameobject holden all the panels for the cutscene")]
    public GameObject panels;
    [Tooltip ("Length between each panel in the cutscene")]
    public float length;

    private int index = 0;

    void Awake(){
        if(!panels) Debug.LogError("Panels not assigned in inspector!!!!");
        foreach(Transform child in panels.transform){
            child.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        showPanel();
    }

    public void showPanel(){
        Debug.Log("Cutscene Test: " + index);
        panels.transform.GetChild(index).gameObject.SetActive(true);
        Timer t = gameObject.AddComponent<Timer>();
        t.TimerEnded.AddListener(timerEnded);
        t.setTimer(length);
        t.startTimer();
        index++;
    }

    public void timerEnded(){
        if (index == panels.transform.childCount)
        {
            GameManager.gameProg = 1;
            SceneManager.LoadScene("00_Tutorial");
        }
        else showPanel();
    }
}