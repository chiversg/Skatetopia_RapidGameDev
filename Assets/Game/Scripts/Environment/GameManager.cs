using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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

    //public static UnityEvent ollieGet;
    //public static UnityEvent uturnGet;
    //public static UnityEvent flipGet;

    public enum GameState
    {
        StartMenu,
        InCutscene,
        InLevel,
        InHub
    }

    public static GameState gameState;

    public enum InputMode 
    { 
        Controller, 
        Keyboard 
    }
    
    public static InputMode currentInput = InputMode.Keyboard;

    [Header("Debug")]
    [Tooltip("gives all tricks")]
    [SerializeField]
    public bool debug;
    public int prog;
    public GameState state;

    public static int gameProg;

    public static bool[,] socks = new bool[3, 3];
    public static int[] rank = {-1,-1,-1};

    public static bool ollie;
    public static bool flip;
    public static bool uturn;

    private List<ResetBehaviour> registeredForReset;

    private int levelBeat;

    private static bool firstLoad = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        registeredForReset = new List<ResetBehaviour>();
        Debug.Log(gameProg);

        if (debug && firstLoad) setDebug();
        //currentInput = InputMode.Keyboard;
    }

    void Update()
    {
        currentInput = processInputMode();
        if (Input.GetKeyDown(KeyCode.P))
        {
            gameProg = 0;
            gameState = GameState.InHub;
            SceneManager.LoadScene("01_Hub");
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            FindObjectOfType<UIManager>().enableHamper();
        }
    }

    private InputMode processInputMode()
    {
        if (Input.GetJoystickNames().Length == 0) return InputMode.Keyboard;

        if(Input.GetKey(KeyCode.JoystickButton9)) return InputMode.Controller;
        if (Input.GetButton("ControllerButton")) return InputMode.Controller;
        if ((Mathf.Abs(Input.GetAxisRaw("ControllerX")) >= 0.01 || Mathf.Abs(Input.GetAxisRaw("ControllerY")) >= 0.01)) return InputMode.Controller;
        if ((Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)) return InputMode.Keyboard;
        if (Input.anyKey) return InputMode.Keyboard;

        return currentInput;
    }

    public static void RegisterForReset(ResetBehaviour resetBehaviour)
    {
        Instance.registeredForReset.Add(resetBehaviour);
    }

    public static void GameReset()
    {
        for (int i = 0; i < instance.registeredForReset.Count; i++)
        {
            Instance.registeredForReset[i].Reset();
        }
    }

    public void setGameState(int i)
    {
        if (levelBeat < i) levelBeat = i;
    }

    public void setCollectable(int i, int j)
    {
        //Debug.Log("Game Manager Set Collectable: " + j + " of Level: " + i + " to be True");
        socks[i, j] = true;
    }
    public bool getCollectableBool(int levIdx, int colIdx)
    {
        //Debug.Log("TEST IS THIS TRUE OR FALSE: " + socks[levIdx,colIdx]);
        return socks[levIdx, colIdx];
    }

    public int getGameProg()
    {
        return gameProg;
    }

    public void setGameProg(int i)
    {
        gameProg = i;
    }

    public void setDebug()
    {
        ollie = true;
        uturn = true;
        flip = true;
        gameProg = prog;
        gameState = state;
        firstLoad = false;
    }
}
