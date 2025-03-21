using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;

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

    [Header("Speedometer")]
    [Tooltip("Speedometer UI Element")]
    public GameObject speedometer;
    [Tooltip("Arrow game object")]
    public RectTransform arrow;
    [Tooltip("Arrow angle when player is slowest")]
    public float minAngle = 90.0f;
    [Tooltip("Arrow angle when player is fastest")]
    public float maxAngle = -90.0f;
    [Tooltip("Sparks particle system")]
    public ParticleSystem sparks;

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

    [Header("Pause")]
    [Tooltip("pause screen game object")]
    public GameObject pauseScreen;
    [Tooltip("input player must press to pause game")]
    public KeyCode pauseKey;
    [Tooltip("Quit Button Text GameObject")]
    public GameObject quitButtonText;

    [Header("Lose Screen")]
    [Tooltip("Lose screen game object")]
    public GameObject loseScreen;

    [Header("Trick Get")]
    [Tooltip("trick info screen game object")]
    public GameObject trickInfo;
    [Tooltip("description for ollie")]
    [TextArea(5, 15)]
    public string ollieDesc;
    [Tooltip("image for ollie")]
    public Sprite ollieImage;
    [Tooltip("description for uturn")]
    [TextArea(5, 15)]
    public string uturnDesc;
    [Tooltip("image for uturn")]
    public Sprite uturnImage;
    [Tooltip("description for backflip")]
    [TextArea(5, 15)]
    public string flipDesc;
    [Tooltip("image for backflip")]
    public Sprite flipImage;

    private GameObject[] collectableImage = new GameObject[3];
    private GameObject timerText;
    public GameObject timerClock;

    private bool paused;
    private bool canContinue;

    private string[] levelName = { "00_Tutorial", "02_Street", "03_Garden" };

    private GameObject trickName;
    private GameObject trickDesc;
    private GameObject trickImage;

    private GameManager gameManager;
    private LevelManager levelManager;
    private SkateboardMovementRigid playerScript;

    public enum Load {Level, Hub, Title}
    void Awake()
    {
        try
        {
            for (int i = 0; i < collectableImage.Length; i++)
            {
                collectableImage[i] = collectable.transform.GetChild(i).gameObject;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Something is wrong with the children of collectables, make sure the sock images are the first three children and are in order");
        }
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<SkateboardMovementRigid>();
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        //GameManager.ollieGet.AddListener(delegate{trickGet("ollie");});
        //GameManager.uturnGet.AddListener(delegate{trickGet("uturn");});
        //GameManager.flipGet.AddListener(delegate{trickGet("flip");});
        //Debug.Log(gameManager.gameState);
        if(gameManager==null) Debug.LogError("Game Manager missing from scene");
        if(!gameManager.enabled) Debug.LogError("Game Manager disabled");
        if (GameManager.gameState == GameManager.GameState.InLevel)
        {
            levelManager = FindObjectOfType<LevelManager>();
            if (levelManager==null) Debug.LogError("Level Manager missing from scene");
            if (!levelManager.enabled) Debug.LogError("Level Manager disabled");
            if (!collectable) Debug.LogError("CollectableUI Not assigned to UI Manager");
            if (!timer) Debug.LogError("Timer not assigned to UIManager");
            if (!speedometer) Debug.LogError("Speedometer Not assigned to UI Manager");
            if (!arrow) Debug.LogError("Arrow not assigned to UIManager");
            if (!sparks) Debug.LogError("Sparks not assigned to UIManager");
            if (!levelComplete) Debug.LogError("Level win screen not assigned to UI Manager");
            if (!trickInfo) Debug.LogError("Trick info screen not assigned to UI Manager");
            if (!scoreText) Debug.LogError("scoreText is not assigned to UI Manager");
            if (!pauseScreen) Debug.LogError("Pause Screen is not assigned to UI Manager");
            if (!loseScreen) Debug.LogError("Lose Screen is not assigned to Ui Manager");
            if (input.ToString().Equals("None")) Debug.LogError("Input Key not Assigned to UI Manager");
            try
            {
                for (int i = 0; i < collectableImage.Length; i++)
                {
                    collectableImage[i] = collectable.transform.GetChild(i).gameObject;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Something is wrong with the children of collectables, make sure the sock images are the first three children and are in order");
            }
            try
            {
                //timerClock = timer.transform.GetChild(0).gameObject;
                timerText = timer.transform.GetChild(1).gameObject;
                timerText.GetComponent<TMPro.TextMeshProUGUI>().text = "500";
            }
            catch (System.Exception ex2)
            {
                Debug.LogError("Something wrong with the timer text, make sure timer text is the second child of timer");
            }
            try
            {
                trickName = trickInfo.transform.GetChild(0).gameObject;
                trickImage = trickInfo.transform.GetChild(1).gameObject;
                trickDesc = trickInfo.transform.GetChild(2).gameObject;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Something is wrong with the children of trickInfo, make sure the trickName, trickImage, and trickDesc are the only children and are in that order");
            }
            collectable.SetActive(true);
            timer.SetActive(true);
            speedometer.SetActive(true);
            quitButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Quit to Hub";
        }
        else if (GameManager.gameState == GameManager.GameState.InHub)
        {
            quitButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Quit to Menu";
        }
        
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<SkateboardMovementRigid>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.gameState==GameManager.GameState.InLevel) updateSpeedometer();
        if(canContinue && Input.GetKey(input)){
            loadScene(Load.Hub);
        }
        if(Input.GetKeyDown(pauseKey)){
            if(Time.timeScale == 1) pauseGame();
            else if(Time.timeScale == 0 && paused) resumeGame();
        }
    }

    public void updateCollectables(int index, Sprite sock)
    {
        collectableImage[index].GetComponent<Image>().sprite = sock;
    }

    public void updateTimerText(int time)
    {
        string minute = (time/60).ToString();
        string second = (time%60).ToString();
        if (second.Length <= 1) second = "0" + second;
        timerText.GetComponent<TMPro.TextMeshProUGUI>().text = minute + ":" + second;
        if (time < 0) playerLose(); 
    }

    public void updateTimerSprite(int time, float totalTime)
    {
        timerClock.GetComponent<Image>().fillAmount = time / totalTime;
    }

    public void updateSpeedometer()
    {
        float speed = Mathf.Abs(playerScript.getSpeed());
        float maxSpeed = playerScript.getMaxManualSpeed();
        arrow.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(minAngle, maxAngle, speed/maxSpeed));
        if(speed > maxSpeed) sparks.Play();
        else sparks.Stop();
    }

    private void loadHub()
    {
        GameManager.gameState = GameManager.GameState.InHub;
        SceneManager.LoadScene("01_Hub");
    }

    private void loadMenu()
    {
        //Debug.Log("THIS IS A TEST");
        GameManager.gameState = GameManager.GameState.StartMenu;
        SceneManager.LoadScene("Title");
    }

    private void reloadLevel()
    {
        SceneManager.LoadScene(levelName[levelManager.getIndex()]);
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

    public void loadScene(Load loadType)
    {
        if (GameManager.gameProg < 2) GameManager.ollie = false;
        if (GameManager.gameProg < 3) GameManager.uturn = false;
        if (GameManager.gameProg < 4) GameManager.flip = false;

        Time.timeScale = 1;

        switch (loadType)
        {
            case Load.Hub:
                loadHub();
                break;
            case Load.Level:
                reloadLevel(); 
                break;
            case Load.Title:
                loadMenu();
                break;
        }
        //if (menu) loadMenu();
        //else loadHub();
    }

    public void loadSceneFromButton(int i)
    {
        if (i == 0) loadScene(Load.Hub);
        if (i == 1) loadScene(Load.Level);
        if (i == 2) loadScene(Load.Title);
    }

    void timerOver()
    {
        canContinue = true;
    }

    public void updatePopupText(string s)
    {
        popupText.GetComponent<TMPro.TextMeshProUGUI>().text = s;
    }

    public void enablePopupText()
    {
        popupText.SetActive(true);
    }

    public void disablePopupText()
    {
        popupText.SetActive(false);
    }

    public void pauseGame()
    {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        timer.SetActive(false);
        collectable.SetActive(false);
        speedometer.SetActive(false);
        paused = true;
    }

    public void playerLose()
    {
        Time.timeScale = 0;
        loseScreen.SetActive(true);
        pauseScreen.SetActive(false);
        timer.SetActive(false);
        collectable.SetActive(false);
        speedometer.SetActive(false);
        paused = false;
    }

    public void resumeGame()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        trickInfo.SetActive(false);
        if(GameManager.gameState == GameManager.GameState.InLevel)
        {
            timer.SetActive(true);
            collectable.SetActive(true);
            speedometer.SetActive(true);
        }
        paused = false;
    }

    public void trickGet(string trick)
    {
        if(string.Equals(trick, "ollie")) displayTrickInfo("Ollie", ollieImage, ollieDesc);
        if(string.Equals(trick, "uturn")) displayTrickInfo("U-Turn", uturnImage, uturnDesc);
        if(string.Equals(trick, "flip")) displayTrickInfo("Flip", flipImage, flipDesc);
    }

    private void displayTrickInfo(string name, Sprite image, string desc)
    {
        trickName.GetComponent<TMPro.TextMeshProUGUI>().text = name;
        trickImage.GetComponent<Image>().sprite = image;
        trickDesc.GetComponent<TMPro.TextMeshProUGUI>().text = desc;
        trickInfo.SetActive(true);
        Time.timeScale = 0;
        paused = true;
    }
}
