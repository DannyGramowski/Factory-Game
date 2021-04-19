using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
    private float currTime;
    private float maxTime;
    private bool looping;

    private void Update() {
        if(!TimerDone()) {
            currTime -= Time.deltaTime;
        }else if(looping) {
            StartTimer();
        }
    }

    public void StartUp(float maxTime, bool looping) {
        this.maxTime = maxTime;
        this.looping = looping;
    }

    public void StartUp(float maxTime) {
        StartUp(maxTime, false);
    }

    public void StartTimer() {
        currTime = maxTime;
    }

    public void StartTimer(float newTime) {
        maxTime = newTime;
        StartTimer();
    }

    public float GetTime() {
        return currTime;
    }

    public bool TimerDone() {
        return currTime <= 0;
    }
}
