using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum FeedTime
{
    Anytime = 0,
    EndOfDay = 1
}

public enum FeedPer
{
    FullFeed = 0,
    BareMinimum = 1
}

public class HungerManager : MonoBehaviour
{
    public static HungerManager instance {get; private set;}
    private TownStorage townStorage;
    private TownManager townManager;

    public Sprite[] buttonStates;

    [Header("Feed Time")]
    public FeedTime feedState;
    public TextMeshProUGUI feedInfo;
    public int feedId;
    public Image[] feedOptions;
    public string[] feedString;

    [Header("Feed Percentage")]
    public FeedPer percentageState;
    public TextMeshProUGUI percentageInfo;
    public int percentageId;
    public Image[] percentageOptions;
    public string[] percentageString;

    [Header("Full Feed Requirements")]
    public float minFullFeed = 0.5f;
    public float maxFullFeed = 1f;

    [Header("Bare Minimum Feed Requirements")]
    public float minBareFeed = 0.3f;
    public float maxBareFeed = 0.6f;

    public float totalUsedWheat;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        townStorage = TownStorage.instance;
        townManager = TownManager.instance;
    }

    public void UpdateVisuals()
    {
        feedInfo.text = feedString[feedId];
        percentageInfo.text = percentageString[percentageId];

        feedState = (FeedTime)feedId;
        percentageState = (FeedPer)percentageId;

        for(int i = 0; i < 2; i++) //since it is two buttons for each, imma make it in one for loop
        {
            feedOptions[i].sprite = i == feedId ? buttonStates[1] : buttonStates[0];
            percentageOptions[i].sprite = i == percentageId ? buttonStates[1] : buttonStates[0];
        }
    }

    public void ChooseFeedTime(int id)
    {
        feedId = id;
        UpdateVisuals();
    }

    public void ChooseFeedPercentage(int id)
    {
        percentageId = id;
        UpdateVisuals();
    }
    
    public void EndOfDayFeed()
    {
        if(feedState != FeedTime.EndOfDay) return;

        totalUsedWheat = 0f;

        foreach(var villager in townManager.villagers)
        {
            FeedVillager(villager, false);
            if(townStorage.wheat <= 0)
            {
                PopupText.instance.Popup("Some Villagers were left hungry.");
                return;
            }
        }

        totalUsedWheat = Mathf.Floor(totalUsedWheat * 100f) / 100f;

        PopupText.instance.Popup($"All villagers are well fed for tonight. Used {totalUsedWheat} Wheat");
    }

    public void FeedVillager(VillagerAI villager, bool popup = true) //For anytime villager to activate on his own
    {
        float hunger = villager.hunger;

        float maxFeed = percentageState == FeedPer.FullFeed ? maxFullFeed : maxBareFeed;

        float difference = maxFeed - hunger;

        Debug.Log($"Villager {villager.name} hunger: {villager.hunger}, target: {maxFeed}");

        if(townStorage.wheat >= difference)
        {
            villager.hunger = maxFeed;
            townStorage.wheat -= difference;
            Debug.Log("Fed full" + villager.gameObject.name);
            totalUsedWheat += difference;
        }
        else if(townStorage.wheat > 0) //Partially feeds
        {
            villager.hunger += townStorage.wheat;
            totalUsedWheat += townStorage.wheat;
            townStorage.wheat = 0;
            Debug.Log("Fed partial" + villager.gameObject.name);
            if(popup) PopupText.instance.Popup("A villager satisfied partially his hunger.");
        }
        else
        {
            Debug.Log("Couldn't Feed" + villager.gameObject.name);
            if(popup) PopupText.instance.Popup("A villager can't satisfy his hunger.");
        }
    }
}
