using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTrigger : MonoBehaviour
{
    [Tooltip("Is the scene load the level exit, if not it'll be a hub entrance")]
    public bool levelExit;
    [Tooltip("If set to true will load sams test scene")]
    public bool debug;

    [Header("Hub Level Modifiers")]
    [Tooltip("Name of the scene to load, if level exit it will be automatically set to the hub")]
    public string sceneName;
    [Tooltip("Input player must press to trigger level load")]
    public KeyCode enter;   

    private bool playerInTrigger;

    //private GameObject sockUI;
    private UIManager uiManager;
    private LevelManager levelManager;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        levelManager = FindObjectOfType<LevelManager>();
        if(uiManager==null) Debug.LogError("UI Manager is not applied to Player_UI");
        if(!uiManager.enabled) Debug.LogError("UI Manager disabled");
        if(levelManager==null) Debug.LogError("No LevelManager in Scene");
        if(!levelManager.enabled) Debug.LogError("LevelManager Disabled");
        if(levelExit) sceneName = "Hub";
        if(debug) sceneName = "Test Scene Sam";
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
            else if(Input.GetKey(enter)) SceneManager.LoadScene(sceneName);
            playerInTrigger = false;
        }
    }

    private void OnTriggerEnter(Collider other){
        if(other.tag == "Player") playerInTrigger = true;
    }

    private void OnTriggerExit(Collider other){
        if(other.tag == "Player") playerInTrigger = false;
    }

    private void loadScene(){
        
    }

    private void levelCompleteScreen(){
        //sockUI.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        //sockUI.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        //sockUI.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
    }
}
