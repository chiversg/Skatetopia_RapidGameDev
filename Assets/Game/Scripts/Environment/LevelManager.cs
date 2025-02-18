using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public enum level {Tutorial, LevelOne, LevelTwo}
    [Tooltip("Which Level is this?")]
    public level Level;
    private int levelIndex;

    private bool[] socks = new bool[3];

    void Start()
    {
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
}
