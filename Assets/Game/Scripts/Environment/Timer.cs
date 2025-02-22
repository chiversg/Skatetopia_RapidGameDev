using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public UnityEvent TimerEnded;
    private float timerLength;
    private bool timerStart;

    void Awake(){
        if(TimerEnded==null) TimerEnded = new UnityEvent();
    }

    void Update(){
        if(timerStart){
            timerLength-=Time.unscaledDeltaTime;
            if(timerLength<=0){
                endTimer();
            }
        }
    }

    public void setTimer(float length){
        timerLength = length;
    }

    public void startTimer(){
        timerStart = true;
    }

    private void endTimer(){
        Debug.Log("Timer has Ended");
        TimerEnded?.Invoke();
        Destroy(this);
    }
}
