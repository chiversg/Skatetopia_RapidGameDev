using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateDust : MonoBehaviour
{
    void Start(){
        
    }

    public void makeDust(Vector3 pos){
        var dust = Resources.Load<GameObject>("Dust_Particle");
        GameObject dustPuff = Instantiate(dust, pos, Quaternion.identity);
        Destroy(dustPuff, 2.0f);
    }
}
