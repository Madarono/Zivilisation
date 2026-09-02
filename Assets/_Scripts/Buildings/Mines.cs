using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class MinesMaterial
{
    public Sprite materialIcon;
    public string materialName;
    public float amountPerIngameHour;
    public Sprite[] materialStages;
    public int storageID;
}

public class Mines : Building, VillageBuildable
{
    private TownStorage townStorage;

    [Header("Mines Specific")]
    public VillagerAI currentVillager;
    public bool isWorkedOn;
    public Sprite[] workedOnStates;
    public int jobPlaceID = 2;

    public Collider2D matSelectionCol;
    public SpriteRenderer matSelectionIcon;
    public SpriteRenderer matGatherIcon;
    public MinesMaterial[] materials;
    public Vector3 popupOffset = new Vector3(0, 1.5f, 0);
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
        HideVisuals(false);
    }

    protected override void Update()
    {
        if(LoseCondition.instance.lost) return;
        if(Settings.instance.isOpen || ActiveWindow.instance.isActive || ActiveWindow.instance.briefActive) return;

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
        AudioManager.instance.Play(AudioManager.instance.select);
    }

    public override void HideVisuals(bool withSound = true)
    {
        buildingStats.gameObject.SetActive(false);
        isShowing = false;
        TownManager.instance.HideSelectedHumans(this);
        if(withSound) AudioManager.instance.Play(AudioManager.instance.buttonClicks[1]);
        PopupText.instance.StopMiniPopup();
        UpdateVisuals();
    }

    protected override void AddHuman()
    {
        isChoosing = true;
        TownManager.instance.activeBuilding = this;
        TownManager.instance.SelectingHumanMode(this);
    }

    public override void RemoveHuman()
    {
        TownManager.instance.RemoveHumanManually(villagers[0], this);
        StopMining();
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

        AudioManager.instance.Play(AudioManager.instance.buttonClicks[0]);
        PopupText.instance.MiniPopup(materials[matSelectionId].materialName, transform, popupOffset);
        UpdateVisuals();
    }

    public void StartMining()
    {
        if(isWorkedOn) return;

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
            villager.hasWarnedInsomnia = false;
        }
    }

    IEnumerator GatherMaterial()
    {
        DayCycle cycle = DayCycle.instance;

        float speed = (currentVillager != null && currentVillager.villagerHealth.functionSpeed > 0) ? currentVillager.villagerHealth.functionSpeed : 1f;
        

        int ticksNeeded = Mathf.Max(1, Mathf.RoundToInt((60f / materials[matSelectionId].amountPerIngameHour) / speed));

        List<int> ticksPerSprite = new List<int>();
        int dividedTicks = Mathf.FloorToInt((float)ticksNeeded / materials[matSelectionId].materialStages.Length);

        for(int i = 0; i < materials[matSelectionId].materialStages.Length; i++)
        {
            ticksPerSprite.Add(Mathf.RoundToInt((i + 1) * dividedTicks));
            Debug.Log(ticksPerSprite[i]);
        }

        int startingMinute = (cycle.hours * 60) + cycle.minutes;
        int deltaMinute = (cycle.hours * 60) + cycle.minutes;
        
        int startingHour = cycle.hours;
        int deltaHour = cycle.hours;

        matGatherIcon.gameObject.SetActive(true);
        
        int stageCount = materials[matSelectionId].materialStages.Length;
        if (stageCount == 0) yield break;

        while (true)
        {
            deltaHour = cycle.hours;
            deltaMinute = (deltaHour * 60) + cycle.minutes;

            int difference = deltaMinute - startingMinute;

            if (currentVillager != null && currentVillager.isCoughing)
            {
                startingMinute = deltaMinute - difference;
                yield return null;
                continue;
            }

            while (difference >= ticksNeeded)
            {
                TownStorage.instance.AddToInventoryID(materials[matSelectionId].storageID, 1f);
                Debug.Log("Gave 1");

                startingMinute += ticksNeeded; 
                difference -= ticksNeeded;
            }


            int currentStageIndex = GetStageIndex(ticksPerSprite, difference);
            matGatherIcon.sprite = materials[matSelectionId].materialStages[currentStageIndex];

            yield return null;
        }
    }

    
    public override void AssignVillagerRole(VillagerAI villager, bool withSound = true)
    {
        currentVillager = villager;
        villager.jobPlace = this.transform;
        villager.jobPlaceID = jobPlaceID;
        villager.villagerSprite.UpdateLooks();
        if(withSound) AudioManager.instance.Play(AudioManager.instance.villagerAssign);
    }

    public override void RemoveVillagerRole(VillagerAI villager, bool withSound = true)
    {
        currentVillager = null;
        villager.jobPlace = null;
        villager.jobPlaceID = 0;
        villager.villagerSprite.UpdateLooks();
        if(withSound) AudioManager.instance.Play(AudioManager.instance.villagerRevoke);
    }

    int GetStageIndex(List<int> ticksPerSprite, int difference)
    {
        for (int i = 0; i < ticksPerSprite.Count; i++)
        {
            if (difference < ticksPerSprite[i]) return i;
        }

        return ticksPerSprite.Count - 1;
    }
}
