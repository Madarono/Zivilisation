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
        choosing++;
        if(choosing == timeStates.Length)
        {
            choosing = 0;
        }

        UpdateTimeScale();
    }

    public void UpdateTimeScale()
    {
        timeIcon.sprite = timeStates[choosing].icon;
        Time.timeScale = timeStates[choosing].timeSpeed;
    }
}
