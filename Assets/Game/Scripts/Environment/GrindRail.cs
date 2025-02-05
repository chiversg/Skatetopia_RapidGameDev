using UnityEngine;

public class GrindRail : MonoBehaviour
{
    public Transform end;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("rail enter");
        if (other.tag == "Player")
        {
            Debug.Log("player enter rail");
            player.GetComponent<SkateboardMovement>().boardRail(end);
        }
    }
}
