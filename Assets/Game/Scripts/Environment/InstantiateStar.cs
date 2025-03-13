using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateStar : MonoBehaviour
{
    void Start(){
        
    }

    public void makeStar(Vector3 pos){
        Debug.Log("HEHEHEBWEHEHSGSZGFBDXETRH");
        var star = Resources.Load<GameObject>("Sock_Get");
        GameObject starPuff = Instantiate(star, pos, Quaternion.identity);
        Destroy(starPuff, 2.0f);
    }
}