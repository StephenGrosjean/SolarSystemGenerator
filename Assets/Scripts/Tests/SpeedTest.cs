using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTest : MonoBehaviour
{

    public int PlanetCount;
    private int currentPlanetDone;
    float startTime;
    public SolarSystemGenerator generator;
    public static SpeedTest instance;

    private void Awake() {
        instance = this;
        //PlanetCount = generator.telluricPlanetCount;
    }
    public void Done() {
        currentPlanetDone++;
        if(currentPlanetDone == PlanetCount) {
            StopTimer();
        }
    }

    public void StartTimer() {
         startTime = Time.realtimeSinceStartup;
    }

    void StopTimer() {
        Debug.Log(PlanetCount + " Planets Done in " + ((Time.realtimeSinceStartup - startTime) * 1000f) + "ms");
    }
}
