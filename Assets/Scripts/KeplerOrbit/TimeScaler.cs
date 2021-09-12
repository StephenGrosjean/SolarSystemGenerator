using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
public class TimeScaler : MonoBehaviour
{
    public static TimeScaler instance;
    public float multiplicator = 1;
    public float SimulationTime;

    [Serializable]
    public class TimeScaleName
    {
        public string name;
        public float value;
    }

    [SerializeField] private List<TimeScaleName> times = new List<TimeScaleName>();
    private int timeIndex;


    private void FixedUpdate() {
        SimulationTime += Time.fixedDeltaTime * multiplicator;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.KeypadPlus)) {
            IncreaseTime();
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus)) {
            DecreaseTime();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            StopTime();
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt)) {
            ResetTime();
        }

    }
    private void Awake()
    {
        instance = this;
        UpdateTimeText();

    }
    public TextMeshProUGUI timeText;
    public void IncreaseTime()
    {
        if(timeIndex+1 < times.Count) {
            timeIndex++;
        }

        UpdateTimeText();
    }

    public void DecreaseTime()
    {
        if (timeIndex - 1 >= 0) {
            timeIndex--;
        }
        UpdateTimeText();
    }

    public void ResetTime()
    {
        timeIndex = 1;
        UpdateTimeText();
    }

    public void StopTime()
    {
        timeIndex = 0;
        UpdateTimeText();
    }

    void UpdateTimeText()
    {
        multiplicator = times[timeIndex].value;
        timeText.text = times[timeIndex].name.ToString();
    }


}
