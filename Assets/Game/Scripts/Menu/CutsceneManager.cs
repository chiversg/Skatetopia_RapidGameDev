using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Tooltip("List holding Empty Gameobjects, each one holds all the panels for a specific cutscene")]
    public List<GameObject> cutscenes;

    [Tooltip("Key to press to continue at end")]
    public KeyCode accept;

    [Tooltip ("Length between each panel in the cutscene")]
    public float length;

    private int panelIndex = 0;
    private int cutsceneIndex = 0;
    private bool cutsceneEnd;

    void Awake(){
        foreach (GameObject cutscene in cutscenes)
        {
            foreach (Transform child in cutscene.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    void Start()
    {
        if (GameManager.gameProg == 0) showCutscene(0);
        else if (GameManager.gameProg == 2) showCutscene(1);
        else if (GameManager.gameProg == 7) showCutscene(2);
    }

    private void Update()
    {
        if(cutsceneEnd && (Input.GetKey(accept) || accept == KeyCode.None))
        {
            if (cutsceneIndex == 0)
            {              
                GameManager.gameProg = 1;
                SceneManager.LoadScene("00_Tutorial");
            }
            else if (cutsceneIndex == 1)
            {               
                GameManager.gameProg = 3;
                SceneManager.LoadScene("01_Hub");
            }
            else if (cutsceneIndex == 2)
            {               
                GameManager.gameProg = 8;
                SceneManager.LoadScene("Game_Over");
            }
        }
    }

    public void showCutscene(int i){
        cutsceneIndex = i;
        Debug.Log("Cutscene Test: " + panelIndex);
        cutscenes[i].transform.GetChild(panelIndex).gameObject.SetActive(true);
        Timer t = gameObject.AddComponent<Timer>();
        t.TimerEnded.AddListener(timerEnded);
        t.setTimer(length);
        t.startTimer();
        panelIndex++;
    }

    public void timerEnded(){
        if (panelIndex == cutscenes[cutsceneIndex].transform.childCount)
        {
            cutsceneEnd = true;
        }
        else showCutscene(cutsceneIndex);
    }
}