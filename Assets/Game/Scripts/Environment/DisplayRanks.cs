using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LoadTrigger;

public class DisplayRanks : MonoBehaviour
{
    [Tooltip("rank sprites going from lowest to highest")]
    public Sprite[] ranks;
    [Tooltip("index of level this is entering, tutorial = 0, street = 1, garden = 2")]
    public int index;
    [Tooltip("Gameobject for rank")]
    public GameObject rank;

    private bool inTrigger;
    private float rate = 0.015f;

    // Start is called before the first frame update
    void Start()
    {
        rank.GetComponent<SpriteRenderer>().color = new Color(rank.GetComponent<SpriteRenderer>().color.r, rank.GetComponent<SpriteRenderer>().color.g, rank.GetComponent<SpriteRenderer>().color.b, 0f);
        if (GameManager.rank[index] == -1)
        {
            rank.SetActive(false);
        }
        else
        {
            rank.GetComponent<SpriteRenderer>().sprite = ranks[GameManager.rank[index]];
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpriteRenderer sr = rank.GetComponent<SpriteRenderer>();
        if(inTrigger && sr.color.a < 1.0f) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a+rate);
        else if(sr.color.a > 0.0f) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - rate);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inTrigger = false;
        }
    }
}
