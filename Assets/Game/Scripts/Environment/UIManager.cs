using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //[Tooltip("Is the level UI such as collectables, speedometer, and timer in the scene?")]
    //public bool levelUI;
    
    [Header("Collectables")]
    [Tooltip("collectable UI element")]
    public GameObject collectable;
    [Tooltip("Number of points collectables reward at end of level")]
    public int collectableScore = 250;
    [Header("Timer")]
    [Tooltip("Timer UI Element")]
    public GameObject timer;
    [Tooltip("Number of points each second left rewards at the end of the level")]
    public int timerScore = 5;
    [Header("Pop-up Text")]
    [Tooltip("Pop up text element in the UI")]
    public GameObject popupText;
    [Header("Level Complete Screen")]
    [Tooltip("level complete screen element")]
    public GameObject levelComplete;
    [Tooltip("Gameobject holding the text that'll display the score")]
    public GameObject scoreText;
    [Tooltip("amount of time before player can move onto the level load")]
    public float screenTimer = 2.5f;
    [Tooltip("Button player must press to continue")]
    public KeyCode input;

    private GameObject[] collectableImage = new GameObject[3];
    private GameObject timerText;
    private bool canContinue;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        //Debug.Log(gameManager.gameState);
        if(gameManager==null) Debug.LogError("Game Manager missing from scenen");
        if(!gameManager.enabled) Debug.LogError("Game Manager disabled");
        if(gameManager.gameState==GameManager.GameState.InLevel){
            if(!collectable) Debug.LogError("CollectableUI Not assigned to UI Manager");
            if(!timer) Debug.LogError("Timer not assigned to UIManager");
            if(!levelComplete) Debug.LogError("Level win screen not assigned to UI Manager");
            if(!scoreText) Debug.LogError("scoreText is not assigned to UI Manager");
            if(input.ToString().Equals("None")) Debug.LogError("Input Key not Assigned to UI Manager");
            try{
                for(int i=0; i<collectableImage.Length; i++){
                    collectableImage[i] = collectable.transform.GetChild(i).gameObject;
                }
            }
            catch(System.Exception ex){
                Debug.LogError("Something is wrong with the children of collectables, make sure the sock images are the first three children and are in order");
            }
            try{
                timerText = timer.transform.GetChild(1).gameObject;
                timerText.GetComponent<TMPro.TextMeshProUGUI>().text = "500";
            }
            catch(System.Exception ex2){
                Debug.LogError("Something wrong with the timer text, make sure timer text is the second child of timer");
            }
            collectable.SetActive(true);
            timer.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(canContinue && Input.GetKey(input)){
            Time.timeScale = 1;
            gameManager.gameState = GameManager.GameState.InHub;
            SceneManager.LoadScene("01_Hub");
        }
    }

    public void updateCollectables(int index, Sprite sock){
        collectableImage[index].GetComponent<Image>().sprite = sock;
    }

    public void updateTimer(int time){
        timerText.GetComponent<TMPro.TextMeshProUGUI>().text = time.ToString();
    }

    public void levelWinScreen(int score){
        //createTimer();
        //setLevelWin();
        
        Timer t = gameObject.AddComponent<Timer>();
        t.TimerEnded.AddListener(timerOver);
        //PlayerHit pHit = gameObject.AddComponent<PlayerHit>();
        //pHit.playerHitHazard();
        t.setTimer(screenTimer);
        t.startTimer();
        Time.timeScale = 0;
        levelComplete.SetActive(true);
        collectable.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        collectable.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        collectable.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 30, 0);
        timer.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        timer.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        timer.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -35, 0);
        scoreText.GetComponent<TMPro.TextMeshProUGUI>().text = "Score: " + score;
    }

    void timerOver(){
        canContinue = true;
    }

    public void updatePopupText(string s){
        popupText.GetComponent<TMPro.TextMeshProUGUI>().text = s;
    }

    public void enablePopupText(){
        popupText.SetActive(true);
    }

    public void disablePopupText(){
        popupText.SetActive(false);
    }
}
