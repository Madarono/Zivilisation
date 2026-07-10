using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class DayCycle : MonoBehaviour
{
    public static DayCycle instance {get; private set;}
    public float dayInMinutes = 10;
    public float clockMultiplyer;
    public int hours;
    public int minutes;
    public float seconds;
    public TextMeshProUGUI clockVisual;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        clockMultiplyer = 1440f / dayInMinutes; //1440 Minutes in 1 Realtime Day
    }

    void Update()
    {
        seconds += clockMultiplyer * Time.deltaTime;
        if(seconds >= 60)
        {
            minutes++;
            seconds = 0;
        }
        if(minutes >= 60)
        {
            hours++;
            minutes = 0;
            TownManager.instance.CheckHour();
        }
        if(hours >= 24)
        {
            hours = 0;
        }

        UpdateClock();
    }

    public void UpdateClock()
    {
        string minuteString = minutes >= 10 ? minutes.ToString() : "0" + minutes.ToString();
        string hoursString = hours >= 10 ? hours.ToString() : "0" + hours.ToString();

        clockVisual.text = hoursString + ":" + minuteString;
    }
}
