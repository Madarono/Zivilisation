using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class VillagerSavingSystem : MonoBehaviour, IDataPersistence
{
    [Header("Villager Saving")]
    public List<int> villagerId = new List<int>();
    public List<int> houseId = new List<int>();
    public List<int> jobId = new List<int>();
    public List<int> quarantineId = new List<int>();
    public List<Vector3> villagerPos = new List<Vector3>();
    public List<float> villagerHunger = new List<float>();
    public List<int> daysLeft = new List<int>();

    [Header("Dead Villagers Saving")]
    public List<Virus> deadVillagerVirus = new List<Virus>();
    public List<Vector3> deadVillagerPos = new List<Vector3>();
    public GameObject deadVillagerPrefab;
    public int totalDead;
    
    [Header("Villager Health Saving")]
    public List<Health> villagerHealth = new List<Health>();
    public List<Virus> villagerVirus = new List<Virus>();

    [Header("Building Saving")]
    public List<int> motelId = new List<int>();
    public List<int> motelTypeId = new List<int>();
    public List<int> motelSellValue = new List<int>();
    public List<Transform> motelTransform = new List<Transform>();
    public List<Vector3> motelPos = new List<Vector3>();

    public List<int> workplaceId = new List<int>();
    public List<int> workplaceTypeId = new List<int>();
    public List<int> workplaceSellValue = new List<int>();
    public List<Transform> workplaceTransform = new List<Transform>();
    public List<Vector3> workplacePos = new List<Vector3>();

    [Header("Demand Saving")]
    public float[] dailyDemand = new float[5];
    public float demandPower;

    [Header("Road Saving")]
    public List<Vector2> roadPos = new List<Vector2>();

    [Header("Town Storage")]
    public int moneySave;
    public float wheatSave;
    public int ironSave;
    public int copperSave;
    public int quartzSave;
    public int titaniumSave;
    public float globalMoralitySave;
    public bool hasCheckedTomorrow;

    [Header("DayCycle")]
    public int hourSave;
    public int minuteSave;
    public float secondSave;

    [Header("Loading")]
    public GameObject villagerPrefab;
    public GameObject[] buildingIdPrefabs;

    //VirusManager
    [Header("Virus Manager")]
    public List<Virus> viruses = new List<Virus>();

    //Settings
    [Header("Settings")]
    public float sfxValue;
    public float musicValue;
    public bool muteSfx;
    public bool muteMusic;
    public int graphicsIndex;
    public bool canScreenShake;
    public int fpsIndex;

    //Quarantine
    [Header("Quarantine")]
    public float timeInsideCurrent;
    public float timeoutCurrent;
    public QuarantineState quarantineState;

    [Header("HungerManager")]
    public int feedId;
    public int percentageId;

    [Header("Lose Condition")]
    public int currentDays;
    public bool progressLosMor;

    [Header("Stats")]
    public int totalDays;
    public float lowestMorality;
    public int desertions;
    public int totalSick;
    public int totalMoneyGained;

    [Header("Player-Based Manual")]
    public List<string> pageInfo = new List<string>();
    public List<string> headerInfo = new List<string>();

    [Header("Laboratory System")]
    public List<string> virusNames = new List<string>();
    public List<int> manualPage = new List<int>();

    [Header("Vaccine System")]
    public List<int> curedVirusId = new List<int>();
    public List<int> vaccinatedVirusId = new List<int>();

    //Temporary then delete immediately
    private List<Building> lateStateBuildings = new List<Building>();

    public void SaveData(GameData data)
    {
        SaveInfo(); 
        data.villagerId = this.villagerId;
        data.houseId = this.houseId;
        data.jobId = this.jobId;
        data.quarantineId = this.quarantineId;
        data.villagerPos = this.villagerPos;
        data.villagerHunger = this.villagerHunger;
        data.daysLeft = this.daysLeft;

        data.deadVillagerPos = this.deadVillagerPos;
        data.deadVillagerVirus = this.deadVillagerVirus;
        data.totalDead = this.totalDead;


        data.villagerHealth = this.villagerHealth;
        data.villagerVirus = this.villagerVirus;

        data.motelId = this.motelId;
        data.motelTypeId = this.motelTypeId;
        data.motelSellValue = this.motelSellValue;
        data.motelPos = this.motelPos;

        data.workplaceId = this.workplaceId;
        data.workplaceTypeId = this.workplaceTypeId;
        data.workplaceSellValue = this.workplaceSellValue;
        data.workplacePos = this.workplacePos;

        data.dailyDemand = this.dailyDemand;
        data.demandPower = this.demandPower;

        data.roadPos = this.roadPos;

        data.moneySave = this.moneySave;
        data.wheatSave = this.wheatSave;
        data.ironSave = this.ironSave;
        data.copperSave = this.copperSave;
        data.quartzSave = this.quartzSave;
        data.titaniumSave = this.titaniumSave;
        data.globalMoralitySave = this.globalMoralitySave;
        data.hasCheckedTomorrow = this.hasCheckedTomorrow;

        data.hourSave = this.hourSave;
        data.minuteSave = this.minuteSave;
        data.secondSave = this.secondSave;

        data.sfxValue = this.sfxValue;
        data.musicValue = this.musicValue;
        data.muteSfx = this.muteSfx;
        data.muteMusic = this.muteMusic;
        data.graphicsIndex = this.graphicsIndex;
        data.canScreenShake = this.canScreenShake;
        data.fpsIndex = this.fpsIndex;
        
        data.viruses = this.viruses;
        
        data.timeInsideCurrent = this.timeInsideCurrent;
        data.timeoutCurrent = this.timeoutCurrent;
        data.quarantineState = this.quarantineState;

        data.feedId = this.feedId;
        data.percentageId = this.percentageId;

        data.currentDays = this.currentDays;
        data.progressLosMor = this.progressLosMor;

        data.totalDays = this.totalDays;
        data.lowestMorality = this.lowestMorality;
        data.desertions = this.desertions;
        data.totalSick = this.totalSick;
        data.totalMoneyGained = this.totalMoneyGained;

        data.pageInfo = this.pageInfo;
        data.headerInfo = this.headerInfo;
        
        data.virusNames = this.virusNames;
        data.manualPage = this.manualPage;

        data.curedVirusId = this.curedVirusId;
        data.vaccinatedVirusId = this.vaccinatedVirusId;
    }

    public void LoadData(GameData data)
    {
        this.villagerId = data.villagerId;
        this.houseId = data.houseId;
        this.jobId = data.jobId;
        this.quarantineId = data.quarantineId;
        this.villagerPos = data.villagerPos;
        this.villagerHunger = data.villagerHunger;
        this.daysLeft = data.daysLeft;

        this.deadVillagerPos = data.deadVillagerPos;
        this.deadVillagerVirus = data.deadVillagerVirus;
        this.totalDead = data.totalDead;

        this.villagerHealth = data.villagerHealth;
        this.villagerVirus = data.villagerVirus;

        this.motelId = data.motelId;
        this.motelTypeId = data.motelTypeId;
        this.motelSellValue = data.motelSellValue;
        this.motelPos = data.motelPos;

        this.workplaceId = data.workplaceId;
        this.workplaceTypeId = data.workplaceTypeId;
        this.workplaceSellValue = data.workplaceSellValue;
        this.workplacePos = data.workplacePos;

        this.dailyDemand = data.dailyDemand;
        this.demandPower = data.demandPower;

        this.roadPos = data.roadPos;

        this.moneySave = data.moneySave;
        this.wheatSave = data.wheatSave;
        this.ironSave = data.ironSave;
        this.copperSave = data.copperSave;
        this.quartzSave = data.quartzSave;
        this.titaniumSave = data.titaniumSave;
        this.globalMoralitySave = data.globalMoralitySave;
        this.hasCheckedTomorrow = data.hasCheckedTomorrow;

        this.hourSave = data.hourSave;
        this.minuteSave = data.minuteSave;
        this.secondSave = data.secondSave;

        this.sfxValue = data.sfxValue;
        this.musicValue = data.musicValue;
        this.muteSfx = data.muteSfx;
        this.muteMusic = data.muteMusic;
        this.graphicsIndex = data.graphicsIndex;
        this.canScreenShake = data.canScreenShake;
        this.fpsIndex = data.fpsIndex;

        this.viruses = data.viruses;

        this.timeInsideCurrent = data.timeInsideCurrent;
        this.timeoutCurrent = data.timeoutCurrent;
        this.quarantineState = data.quarantineState;

        this.feedId = data.feedId;
        this.percentageId = data.percentageId;

        this.currentDays = data.currentDays;
        this.progressLosMor = data.progressLosMor;

        this.totalDays = data.totalDays;
        this.lowestMorality = data.lowestMorality;
        this.desertions = data.desertions;
        this.totalSick = data.totalSick;
        this.totalMoneyGained = data.totalMoneyGained;

        this.pageInfo = data.pageInfo;
        this.headerInfo = data.headerInfo;

        this.virusNames = data.virusNames;
        this.manualPage = data.manualPage;

        this.curedVirusId = data.curedVirusId;
        this.vaccinatedVirusId = data.vaccinatedVirusId;

        LoadInfo();
    }

    [ContextMenu("SaveInfo")]
    public void SaveInfo()
    {
        villagerId.Clear(); quarantineId.Clear(); houseId.Clear(); jobId.Clear(); villagerPos.Clear(); villagerHunger.Clear(); villagerHealth.Clear(); villagerVirus.Clear(); daysLeft.Clear(); deadVillagerPos.Clear(); deadVillagerVirus.Clear();
        motelId.Clear(); motelTypeId.Clear(); motelTransform.Clear(); motelPos.Clear(); motelSellValue.Clear();
        workplaceId.Clear(); workplaceTypeId.Clear(); workplaceTransform.Clear(); workplaceSellValue.Clear(); workplacePos.Clear();
        roadPos.Clear(); viruses.Clear();
        pageInfo.Clear(); headerInfo.Clear();
        virusNames.Clear();
        manualPage.Clear();
        curedVirusId.Clear(); vaccinatedVirusId.Clear();

        //Gathering Buildings' Info
        GridManager grid = GridManager.instance;
        TownManager town = TownManager.instance;
        RoadSystem roadSys = RoadSystem.instance;
        int idCounter = 0;

        foreach(var building in grid.buildings)
        {
            Building buildingScript = building as Building;
            Farm farmScript = building as Farm;
            Mines minesScript = building as Mines;
            Quarantine quarantineScript = building as Quarantine;

            if(buildingScript != null && farmScript == null && minesScript == null && quarantineScript == null) //Houses only here
            {
                motelId.Add(idCounter);
                motelTypeId.Add(buildingScript.buildingId);
                motelTransform.Add(buildingScript.gameObject.transform);
                motelPos.Add(buildingScript.gameObject.transform.position);
                motelSellValue.Add(buildingScript.sellValue);
                idCounter++;
                continue;
            }

            workplaceId.Add(idCounter);
            workplaceTypeId.Add(buildingScript.buildingId);
            workplaceTransform.Add(buildingScript.gameObject.transform);
            workplacePos.Add(buildingScript.gameObject.transform.position);
            workplaceSellValue.Add(buildingScript.sellValue);
            idCounter++;
        }

        idCounter = 0;

        //Gathering Villagers' Info
        foreach(var villager in town.villagers)
        {
            villagerId.Add(idCounter);
            houseId.Add(IdExtractor(motelTransform, motelId, villager.house));
            jobId.Add(IdExtractor(workplaceTransform, workplaceId, villager.jobPlace));
            quarantineId.Add(IdExtractor(workplaceTransform, workplaceId, villager.quarantine));
            villagerPos.Add(villager.gameObject.transform.position);
            villagerHunger.Add(villager.hunger);
            villagerHealth.Add(villager.villagerHealth.health);
            villagerVirus.Add(villager.villagerHealth.inflictedVirus);
            daysLeft.Add(villager.villagerHealth.daysLeft);
            idCounter++;
        }

        //Gathering the Dead Villagers' Info
        foreach(var deadVillager in town.deadVillagers)
        {
            deadVillagerVirus.Add(deadVillager.inflictedVirus);
            deadVillagerPos.Add(deadVillager.gameObject.transform.position);
        }

        totalDead = TownManager.instance.totalDead;

        //Gathering the Roads' Info
        foreach(var road in roadSys.roadPos)
        {
            Vector2 pos = new Vector2(road.Key.x, road.Key.y);
            roadPos.Add(pos);
        }

        //Gathering the Town Storage's Info
        moneySave = TownStorage.instance.Money;
        wheatSave = TownStorage.instance.wheat;
        ironSave = TownStorage.instance.iron;
        copperSave = TownStorage.instance.copper;
        quartzSave = TownStorage.instance.quartz;
        titaniumSave = TownStorage.instance.titanium;
        globalMoralitySave = TownStorage.instance.globalMorality;
        hasCheckedTomorrow = TownStorage.instance.hasCheckedTomorrow;


        //Gathering the Daycycle's Info
        hourSave = DayCycle.instance.hours;
        minuteSave = DayCycle.instance.minutes;
        secondSave = DayCycle.instance.seconds;

        //Gathering the Settings's Info
        sfxValue = Settings.instance.sfxValue;
        musicValue = Settings.instance.musicValue;
        muteSfx = Settings.instance.muteSfx;
        muteMusic = Settings.instance.muteMusic;
        graphicsIndex = Settings.instance.graphicsIndex;
        canScreenShake = Settings.instance.canScreenShake;
        fpsIndex = Settings.instance.fpsIndex;

        //Gathering the MarketSystem's Info
        dailyDemand = (float[])MarketSystem.instance.dailyDemand.Clone();
        demandPower = MarketSystem.instance.demandPower;
        
        //Gathering the VirusManager's Info
        viruses = new List<Virus>(VirusManager.instance.viruses);

        //Gathering the Quarantine's Info
        if(TownManager.instance.availableQuarantine != null)
        {
            timeInsideCurrent = TownManager.instance.availableQuarantine.timeInsideCurrent;
            timeoutCurrent = TownManager.instance.availableQuarantine.timeoutCurrent;
            quarantineState = TownManager.instance.availableQuarantine.state;
        }

        //Gathering the HungerManager's Info
        feedId = HungerManager.instance.feedId;
        percentageId = HungerManager.instance.percentageId;

        //Gathering the LoseCondition's Info
        currentDays = LoseCondition.instance.currentDays;
        progressLosMor = LoseCondition.instance.progressLosMor;

        //Gathering the Stats' info
        totalDays = Stats.instance.totalDays;
        lowestMorality = Stats.instance.lowestMorality;
        desertions = Stats.instance.desertions;
        totalSick = Stats.instance.totalSick;
        totalMoneyGained = Stats.instance.totalMoneyGained;

        //Gathering the ManualSystem's info
        pageInfo = new List<string>(ManualSystem.instance.pageInfo);
        headerInfo = new List<string>(ManualSystem.instance.headerInfo);

        //Gathering the LaboratorySystem's info
        virusNames = new List<string>(LaboratorySystem.instance.virusNames);
        manualPage = new List<int>(LaboratorySystem.instance.manualPage);

        //Gathering the VaccineSystem's info
        curedVirusId = new List<int>(VaccineSystem.instance.curedVirusId);
        vaccinatedVirusId = new List<int>(VaccineSystem.instance.vaccinatedVirusId);
    }

    public void LoadInfo()
    {
        motelTransform.Clear();
        workplaceTransform.Clear();

        //Roads
        for(int i = 0; i < roadPos.Count; i++)
        {
            Vector2Int pos = new Vector2Int((int)roadPos[i].x, (int)roadPos[i].y);
            RoadSystem.instance.PutRoad(pos);
        }

        //Buildings
        for(int i = 0; i < motelId.Count; i++)
        {
            GameObject go = Instantiate(buildingIdPrefabs[motelTypeId[i]], motelPos[i], Quaternion.identity);
            motelTransform.Add(go.transform);
            if(go.TryGetComponent(out Building goBuilding))
            {
                goBuilding.gridTaken.MeasureGridTaken();
                goBuilding.gridTaken.PutUnwalkable();
                goBuilding.sellValue = motelSellValue[i];
                GridManager.instance.buildings.Add(goBuilding);
            }
        }

        for(int i = 0; i < workplaceId.Count; i++)
        {
            GameObject go = Instantiate(buildingIdPrefabs[workplaceTypeId[i]], workplacePos[i], Quaternion.identity);
            workplaceTransform.Add(go.transform);
            if(go.TryGetComponent(out Building goBuilding))
            {
                goBuilding.gridTaken.MeasureGridTaken();
                goBuilding.gridTaken.PutUnwalkable();
                goBuilding.sellValue = workplaceSellValue[i];
                GridManager.instance.buildings.Add(goBuilding);
            }
        }

        //Villagers
        for(int i = 0; i < villagerId.Count; i++)
        {
            GameObject go = Instantiate(villagerPrefab, villagerPos[i], Quaternion.identity);
            if(go.TryGetComponent(out VillagerAI goScript))
            {
                goScript.villagerHealth.health = villagerHealth[i];
                goScript.villagerHealth.inflictedVirus = villagerVirus[i];
                goScript.villagerHealth.daysLeft = daysLeft[i];

                if(houseId[i] != -1) //Has a house
                {
                    goScript.house = GetTransformFromId(houseId[i], motelTransform, motelId);
                    if(goScript.house != null && goScript.house.TryGetComponent(out Building building))
                    {
                        building.AssignVillagerRole(goScript);
                        building.villagers.Add(goScript);
                    }
                }

                if(jobId[i] != -1)
                {
                    goScript.jobPlace = GetTransformFromId(jobId[i], workplaceTransform, workplaceId);
                    if(goScript.jobPlace != null && goScript.jobPlace.TryGetComponent(out Building building))
                    {
                        building.AssignVillagerRole(goScript);
                        building.villagers.Add(goScript);
                    }
                }

                if(quarantineId[i] != -1)
                {
                    goScript.quarantine = GetTransformFromId(quarantineId[i], workplaceTransform, workplaceId);
                    if(goScript.quarantine != null && goScript.quarantine.TryGetComponent(out Building building))
                    {
                        // building.AssignVillagerRole(goScript);
                        lateStateBuildings.Add(building);
                        building.villagers.Add(goScript);
                    }
                }

                goScript.hunger = villagerHunger[i];

                TownManager.instance.villagers.Add(goScript);
            }
        }

        //DeadVillagers
        for(int i = 0; i < deadVillagerPos.Count; i++)
        {
            GameObject go = Instantiate(deadVillagerPrefab, deadVillagerPos[i], Quaternion.identity);
            if(go.TryGetComponent(out VillagerDead goScript))
            {
                goScript.inflictedVirus = deadVillagerVirus[i];
            }
        }

        TownManager.instance.totalDead = totalDead;

        MoneyCounter.instance.deltaPrice = moneySave;
        
        //TownStorage
        TownStorage.instance.Money = moneySave;
        TownStorage.instance.wheat = wheatSave;
        TownStorage.instance.iron = ironSave;
        TownStorage.instance.copper = copperSave;
        TownStorage.instance.quartz = quartzSave;
        TownStorage.instance.titanium = titaniumSave;
        TownStorage.instance.globalMorality = globalMoralitySave;
        TownStorage.instance.hasCheckedTomorrow = hasCheckedTomorrow;

        //Daycycle
        DayCycle.instance.hours = hourSave;
        DayCycle.instance.minutes = minuteSave;
        DayCycle.instance.seconds = secondSave;
        DayCycle.instance.UpdateClock(true); //To force update on the time

        //Settings
        Settings.instance.sfxValue = sfxValue;
        Settings.instance.musicValue = musicValue;
        Settings.instance.muteSfx = muteSfx;
        Settings.instance.muteMusic = muteMusic;
        Settings.instance.graphicsIndex = graphicsIndex;
        Settings.instance.canScreenShake = canScreenShake;
        Settings.instance.fpsIndex = fpsIndex;
        Settings.instance.musicSlider.value = musicValue * Settings.instance.audioDivider;
        Settings.instance.sfxSlider.value = sfxValue * Settings.instance.audioDivider;
        Settings.instance.UpdateValues();
        Settings.instance.SetFPS();
        Settings.instance.ApplyChanges();

        //VirusManager
        VirusManager.instance.viruses = new List<Virus>(viruses);

        //MarketSystem.cs
        MarketSystem.instance.dailyDemand = (float[])dailyDemand.Clone();
        MarketSystem.instance.demandPower = demandPower;

        bool allZero = true;

        foreach(var demand in MarketSystem.instance.dailyDemand)
        {
            if(demand > 0)
            {
                allZero = false; 
                break;
            }
        }

        if(allZero)
        {
            MarketSystem.instance.RandomizeDemand();
        }

        //HungerManager.cs
        HungerManager.instance.feedId = feedId;
        HungerManager.instance.percentageId = percentageId;
        HungerManager.instance.UpdateVisuals();

        //LoseConditions.cs
        LoseCondition.instance.currentDays = currentDays;
        LoseCondition.instance.progressLosMor = progressLosMor;

        //Stats.cs
        Stats.instance.totalDays = totalDays;
        Stats.instance.lowestMorality = lowestMorality;
        Stats.instance.desertions = desertions;
        Stats.instance.totalSick = totalSick;
        Stats.instance.totalMoneyGained = totalMoneyGained;

        //ManualSyustem.cs
        ManualSystem.instance.pageInfo = new List<string>(this.pageInfo);
        ManualSystem.instance.headerInfo = new List<string>(this.headerInfo);

        //LaboratorySystem.cs
        LaboratorySystem.instance.virusNames = new List<string>(this.virusNames);
        LaboratorySystem.instance.manualPage = new List<int>(this.manualPage);

        //VaccineSystem.cs
        VaccineSystem.instance.curedVirusId = new HashSet<int>(this.curedVirusId);
        VaccineSystem.instance.vaccinatedVirusId = new HashSet<int>(this.vaccinatedVirusId);

        StartCoroutine(WaitForStart());
    }

    int IdExtractor(List<Transform> placeTransform, List<int> placeId, Transform placeToFind)
    {
        if(placeToFind == null) return -1;

        for(int i = 0; i < placeTransform.Count; i++)
        {
            if(placeToFind == placeTransform[i])
            {
                return placeId[i];
            }
        }

        return -1;
    }

    Transform GetTransformFromId(int id, List<Transform> resultTransform, List<int> resultId)
    {
        for(int i = 0; i < resultId.Count; i++)
        {
            if(id == resultId[i])
            {
                return resultTransform[i];
            }
        }

        return null;
    }

    IEnumerator WaitForStart()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        LateStart();
    }

    void LateStart()
    {
        TownManager.instance.CheckHour();

        //For the Quarantine
        TownManager townManager = TownManager.instance;

        if(townManager.availableQuarantine != null)
        {
            townManager.availableQuarantine.timeInsideCurrent = timeInsideCurrent;
            townManager.availableQuarantine.timeoutCurrent = timeoutCurrent;
            townManager.availableQuarantine.state = quarantineState;

            if(townManager.availableQuarantine.state == QuarantineState.Able && timeInsideCurrent != 0 && townManager.availableQuarantine.insideCoroutine == null)
            {
                townManager.availableQuarantine.StartInside();
            }
            else if(townManager.availableQuarantine.state == QuarantineState.Timeout && timeoutCurrent != 0 && townManager.availableQuarantine.outsideCoroutine == null)
            {
                townManager.availableQuarantine.StartOutside();
            }
        }

        //For all
        foreach(var building in lateStateBuildings)
        {
            Quarantine quarantine = building as Quarantine;

            foreach(var villager in building.villagers)
            {
                building.AssignVillagerRole(villager);
            }
        }

        LoseCondition.instance.CheckLossPopulation();
        LoseCondition.instance.CheckLossCondition();
    }
}