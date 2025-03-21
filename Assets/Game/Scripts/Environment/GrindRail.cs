using UnityEngine;

public class GrindRail : MonoBehaviour
{

    public enum direction {Left,Right,Flat}
    [Tooltip("side of the rail that is elevated")]
    public direction Direction;
    private int dir;

    [Tooltip("End point of the rail")]
    public Transform end;
    public bool debug;
    
    private GameObject player;
    private SkateboardMovementRigid playerScript;
    // Start is called before the first frame update
    void Start()
    {
        if(debug) Debug.Log("X:" + end.position.x + " Y:" + end.position.y + " Z:" + end.position.z);
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<SkateboardMovementRigid>();
        if(Direction == direction.Left) dir = 1;
        else if(Direction == direction.Right) dir = -1;
        else if(Direction == direction.Flat) dir = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        var height = GetComponent<MeshFilter>().mesh.bounds.extents.z;
        float xSpeed = playerScript.getXSpeed();
        //Debug.Log("rail enter" + height);
        if (other.tag == "Player" && xSpeed*dir >= 0)
        {
            Debug.Log("player enter rail");
            playerScript.boardRail(end, transform);
        }
    }
}
