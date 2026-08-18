using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TimeStates
{
    public float timeSpeed = 1f;
    public Sprite icon;
}
public class TimeForward : MonoBehaviour
{
    public static TimeForward instance {get; private set;}
    public Image timeIcon;
    public TimeStates[] timeStates;
    public int choosing;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        choosing = 1;
        UpdateTimeScale();
    }

    public void IncreaseChoosing()
    {
        if(LoseCondition.instance.lost) return;

        choosing++;
        if(choosing == timeStates.Length)
        {
            choosing = 0;
        }

        if(choosing == 0 && TownManager.instance.availableLaboratory != null && TownManager.instance.availableLaboratory.isShowing) //Avoid going to Timescale 0
        {
            choosing = 1;
        }

        UpdateTimeScale();
    }

    public void UpdateTimeScale()
    {
        if(LoseCondition.instance.lost) return;
        
        timeIcon.sprite = timeStates[choosing].icon;
        Time.timeScale = timeStates[choosing].timeSpeed;
    }
}
