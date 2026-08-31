using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class TownManager : MonoBehaviour
{
    public static TownManager instance {get; private set;}

    [Header("Selecting Human House")]
    public Building activeBuilding;
    public GameObject denyButton;

    [Header("Building")]
    public bool isBuilding;

    [Header("For Market")]
    public GameObject marketWindow;
    public Market availableMarket;
    public Slider[] marketSliders;
    public TextMeshProUGUI[] marketVisuals;
    public TextMeshProUGUI[] marketSellVisuals;
    public TextMeshProUGUI marketSellValue;
    public Image[] marketArrows;

    [Header("For Quarantine")]
    public Quarantine availableQuarantine;

    [Header("For Laboratory")]
    public Laboratory availableLaboratory;
    public GameObject laboratoryWindow;

    [Header("Village Hunger")]
    public int totalDead;
    public List<VillagerAI> villagers = new List<VillagerAI>();
    public List<VillagerDead> deadVillagers = new List<VillagerDead>();
    float wheatNeeded;

    [Header("Other Stats")]
    public int housedPopulation;
    public int workingPopulation;
    public int houses;
    public int farms;
    public int mines;
    public TextMeshProUGUI moralityVisual;
    public TextMeshProUGUI populationVisual;
    public TextMeshProUGUI housedPopulationVisual;
    public TextMeshProUGUI workingPopulationVisual;
    public TextMeshProUGUI housesVisual;
    public TextMeshProUGUI farmsVisual;
    public TextMeshProUGUI minesVisual;

    [Header("Villager Check")]
    public LayerMask villagerLayer;

    [Header("Village Sleep Times")]
    public int hourSleepReq = 22;
    public int hourAwakeReq = 6;

    [Header("Villager Work Hours")]
    public int hourWorkReq = 9;
    public int hourStopReq = 17;

    [Header("Wandering Controller")]
    public int maxSimultaneousWanderers = 50;
    public int currentWandererCount = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        marketWindow.SetActive(false);
        denyButton.SetActive(false);
    }

    void Update()
    {
        if(LoseCondition.instance.lost) return;
        if(Settings.instance.isOpen || ActiveWindow.instance.isActive || ActiveWindow.instance.briefActive) return;
        
        UpdateVisuals();    

        if ((Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) && !ViewMode.instance.viewMode)
        {
            if (CameraPinch.Instance != null && CameraPinch.Instance.IsPanning) 
            {
                return;
            }

            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            Collider2D hit = Physics2D.OverlapPoint(clickPos, villagerLayer);

            if (hit != null)
            {
                if (hit.TryGetComponent(out VillagerAI villager))
                {
                    villager.HandleSelection();
                }
                else 
                {
                    VillagerAI failedVillager = hit.GetComponentInParent<VillagerAI>();
                    if (failedVillager != null && hit == failedVillager.failedCol)
                    {
                        failedVillager.HandleFailedClick();
                    }
                }
            }
            else if (activeBuilding == null && !isBuilding)
            {
                for (int i = 0; i < villagers.Count; i++)
                {
                    if (villagers[i] != null && villagers[i].isShowing && !villagers[i].goingToHouse)
                    {
                        villagers[i].HideVisuals();
                    }
                }
            }
        }
    }
    

    public void CalculateHousedPopulation()
    {
        housedPopulation = 0;
        foreach(var villager in villagers)
        {
            if(villager.house != null)
            {
                housedPopulation++;
            }
        }
    }

    public void CalculateWorkingPopulation()
    {
        workingPopulation = 0;
        foreach(var villager in villagers)
        {
            if(villager.jobPlace != null)
            {
                workingPopulation++;
            }
        }
    }

    public void CalculateFarms()
    {
        farms = 0;
        foreach(var building in GridManager.instance.buildings)
        {
            Farm farmScript = building as Farm;
            if(farmScript != null)
            {
                farms++;
            }
        }
    }

    public void CalculateHouses()
    {
        houses = 0;
        foreach(var building in GridManager.instance.buildings)
        {
            Building buildingScript = building as Building;
            Farm farmScript = building as Farm;
            if(farmScript == null && buildingScript != null)
            {
                houses++;
            }
        }
    }

    public void CalculateMines()
    {
        mines = 0;
        foreach(var building in GridManager.instance.buildings)
        {
            Mines minesScript = building as Mines;
            if(minesScript != null)
            {
                mines++;
            }
        }
    }

    public void UpdateVisuals()
    {
        CalculateHousedPopulation(); //Change this to update whenever a villager gets housed by a house
        CalculateWorkingPopulation();
        CalculateFarms(); 
        CalculateHouses();
        CalculateMines();
        moralityVisual.text = "Global Morality: " + TownStorage.instance.globalMorality.ToString("F2") + " / 1.00";
        populationVisual.text = "Population: " + villagers.Count.ToString();
        housedPopulationVisual.text = "Housed Population: " + housedPopulation.ToString();
        workingPopulationVisual.text = "Working Population: " + workingPopulation.ToString();
        housesVisual.text = "Houses: " + houses.ToString();
        farmsVisual.text = "Farms: " + farms.ToString();
        minesVisual.text = "Mines: " + mines.ToString();
    }

    public void CheckHour()
    {
        DayCycle cycle = DayCycle.instance;

        if(cycle.hours >= hourSleepReq || cycle.hours < hourAwakeReq)
        {
            SleepAllVillagers();
            TownStorage.instance.StopVillagerCooldown();
            LoseCondition.instance.CheckLossByMorality();
            if(!TownStorage.instance.hasCheckedTomorrow)
            {
                Stats.instance.totalDays++;
                ReduceDayVillagers();
                MarketSystem.instance.RandomizeDemand();
                HungerManager.instance.EndOfDayFeed();
                TownStorage.instance.CalculateTomorrowMorality();
                TownStorage.instance.hasCheckedTomorrow = true;
                LoseCondition.instance.DecreaseLossMorality();
            }

            if(LoseCondition.instance.LossByMorality())
            {
                LoseCondition.instance.CheckLossCondition();
            }
            
        }
        else if(cycle.hours >= hourAwakeReq && cycle.hours < hourWorkReq)
        {
            TownStorage.instance.hasCheckedTomorrow = false;
            AwakeAllVillagers();
            TownStorage.instance.StartVillagerCooldown();
        }
        else if(cycle.hours >= hourWorkReq && cycle.hours < hourStopReq)
        {
            WorkAllVillagers();
        }
        else if(cycle.hours >= hourStopReq && cycle.hours < hourSleepReq)
        {
            StopWorkAllVillagers();
        }
    }
    public void SleepAllVillagers()
    {
        foreach(var villager in villagers)
        {
            villager.MoveVillagerToHouse();
        }
    }
    public void AwakeAllVillagers()
    {
        foreach(var villager in villagers)
        {
            villager.WakeUpFromHouse();
        }
    }

    public void WorkAllVillagers()
    {
        foreach(var villager in villagers)
        {
            villager.GoToWork();
        }
    }

    public void ReduceDayVillagers()
    {
        foreach(var villager in villagers)
        {
            if(villager.villagerHealth.health == Health.Incubation)
            {
                villager.villagerHealth.ReduceDays();
            }
        }
    }

    public void StopWorkAllVillagers()
    {
        foreach(var villager in villagers)
        {
            if(villager.jobPlace == null)
            {
                continue;
            }

            if(villager.jobPlace.TryGetComponent(out Farm villagerFarm))
            {
                villagerFarm.StopFarming();
            }
            else if(villager.jobPlace.TryGetComponent(out Mines villagerMines))
            {
                villagerMines.StopMining();
            }
        }
    }

    public bool RequestWanderSlot()
    {
        if (currentWandererCount < maxSimultaneousWanderers)
        {
            currentWandererCount++;
            return true;
        }
        return false;
    }
    public void ReleaseWanderSlot()
    {
        currentWandererCount = Mathf.Max(0, currentWandererCount - 1);
    }


    //Building - Choosing Human
    public void SelectingHumanMode(VillageBuildable building, bool? inverse = false, bool infected = false)
    {
        denyButton.SetActive(true);
        
        bool isWorkplace = (building is Farm) || (building is Mines);
        Building buildingScript = building as Building;

        if (inverse is true)
        {
            if (buildingScript == null) return;

            foreach (var villager in buildingScript.villagers)
            {
                if(villager.state == VillagerState.Sleeping)
                {
                    activeBuilding.isChoosing = false;
                    activeBuilding = null;
                    denyButton.SetActive(false);
                    PopupText.instance.Popup("Can't disown villager whilst sleeping.");
                    villager.rend.sprite = null;
                    return;
                }
                villager.villagerSprite.Selected();

            }
            return;
        }
        else if(infected)
        {
            if (buildingScript == null) return;

            foreach (var villager in villagers)
            {
                if(villager.villagerHealth.health != Health.Healthy && villager.quarantine == null)
                {
                    villager.villagerSprite.Selected(); 
                }
            }

            return;
        }

        foreach (var villager in villagers)
        {
            if (isWorkplace)
            {
                if (villager.jobPlace == null && villager.quarantine == null)
                {
                    villager.villagerSprite.Selected();
                }
                else
                {
                    villager.villagerSprite.DeSelected();
                }
            }
            else
            {
                if (villager.house == null && villager.quarantine == null)
                {
                    villager.villagerSprite.Selected();
                }
                else if(villager.state != VillagerState.Sleeping && villagers[0].rend.sprite != null)
                {
                    villager.villagerSprite.DeSelected();
                }
            }
        }
    }

    public void ShowSelectedHumans(VillageBuildable building)
    {
        Building buildingScript = building as Building;
        
        if(buildingScript == null)
        {
            Debug.Log("No building script");
            return;
        }

        foreach(var villager in buildingScript.villagers)
        {
            if(villager.state == VillagerState.Sleeping)
            {
                return;
            }

            villager.villagerSprite.Selected();
        }
    }

    public void HideSelectedHumans(VillageBuildable building)
    {
        Building buildingScript = building as Building;

        if(buildingScript == null)
        {
            Debug.Log("No building script");
            return;
        }

        foreach(var villager in buildingScript.villagers)
        {
            if(villager.state == VillagerState.Sleeping)
            {
                return;
            }

            villager.villagerSprite.DeSelected();
        }
    }

    public void DenyChoosingHuman()
    {
        foreach(var villager in villagers)
        {
            if(villager.state != VillagerState.Sleeping && villagers[0].rend.sprite != null)
            {
                villager.villagerSprite.DeSelected();
            }
        }
        activeBuilding.isChoosing = false;
        activeBuilding = null;
        denyButton.SetActive(false);
    }

    public void SelectedHuman(VillagerAI villager)
    {
        activeBuilding.villagers.Add(villager);
        activeBuilding.AssignVillagerRole(villager);
        activeBuilding.isChoosing = false;
        activeBuilding.UpdateVisuals();
        activeBuilding = null;
        denyButton.SetActive(false);

        foreach(var villager1 in villagers)
        {
            if(villager.state != VillagerState.Sleeping && villagers[0].rend.sprite != null)
            {
                villager.villagerSprite.DeSelected();
            }
        }
    }

    public void RemoveHuman(VillagerAI villager)
    {
        activeBuilding.RemoveVillagerRole(villager);
        activeBuilding.isChoosing = false;
        activeBuilding.isDisowning = false;
        activeBuilding.villagers.Remove(villager);
        activeBuilding.UpdateVisuals();
        denyButton.SetActive(false);

        villager.villagerSprite.DeSelected();

        foreach(var villager1 in activeBuilding.villagers)
        {
            villager1.villagerSprite.DeSelected();
        }

        activeBuilding = null;
    }

    public void RemoveHumanManually(VillagerAI villager, Building building)
    {
        building.RemoveVillagerRole(villager);
        building.isChoosing = false;
        building.isDisowning = false;
        building.villagers.Remove(villager);
        building.UpdateVisuals();
        denyButton.SetActive(false);

        villager.villagerSprite.DeSelected();

        foreach(var villager1 in building.villagers)
        {
            villager1.villagerSprite.DeSelected();
        }
    }
    

    //For Market
    public void CloseMarketWindow()
    {
        if(availableMarket == null) return;

        availableMarket.HideVisuals();
    }

    public void UpdateSliderVisual(int id)
    {
        if(availableMarket == null) return;

        availableMarket.UpdateSliderVisual(id);
    }

    public void Sell()
    {
        if(availableMarket == null) return;

        availableMarket.Sell();
    }

    //For Laboratory
    public void OpenLaboratoryWindow()
    {
        if (TownManager.instance.availableQuarantine == null && VaccineSystem.instance.curedVirusId.Count == 0) 
        {
            PopupText.instance.Popup("Need Quarantine to function.");
            return;
        }
        
        if (TownManager.instance.availableQuarantine.villagers.Count == 0 && VaccineSystem.instance.curedVirusId.Count == 0) 
        {
            PopupText.instance.Popup("Need Quarantined Villagers to function.");
            return;
        }

        TownManager.instance.availableLaboratory.isShowing = true;

        if(TimeForward.instance.choosing == 0)
        {
            TimeForward.instance.choosing = 1;
            TimeForward.instance.UpdateTimeScale();
        }

        if(TownManager.instance.availableQuarantine.villagers.Count > 0)
        {
            TownManager.instance.availableLaboratory.laboratoryWindow.SetActive(true);
            foreach(var sideButton in VaccineSystem.instance.sideButtons)
            {
                sideButton.SetActive(true);
            }
            LaboratorySystem.instance.UpdateVisuals();
        }
        else if(VaccineSystem.instance.curedVirusId.Count > 0)
        {
            VaccineSystem.instance.OpenWindow(true);
        }
    }

    public void CloseLaboratoryWindow()
    {
        if(availableLaboratory == null) return;

        availableLaboratory.laboratoryWindow.SetActive(false);
        VaccineSystem.instance.CloseWindow();
        availableLaboratory.isShowing = false;
        LookAhead.instance.SetValues(false);
        LookAhead.instance.CloseWindow();
    }
}
