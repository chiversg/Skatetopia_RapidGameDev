using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectable : MonoBehaviour
{
    public enum index{First, Second, Third}
    [Tooltip("Which position is the collectable in the level, first, second, or third?")]
    public index Index;
    private int idx;
    [Tooltip("Sprite that will be filled into the ui on collection")]
    public Sprite sock;

    private GameObject collectableUI;
    private GameObject collectableImage;

    // Start is called before the first frame update
    void Start()
    {
        if(Index == index.First) idx = 0;
        else if(Index == index.Second) idx = 1;
        else if(Index == index.Third) idx = 2;
        collectableUI = GameObject.FindGameObjectWithTag("Collectable UI");
        collectableImage = collectableUI.transform.GetChild(idx).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
            collectableImage.GetComponent<Image>().sprite = sock;
            Destroy(this.gameObject);
        }
    }
}
