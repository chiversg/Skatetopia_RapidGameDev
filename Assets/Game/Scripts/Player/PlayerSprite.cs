using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    [SerializeField] private SkateboardMovementRigid player;
    [SerializeField] private Transform pivot;
    private Vector3 offset;
    private Quaternion targetRotation;
    private Vector3 currentRotation;
    private float groundRotateSpeed = 20f;
    private float airRotateSpeed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        offset = pivot.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        targetRotation = Quaternion.FromToRotation(Vector3.up, player.spriteRotation);
        pivot.position = player.transform.position + offset;
        float deltaAngle = Mathf.Abs(Mathf.DeltaAngle(pivot.transform.rotation.eulerAngles.z, targetRotation.eulerAngles.z));
        if (player.CloseToGround())
        {
            pivot.transform.rotation = Quaternion.RotateTowards(pivot.transform.rotation, targetRotation, groundRotateSpeed * deltaAngle * Time.deltaTime);
        }
        else
        {
            pivot.transform.rotation = Quaternion.RotateTowards(pivot.transform.rotation, targetRotation, airRotateSpeed * deltaAngle * Time.deltaTime);
        }
    }
}
