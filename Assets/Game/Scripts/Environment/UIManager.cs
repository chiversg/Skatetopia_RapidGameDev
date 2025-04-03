using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEngine.EventSystems;

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

    [Header("Pop-up Alert")]
    [Tooltip("Pop-up alert element in UI")]
    public GameObject alert;
    [Tooltip("alert text element")]
    public GameObject alertText;

    [Header("Level Complete Screen")]
    [Tooltip("level complete screen element")]
    public GameObject levelComplete;
    [Tooltip("Gameobject holding the text that'll display the flavour text for rank")]
    public GameObject rankFlavourText;
    [Tooltip("amount of time before player can move onto the level load")]
    public float screenTimer = 2.5f;
    [Tooltip("Button player must press to continue")]
    public KeyCode input;
    [Tooltip("Game object that holds rank images")]
    public GameObject ranks;
    [Tooltip("Time remaining text object")]
    public GameObject timeRemainText;
    [Tooltip("Timer sprite for level complete")]
    public GameObject levelCompleteTimer;


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
    [Tooltip("Name of trick that player gets in this level")]
    public string quitTrick;
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

    [Header("Sock Hamper")]
    [Tooltip("Sock Screen")]
    public GameObject sockScreen;
    [Tooltip("Array of socks")]
    public GameObject[] hamperSocks;
    [Tooltip("sock collected sprite")]
    public Sprite sockCollected;
    [Tooltip("sock not collected sprite")]
    public Sprite sockNotCollected;

    private GameObject[] collectableImage = new GameObject[3];
    private GameObject[] rankSprite = new GameObject[5];
    private GameObject timerText;
    public GameObject timerClock;

    private string flavourText = "";
    private int timeAmt;
    private float totalTimeAmt;

    private bool trickUp;
    private bool inHamper;
    private bool paused;
    private bool canContinue;
    private bool alertUp;
    private bool popText;

    private string[] levelName = { "00_Tutorial", "02_Street", "03_Garden" };

    private GameObject trickName;
    private GameObject trickDesc;
    private GameObject trickImage;

    private GameManager gameManager;
    private LevelManager levelManager;
    private SkateboardMovementRigid playerScript;

    private GameObject resumeButton;
    private GameObject retryButton;
    public enum Load {Level, Hub, Title, Cutscene}
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
        if (!alert) Debug.LogError("Alert not assigned to UIManager");
        if (!alertText) Debug.LogError("Alert Text not assigned to UIManager");
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
            if (!rankFlavourText) Debug.LogError("rankFlavourText is not assigned to UI Manager");
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
            try
            {
                for (int i = 0; i < rankSprite.Length; i++)
                {
                    rankSprite[i] = ranks.transform.GetChild(i).gameObject;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Something is wrong with the children of ranks, make sure the ranks are the only 5 children of rank letters game object");
            }
            collectable.SetActive(true);
            timer.SetActive(true);
            speedometer.SetActive(true);
            if(GameManager.gameProg>=2) quitButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Quit to Hub";
            else quitButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Quit to Menu";
        }
        else if (GameManager.gameState == GameManager.GameState.InHub)
        {
            if (!sockScreen) Debug.LogError("Hamper Screen not assigned to UIMAnager");
            quitButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Quit to Menu";
            if(GameManager.gameProg == 3 || GameManager.gameProg == 5)
            {
                Debug.Log("TESTING ALERT THING");
                alert.SetActive(true);
                alertText.GetComponent<TMPro.TextMeshProUGUI>().text = "Talk to Mom";
                alertUp = true;
                Timer t = gameObject.AddComponent<Timer>();
                t.TimerEnded.AddListener(alertTimerOver);
                t.setTimer(2.0f);
                t.startTimer();
            }
            checkSockCollected();
        }
        resumeButton = pauseScreen.gameObject.transform.GetChild(1).gameObject;
        retryButton = loseScreen.gameObject.transform.GetChild(2).gameObject;
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<SkateboardMovementRigid>();
        popupText.GetComponent<TextMeshProUGUI>().CrossFadeAlpha(0f, 0f, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey && !alertUp) disableAlert();
        if(GameManager.gameState==GameManager.GameState.InLevel) updateSpeedometer();
        if(canContinue && Input.GetButton("Interact")){
            if(GameManager.gameProg == 7 || GameManager.gameProg == 2) loadScene(Load.Cutscene);
            else loadScene(Load.Hub);
        }
        //if(Input.GetKeyDown(pauseKey)){
        if (Input.GetButtonDown("Pause") && !trickUp){ 
            if(Time.timeScale == 1) pauseGame();
            else if(Time.timeScale == 0 && paused) resumeGame();
        }
        if (trickUp && Input.GetButtonDown(quitTrick))
        {
            if(Time.timeScale == 0) resumeGame();
        }

        if ((Input.GetAxis("ControllerX") != 0 || Input.GetAxis("ControllerY") != 0) && EventSystem.current.currentSelectedGameObject == null)
        {
            if (timeAmt > 0) EventSystem.current.SetSelectedGameObject(resumeButton);
            else EventSystem.current.SetSelectedGameObject(retryButton);
        }

        if (Input.GetButtonDown("Interact") && inHamper) resumeGame();
    }

    public void updateCollectables(int index, Sprite sock)
    {
        collectableImage[index].GetComponent<Image>().sprite = sock;
    }

    public void updateTimerText(int time)
    {
        timeAmt = time;
        string minute = (time/60).ToString();
        string second = (time%60).ToString();
        if (second.Length <= 1) second = "0" + second;
        timerText.GetComponent<TMPro.TextMeshProUGUI>().text = minute + ":" + second;
        if (time < 0) playerLose(); 
    }

    public void updateTimerSprite(int time, float totalTime)
    {
        totalTimeAmt = totalTime;
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

    private void loadCutscene()
    {
        GameManager.gameState = GameManager.GameState.InCutscene;
        SceneManager.LoadScene("Cutscene");
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
        speedometer.SetActive(false);
        timer.SetActive(false);
        rankSprite[calculateScore()].SetActive(true);
        levelComplete.SetActive(true);
        string minute = (timeAmt / 60).ToString();
        string second = (timeAmt % 60).ToString();
        if (second.Length <= 1) second = "0" + second;
        timeRemainText.GetComponent<TextMeshProUGUI>().text = minute + ":" + second;
        levelCompleteTimer.GetComponent<Image>().fillAmount = timeAmt / totalTimeAmt;
        collectable.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        collectable.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        collectable.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
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
            case Load.Cutscene:
                loadCutscene();
                break;
        }
        //if (menu) loadMenu();
        //else loadHub();
    }

    public void loadSceneFromButton(int i)
    {
        if (GameManager.gameProg < 2 && i == 0) i = 2;
        if (i == 0) loadScene(Load.Hub);
        if (i == 1) loadScene(Load.Level);
        if (i == 2) loadScene(Load.Title);
    }

    void timerOver()
    {
        canContinue = true;
    }

    public void alertTimerOver()
    {
        alertUp = false;
    }

    public void updatePopupText(string s)
    {
        popupText.GetComponent<TMPro.TextMeshProUGUI>().text = s;
    }

    public void enablePopupText()
    {
        popText = true;
        popupText.SetActive(true);
        popupText.GetComponent<TextMeshProUGUI>().CrossFadeAlpha(1.0f, 0.1f, false);
    }

    public void disablePopupText()
    {
        popText = false;
        popupText.GetComponent<TextMeshProUGUI>().CrossFadeAlpha(0.0f, 0.1f, false);
        if (Time.timeScale == 0) popupText.SetActive(false);
    }

    public void updateAlertText(string s)
    {
        alertText.GetComponent<TMPro.TextMeshProUGUI>().text = s;
    }

    public void enableAlert()
    {
        alert.SetActive(true);
    }

    public void disableAlert()
    {
        alert.GetComponent<Image>().CrossFadeAlpha(0.0f, 0.1f, false);
        alertText.GetComponent<TextMeshProUGUI>().CrossFadeAlpha(0.0f, 0.1f, false);
    }

    public void pauseGame()
    {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        timer.SetActive(false);
        collectable.SetActive(false);
        speedometer.SetActive(false);
        if(popText) popupText.SetActive(false);
        if(alertUp) alert.SetActive(false);
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
        EventSystem.current.SetSelectedGameObject(retryButton);
    }

    public void resumeGame()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        trickInfo.SetActive(false);
        sockScreen.SetActive(false);
        if(GameManager.gameState == GameManager.GameState.InLevel)
        {
            timer.SetActive(true);
            collectable.SetActive(true);
            speedometer.SetActive(true);
        }
        if (inHamper) enablePopupText();
        if (popText) popupText.SetActive(true);
        if(alertUp) alert.SetActive(true); 
        paused = false;
        inHamper = false;
        trickUp = false;
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
        trickUp = true;
    }

    private int calculateScore()
    {
        int rank = 0;
        for(int i =0; i<3; i++)
        {
            if (levelManager.sockCollected(i)) rank++;
        }
        if (rank != 3) rankFlavourText.GetComponent<TextMeshProUGUI>().text = "Collect More Socks";
        if (timeAmt >= 60)
        {
            rank++;
        } 
        else
        {
            if (!rankFlavourText.GetComponent<TextMeshProUGUI>().text.Equals("")) rankFlavourText.GetComponent<TextMeshProUGUI>().text += " and ";
            rankFlavourText.GetComponent<TextMeshProUGUI>().text += "Go Faster";
        }
        if(rank!=4) rankFlavourText.GetComponent<TextMeshProUGUI>().text += " for a Higher Rank";
        if(rank == 4) rankFlavourText.GetComponent<TextMeshProUGUI>().text = "Great Job!";
        GameManager.rank[levelManager.getIndex()] = rank;
        return rank;
    }

    public void enableHamper()
    {
        sockScreen.SetActive(true);
        paused = true;
        inHamper = true;
        Time.timeScale = 0;
    }

    public void checkSockCollected()
    {
        for(int i=0; i<3; i++)
        {
            for(int j=0; j<3; j++)
            {
                if (GameManager.socks[i, j]) hamperSocks[i].transform.GetChild(j).GetComponent<Image>().sprite = sockCollected;
                else hamperSocks[i].transform.GetChild(j).GetComponent<Image>().sprite = sockNotCollected;
            }
        }
    }
}
