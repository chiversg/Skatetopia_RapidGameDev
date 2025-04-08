using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    [SerializeField] private SkateboardMovementRigid player;
    [SerializeField] private Transform pivot;
    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = pivot.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        pivot.position = player.transform.position + offset;
        pivot.rotation = player.transform.rotation;
    }
}
