using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public enum level {Tutorial, LevelOne, LevelTwo}
    [Tooltip("Which Level is this?")]
    public level Level;
    [Tooltip("How many seconds does the player have to complete level?")]
    public float realTimeTimer = 500;
    [Tooltip("Number of points collectables reward at end of level")]
    public int collectableScore = 250;
    [Tooltip("Number of points each second left rewards at the end of the level")]
    public int timerScore = 5;

    private int levelIndex;
    private int intTimer = 500;

    private bool[] socks = new bool[3];

    private UIManager uiManager;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        if(uiManager==null) Debug.LogError("UI Manager is not applied to Player_UI");
        if(!uiManager.enabled) Debug.LogError("UI Manager disabled");
        if(Level == level.Tutorial) levelIndex = 0;
        else if(Level == level.LevelOne) levelIndex = 1;
        else if(Level == level.LevelTwo) levelIndex = 2;
        Collectable[] collectables = FindObjectsOfType(typeof(Collectable)) as Collectable[];
        //Debug.Log("TESTING THE LEVEL LOADING THINGY");
        foreach(var c in collectables){
            if(FindObjectOfType<GameManager>().getCollectableBool(levelIndex, c.getIdx())){
                //Debug.Log("TESTING GAME MANAGER BOLLEAN");
                c.setCollected(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        realTimeTimer -= Time.deltaTime;
        if(MathF.Floor(realTimeTimer)<intTimer){
            intTimer = (int) MathF.Floor(realTimeTimer);
            uiManager.updateTimer(intTimer);
        }
    }

    public void collectSock(int i){
        socks[i] = true;
    }

    public void recordSocks(){
        for(int i=0; i<socks.Length; i++){
            if(socks[i]){
                //Debug.Log("Recorded Sock: " + i);
                FindObjectOfType<GameManager>().setCollectable(levelIndex, i);
            } 
        }
    }

    public int calculateScore(){
        int score = 0;
        for(int i=0; i<socks.Length; i++){
            if(socks[i]) score += collectableScore;
        }
        score += intTimer * timerScore;
        return score;
    }
}
