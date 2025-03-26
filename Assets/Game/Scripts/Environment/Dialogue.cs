using System.Collections;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Lines of Dialogue")]
    private string[] lines;
    [SerializeField]
    [Tooltip("Reference to UI button indictator")]
    private TextMeshProUGUI toolTipText;
    [SerializeField]
    [Tooltip("Reference to dialogue box")]
    private TextMeshPro dialogueText;
    [SerializeField]
    [Tooltip("How fast the text scrolls by (in seconds)")]
    private float playbackSpeed = 0.1f;
    [SerializeField]
    [Tooltip("Required internal game progress for dialogue to be active. Use -1 to be always active")]
    private int requiredGameProgress;
    [SerializeField]
    [Tooltip("What to set internal game progress to after dialogue event is complete. Use -1 to never update game progress")]
    private int nextGameProgress;
    [SerializeField] private PlayerWalk walking;
    [SerializeField] private SkateboardMovementRigid skating;
    [SerializeField] private GameObject speechBubblePivot;
    [SerializeField] private LoadTrigger loadTrigger;

    private bool isListening;
    private bool playNext;
    private bool keyPressed;
    private bool donePrinting = true;
    private bool talkAgain;
    private int lineNum = 0;
    private int numberOfLines;
    private GameManager gameManager;
    private LevelManager levelManager;


    private void Start()
    {
        talkAgain = true;
        speechBubblePivot.SetActive(false);
        gameManager = FindObjectOfType<GameManager>();
        levelManager = FindObjectOfType<LevelManager>();
        numberOfLines = lines.Length;
    }
    void Update()
    {
        if (Input.GetButtonDown("Accept")) keyPressed = true;

        if (lineNum >= numberOfLines && (playNext && donePrinting))
        {
            isListening = false;
            playNext = false;
            lineNum = 0;
            if (nextGameProgress >= 0) GameManager.gameProg = nextGameProgress;
            LoadTrigger[] triggers = FindObjectsOfType(typeof(LoadTrigger)) as LoadTrigger[];
            foreach (LoadTrigger trigger in triggers)
            {
                trigger.updateDoors();
            }
            if (loadTrigger != null) loadTrigger.playerInTrigger = true;
            speechBubblePivot.SetActive(false);
            if (walking != null) walking.exitDialogue(); else skating.exitDialogue();
            if (levelManager != null) levelManager.unpauseGame();
            talkAgain = false;
        }

        if (playNext && donePrinting)
        {

            StartCoroutine(printLine(lines[lineNum]));

        }
    }

    IEnumerator printLine(string currLine)
    {
        playNext = false;
        donePrinting = false;
        dialogueText.text = null;
        char[] characters = currLine.ToCharArray();
        for (int i = 0; i < characters.Length; i++)
        {
            if (playNext)
            {
                playNext = false;
                dialogueText.text = currLine;
                break;
            }
            yield return new WaitForSeconds(playbackSpeed);
            dialogueText.text += characters[i];
        }
        donePrinting = true;
        lineNum++;
    }
    private void OnTriggerStay(Collider other)
    {
        if ((gameManager.getGameProg() == requiredGameProgress || gameManager.getGameProg() == nextGameProgress) || requiredGameProgress < 0)
        {
            Debug.Log("something entered");
            if (other.gameObject.tag == "Player")
            {
                Debug.Log("in the dialog zone");
                //toolTipText.text = "Press spacebar to listen";
                if (talkAgain)
                {


                    if (keyPressed)
                    {
                        keyPressed = false;
                        if (!isListening)
                        {
                            isListening = true;
                            playNext = true;
                            speechBubblePivot.SetActive(true);
                            if (walking != null) walking.enterDialogue(); else skating.enterDialogue();
                            if (levelManager != null) levelManager.pauseGame();
                        }
                        else
                        {
                            playNext = true;
                        }
                    }
                }
                else
                {
                    talkAgain = true;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            toolTipText.text = null;
        }
    }
}

