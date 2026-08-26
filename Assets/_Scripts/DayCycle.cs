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
        if(LoseCondition.instance.lost) return;

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
            VirusManager.instance.CheckInfect();
        }
        if(hours >= 24)
        {
            hours = 0;
        }

        UpdateClock(false);
    }

    public void UpdateClock(bool forceUpdate)
    {
        if(minutes % 10 != 0 && !forceUpdate) return;

        int minuteWithoutUnits = (minutes / 10) * 10;

        string minuteString = minuteWithoutUnits >= 10 ? minuteWithoutUnits.ToString() : "0" + minuteWithoutUnits.ToString();
        string hoursString = hours >= 10 ? hours.ToString() : "0" + hours.ToString();

        clockVisual.text = hoursString + ":" + minuteString;
    }
}
