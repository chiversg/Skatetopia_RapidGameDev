using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTrigger : MonoBehaviour
{
    public enum Level{Tutorial, LevelOne, LevelTwo, Hub}
    [Tooltip("Is the scene load the level exit, if not it'll be a hub entrance")]
    public bool levelExit;
    [Tooltip("If set to true will load sams test scene")]
    public bool debug;

    [Header("Hub Level Modifiers")]
    [Tooltip("Level that this door will load into")]
    public Level level;
    [Tooltip("Input player must press to trigger level load")]
    public KeyCode enter;   

    private bool playerInTrigger;
    private string levelName;
    private string officialLevelName;

    //private GameObject sockUI;
    private UIManager uiManager;
    private LevelManager levelManager;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        levelManager = FindObjectOfType<LevelManager>();
        if(uiManager==null) Debug.LogError("UI Manager is not applied to Player_UI");
        if(!uiManager.enabled) Debug.LogError("UI Manager disabled");
        if(levelExit&&levelManager==null) Debug.LogError("No LevelManager in Scene");
        if(levelExit&&!levelManager.enabled) Debug.LogError("LevelManager Disabled");

        if(level==Level.Tutorial){
            levelName = "Tutorial";
            officialLevelName = "00_Tutorial";
        }
        else if(level==Level.LevelOne){
            levelName = "Level One";
            officialLevelName = "02_Street";
        }
        else if(level==Level.LevelTwo){
            levelName = "Level Two";
            officialLevelName = "03_Garden";
        }
        //sockUI = GameObject.FindGameObjectWithTag("Collectable UI");
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInTrigger){
            if(levelExit) {
                levelManager.recordSocks();
                uiManager.levelWinScreen(levelManager.calculateScore());
            }
            else{
                if(Input.GetKey(enter)){
                    SceneManager.LoadScene(officialLevelName);
                    playerInTrigger = false;
                } 
            } 
        }
    }

    private void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
            playerInTrigger = true;
            if(!levelExit){
                uiManager.updatePopupText("Press " + enter + " to Enter " + levelName);
                uiManager.enablePopupText();
            }
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.tag == "Player"){
            playerInTrigger = false;
            if(!levelExit) uiManager.disablePopupText();
        }
    }

    private void loadScene(){
        
    }

    private void levelCompleteScreen(){
        //sockUI.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        //sockUI.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        //sockUI.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
    }
}
