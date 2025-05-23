using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectable : MonoBehaviour
{
    public enum index { First, Second, Third }
    [Tooltip("Which position is the collectable in the level, first, second, or third?")]
    public index Index;
    private int idx;
    [Tooltip("Sprite that will be filled into the ui on collection")]
    public Sprite sock;
    [Tooltip("Collectable Get particle effect")]
    public ParticleSystem sock_get;
    [Tooltip("Sock gameobject")]
    public GameObject sockObj;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sockCollectAudio;

    private UIManager uiManager;
    private LevelManager levelManager;
    //private GameObject collectableUI;
    //private GameObject collectableImage;
    private bool collected;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        levelManager = FindObjectOfType<LevelManager>();
        if(uiManager==null) Debug.LogError("UI Manager is not applied to Player_UI");
        if(!uiManager.enabled) Debug.LogError("UI Manager disabled");
        if(levelManager==null) Debug.LogError("No LevelManager in Scene");
        if(!levelManager.enabled) Debug.LogError("LevelManager Disabled");
        if(sock_get==null) Debug.LogError("No particle system attached");
        if(Index == index.First) idx = 0;
        else if(Index == index.Second) idx = 1;
        else if(Index == index.Third) idx = 2;
        //collectableUI = GameObject.FindGameObjectWithTag("Collectable UI");
        //collectableImage = collectableUI.transform.GetChild(idx).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
            updateUI();
            levelManager.collectSock(idx);
            audioSource.PlayOneShot(sockCollectAudio);
            sock_get.Play();
            sockObj.SetActive(false);
            Destroy(this.gameObject, 2.0f);
        }
    }

    public void updateUI(){
        //Debug.Log("TESTING COLLECTABLE THINGY V2");
        uiManager.updateCollectables(idx);
    }

    public int getIdx(){
        return idx;
    }

    public void setCollected(bool b){
        try{
            updateUI();
        }
        catch(System.Exception ex){
            collected = b;
        }
    }
}
