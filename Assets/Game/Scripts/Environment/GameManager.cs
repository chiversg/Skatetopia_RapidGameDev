using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }

            if (!instance)
            {
                Debug.LogError("No Game Manager Present !!!");
            }

            return instance;

        }
    }

    public enum GameState {
        StartMenu,
        InCutscene, 
        InLevel,
        InHub
    }

    public GameState gameState;
    private int gameProg;

    public static bool[,] socks = new bool[3,3];

    private List<ResetBehaviour> registeredForReset;

    private int levelBeat;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        registeredForReset = new List<ResetBehaviour>();
    }

    public static void RegisterForReset(ResetBehaviour resetBehaviour)
    {
        Instance.registeredForReset.Add(resetBehaviour);
    }
    
    public static void GameReset()
    {
        for(int i =0; i < instance.registeredForReset.Count; i ++)
        {
            Instance.registeredForReset[i].Reset();
        }
    }

    public void setGameState(int i){
        if(levelBeat < i) levelBeat = i;
    }

    public void setCollectable(int i, int j){
        //Debug.Log("Game Manager Set Collectable: " + j + " of Level: " + i + " to be True");
        socks[i,j] = true;
    }
    public bool getCollectableBool(int levIdx, int colIdx){
        //Debug.Log("TEST IS THIS TRUE OR FALSE: " + socks[levIdx,colIdx]);
        return socks[levIdx, colIdx];
    }

    public int getGameProg(){
        return gameProg;
    }

    public void setGameProg(int i){
        gameProg = i;
    }
}
