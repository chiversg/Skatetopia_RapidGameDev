using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTrigger : MonoBehaviour
{
    [Tooltip("Is the scene load the level exit, if not it'll be a hub entrance")]
    public bool levelExit;

    [Header("Hub Level Modifiers")]
    [Tooltip("Name of the scene to load, if level exit it will be automatically set to the hub")]
    public string sceneName;
    [Tooltip("Input player must press to trigger level load")]
    public KeyCode enter;   

    private bool playerInTrigger;

    void Start()
    {
        if(levelExit) sceneName = "Hub";
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInTrigger && (Input.GetKey(enter) || levelExit)){
            SceneManager.LoadScene(sceneName);
        }
    }

    private void OnTriggerEnter(Collider other){
        if(other.tag == "Player") playerInTrigger = true;
    }

    private void OnTriggerExit(Collider other){
        if(other.tag == "Player") playerInTrigger = false;
    }
}
