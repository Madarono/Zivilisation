using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class MinesMaterial
{
    public Sprite materialIcon;
    public int timeToGather;
    public Sprite[] materialStages;
    public int storageID;
}

public class Mines : Building, VillageBuildable
{
    private TownStorage townStorage;

    [Header("Mines Specific")]
    public bool isWorkedOn; //When villager is working
    public Sprite[] workedOnStates;
    public int jobPlaceID = 2;

    public Collider2D matSelectionCol;
    public SpriteRenderer matSelectionIcon;
    public SpriteRenderer matGatherIcon;
    public MinesMaterial[] materials;
    public int matSelectionId = 0;

    private Coroutine activeMines;

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

    protected override void Update()
    {
        if ((Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) && !ViewMode.instance.viewMode)
        {
            if (CameraPinch.Instance != null && CameraPinch.Instance.IsPanning) 
            {
                return;
            }

            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (col == Physics2D.OverlapPoint(clickPos) && !isChoosing && !TownManager.instance.isBuilding && TownManager.instance.activeBuilding == null)
            {
                OnSpriteClicked();
            }
            else if(addCol == Physics2D.OverlapPoint(clickPos) && villagers.Count < humanIcons.Length && !isChoosing)
            {
                AddHuman();
            }
            else if(deleteCol == Physics2D.OverlapPoint(clickPos) && humanIcons.Length > 0 && !isChoosing)
            {
                RemoveHuman();
            }
            else if(matSelectionCol == Physics2D.OverlapPoint(clickPos))
            {
                ChangeMaterial();
            }
            // else if(isShowing && !isChoosing && (TownManager.instance.activeBuilding != this || TownManager.instance.activeBuilding == null))
            // {
            //     HideVisuals();
            // }
        }

        if(CameraPinch.Instance.IsPanning && isShowing && !isChoosing && (TownManager.instance.activeBuilding != this || TownManager.instance.activeBuilding == null))
        {
            HideVisuals();
        }
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
        
        matSelectionIcon.sprite = materials[matSelectionId].materialIcon;
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

    public void ChangeMaterial()
    {
        if(isWorkedOn)
        {
            PopupText.instance.Popup("Can't change metal during work hours.");
            return;
        }

        matSelectionId++;
        if(matSelectionId >= materials.Length)
        {
            matSelectionId = 0;
        }

        UpdateVisuals();
    }

    public void StartMining()
    {
        isWorkedOn = true;
        UpdateVisuals();

        if(activeMines != null)
        {
            StopCoroutine(activeMines);
        }

        activeMines = StartCoroutine(GatherMaterial());
    }

    public void StopMining() //Controlled by TownManager.cs, should remind all villagers
    {
        RemindVillagerStop();
        isWorkedOn = false;
        matGatherIcon.gameObject.SetActive(false);
        UpdateVisuals();

        if(activeMines != null)
        {
            StopCoroutine(activeMines);
        }
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

    IEnumerator GatherMaterial()
    {
        matGatherIcon.gameObject.SetActive(true);
        float gatherTime = materials[matSelectionId].timeToGather;
        List<float> spriteUpdateTime = new List<float>();
        float differenceTime = gatherTime / (float)materials[matSelectionId].materialStages.Length; //To add each spriteUpdateTime
        float accumulatedTime = 0; //To set each spriteUpdateTime
        int currentSpriteUpdateTime = 0; //Id for each spriteUpdateTime

        for(int i = 0; i < materials[matSelectionId].materialStages.Length; i++)
        {
            accumulatedTime += differenceTime;
            spriteUpdateTime.Add(accumulatedTime);
        }

        float t = 0;
        matGatherIcon.sprite = materials[matSelectionId].materialStages[0];

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
                    TownStorage.instance.AddToInventoryID(materials[matSelectionId].storageID, 1f);
                    matGatherIcon.sprite = materials[matSelectionId].materialStages[spriteUpdateTime.Count - 1];
                    yield return null;
                }

                matGatherIcon.sprite = materials[matSelectionId].materialStages[currentSpriteUpdateTime];
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
