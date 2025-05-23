using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTrigger : MonoBehaviour
{
    public enum Level { Tutorial, LevelOne, LevelTwo, Hub }
    public enum TriggerType { HubDoor, LevelExit, MidwayPoint }
    [Tooltip("What type of load trigger is this?")]
    public TriggerType triggerType;
    [Tooltip("If set to true will load sams test scene")]
    public bool debug;

    [Header("Hub Level Modifiers")]
    [Tooltip("Level that this door will load into")]
    public Level level;
    [Tooltip("Input player must press to trigger level load")]
    public KeyCode enter;

    [Header("Level Midpoint Variables")]
    [Tooltip("First half of level terrain")]
    public GameObject firstHalf;
    [Tooltip("Second half of level terrain")]
    public GameObject secondHalf;

    public bool playerInTrigger;
    private string levelName;
    private string officialLevelName;
    private bool off;

    //private GameObject sockUI;
    private GameManager gameManager;
    private UIManager uiManager;
    private LevelManager levelManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        uiManager = FindObjectOfType<UIManager>();
        levelManager = FindObjectOfType<LevelManager>();
        if (gameManager == null) Debug.LogError("No Game Manager in scene");
        if (!gameManager.enabled) Debug.LogError("Game Manager is disabled");
        if (uiManager == null) Debug.LogError("UI Manager is not applied to Player_UI");
        if (!uiManager.enabled) Debug.LogError("UI Manager disabled");
        if (triggerType != TriggerType.HubDoor && levelManager == null) Debug.LogError("No LevelManager in Scene");
        if (triggerType != TriggerType.HubDoor && !levelManager.enabled) Debug.LogError("LevelManager Disabled");
        if (triggerType == TriggerType.MidwayPoint)
        {
            if (firstHalf == null) Debug.LogError("First half of level terrain not given to midpoint load trigger");
            if (secondHalf == null) Debug.LogError("Second half of level terrain not given to midpoint load trigger");
            firstHalf.SetActive(true);
            secondHalf.SetActive(false);
        }
        //sockUI = GameObject.FindGameObjectWithTag("Collectable UI");
        updateDoors();
    }

    private void OnDisable()
    {
        //Debug.LogError("DSIFSDFADF");
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("TESTING TESTING 12345");
        if (playerInTrigger)
        {
            if (triggerType == TriggerType.LevelExit)
            {
                levelManager.recordSocks();
                levelManager.updateGameProg();
                uiManager.levelWinScreen(levelManager.calculateScore());
                playerInTrigger = false;
            }
            else if (triggerType == TriggerType.HubDoor)
            {
                if (Input.GetButtonDown("Interact") && Time.timeScale == 1)
                {
                    loadScene();
                    playerInTrigger = false;
                }
            }
            else if (triggerType == TriggerType.MidwayPoint)
            {
                firstHalf.SetActive(false);
                secondHalf.SetActive(true);
                uiManager.updateAlertText("Go Back Home");
                uiManager.enableAlert();
                Timer t = secondHalf.gameObject.AddComponent<Timer>();
                t.TimerEnded.AddListener(timerOver);
                t.setTimer(2.0f);
                t.startTimer();
                playerInTrigger = false;
                //off = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInTrigger = true;
            if (triggerType == TriggerType.HubDoor && this.enabled)
            {
                uiManager.updatePopupText("Enter " + levelName);
                uiManager.enablePopupText();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInTrigger = false;
            if (triggerType == TriggerType.HubDoor && this.enabled) uiManager.disablePopupText();
        }
    }

    private void loadScene()
    {
        Debug.Log("SCENE LOAD TEST");
        //gameManager.gameState = GameManager.GameState.InLevel;
        SceneManager.LoadScene(officialLevelName);
    }

    public void updateDoors()
    {
        if (triggerType == TriggerType.HubDoor)
        {
            if (level == Level.Tutorial)
            {
                levelName = "Level One: Bedroom";
                officialLevelName = "00_Tutorial";
                if (gameManager.getGameProg() < 2) this.enabled = false;
                else this.enabled = true;
            }
            else if (level == Level.LevelOne)
            {
                levelName = "Level Two: Street";
                officialLevelName = "02_Street";
                if (gameManager.getGameProg() < 4) this.enabled = false;
                else this.enabled = true;
            }
            else if (level == Level.LevelTwo)
            {
                levelName = "Level Three: Garden";
                officialLevelName = "03_Garden";
                if (gameManager.getGameProg() < 6) this.enabled = false;
                else this.enabled = true;
            }
        }
        else if (level == Level.Hub) officialLevelName = "01_Hub";
    }
    private void timerOver()
    {
        uiManager.disableAlert();
        gameObject.SetActive(false);
    }
}
