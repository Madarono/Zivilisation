using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Farm : Building, VillageBuildable
{
    private TownStorage townStorage;

    [Header("Farm Specific")]
    public bool isWorkedOn; //When villager is working
    public Sprite[] workedOnStates;
    public int jobPlaceID = 1;

    [Header("Farming")]
    public float wheatPerGain = 0.1f;
    public float cooldownDuration = 10f; //in a course of 8 hours ingame it can feed upto 5 villagers housed, and 3 villagers unhoused

    public SpriteRenderer matGatherIcon;
    public Sprite[] wheatStages;

    private Coroutine activeMines;
    private Coroutine activeFarm;

    protected override void Start()
    {
        matGatherIcon.gameObject.SetActive(false);
        townStorage = TownStorage.instance;
        if(!GridManager.instance.buildings.Contains(this))
        {
            GridManager.instance.buildings.Add(this);
        }
        HideVisuals();
    }

    public override void ShowVisuals()
    {
        buildingStats.gameObject.SetActive(true);
        isShowing = true;
        TownManager.instance.ShowSelectedHumans(this);
        UpdateVisuals();
    }

    public override void HideVisuals()
    {
        buildingStats.gameObject.SetActive(false);
        isShowing = false;
        TownManager.instance.HideSelectedHumans(this);
        UpdateVisuals();
    }

    protected override void AddHuman()
    {
        isChoosing = true;
        TownManager.instance.activeBuilding = this;
        TownManager.instance.SelectingHumanMode(this);
    }



    public override void UpdateVisuals()
    {
        addButton.SetActive(villagers.Count < humanIcons.Length);
        deleteButton.SetActive(villagers.Count > 0);
        rend.sprite = (isShowing, isWorkedOn) switch
        {
            (true, false) => buildingStates[1],
            (false, false) => buildingStates[0],
            (true, true) => workedOnStates[1],
            (false, true) =>  workedOnStates[0]
        };

        for(int i = 0; i < humanIcons.Length; i++)
        {
            if(i < villagers.Count && villagers[i] != null)
            {
                humanIcons[i].sprite = humanStates[1];
            }
            else
            {
                humanIcons[i].sprite = humanStates[0];
            }
        }
    }

    public void StartFarming()
    {
        if(activeFarm != null)
        {
            StopCoroutine(activeFarm);
        }

        isWorkedOn = true;
        UpdateVisuals();
        activeFarm = StartCoroutine(Farming());
    }

    public void StopFarming() //Controlled by TownManager.cs, should remind all villagers
    {
        if(activeFarm == null)
        {
            return;
        }
        RemindVillagerStop();
        isWorkedOn = false;
        UpdateVisuals();
        StopCoroutine(activeFarm);
    }

    public void RemindVillagerStop() //When deletion or finishing the Job
    {
        if(!isWorkedOn)
        {
            return;
        }

        foreach(var villager in villagers)
        {
            if(villager.villagerPF.isMoving && villager.state == VillagerState.Working)
            {
                villager.villagerPF.CancelMovement();
            }  
            villager.state = VillagerState.Idle;
        }
    }

    IEnumerator Farming()
    {
        matGatherIcon.gameObject.SetActive(true);
        float gatherTime = cooldownDuration;
        List<float> spriteUpdateTime = new List<float>();
        float differenceTime = gatherTime / (float)wheatStages.Length; //To add each spriteUpdateTime
        float accumulatedTime = 0; //To set each spriteUpdateTime
        int currentSpriteUpdateTime = 0; //Id for each spriteUpdateTime

        for(int i = 0; i < wheatStages.Length; i++)
        {
            accumulatedTime += differenceTime;
            spriteUpdateTime.Add(accumulatedTime);
        }

        float t = 0;
        matGatherIcon.sprite = wheatStages[0];

        while(true)
        {
            t += Time.deltaTime;
            if(t >= (spriteUpdateTime[currentSpriteUpdateTime] + differenceTime) && currentSpriteUpdateTime < spriteUpdateTime.Count)
            {
                currentSpriteUpdateTime++;
                
                if(currentSpriteUpdateTime >= spriteUpdateTime.Count)
                {
                    currentSpriteUpdateTime = 0;
                    t = 0;
                    TownStorage.instance.AddToInventoryID(0, wheatPerGain);
                    matGatherIcon.sprite = wheatStages[spriteUpdateTime.Count - 1];
                    yield return null;
                }

                matGatherIcon.sprite = wheatStages[currentSpriteUpdateTime];
            }
            yield return null;
        }
    }

    public override void AssignVillagerRole(VillagerAI villager)
    {
        villager.jobPlace = this.transform;
        villager.jobPlaceID = jobPlaceID;
        villager.villagerSprite.UpdateLooks();
    }

    public override void RemoveVillagerRole(VillagerAI villager)
    {
        villager.jobPlace = null;
        villager.jobPlaceID = 0;
        villager.villagerSprite.UpdateLooks();
    }
}
