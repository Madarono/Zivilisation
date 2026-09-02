using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    //VillagerSavingSystem.cs
    public List<int> villagerId = new List<int>();
    public List<int> houseId = new List<int>();
    public List<int> jobId = new List<int>();
    public List<int> quarantineId = new List<int>();
    public List<Vector3> villagerPos = new List<Vector3>();
    public List<float> villagerHunger = new List<float>();
    public List<int> daysLeft = new List<int>();
    public List<Virus> deadVillagerVirus = new List<Virus>();
    public List<Vector3> deadVillagerPos = new List<Vector3>();
    public List<Health> villagerHealth = new List<Health>();
    public List<Virus> villagerVirus = new List<Virus>();

    public int totalDead;

    public List<int> motelId = new List<int>();
    public List<int> motelTypeId = new List<int>();
    public List<int> motelSellValue = new List<int>();
    public List<Vector3> motelPos = new List<Vector3>();

    public List<int> workplaceId = new List<int>();
    public List<int> workplaceTypeId = new List<int>();
    public List<int> workplaceSellValue = new List<int>();
    public List<int> matSelectionId = new List<int>();
    public List<Vector3> workplacePos = new List<Vector3>();

    public float[] dailyDemand = new float[5];
    public float demandPower;

    public List<Vector2> roadPos = new List<Vector2>();

    public int moneySave;
    public float wheatSave;
    public int ironSave;
    public int copperSave;
    public int quartzSave;
    public int titaniumSave;
    public float globalMoralitySave;
    public bool hasCheckedTomorrow;

    public int hourSave;
    public int minuteSave;
    public float secondSave;

    public float sfxValue;
    public float musicValue;
    public bool muteSfx;
    public bool muteMusic;
    public int graphicsIndex;
    public bool canScreenShake;
    public int fpsIndex;

    public List<Virus> viruses = new List<Virus>();

    public float timeInsideCurrent;
    public float timeoutCurrent;
    public QuarantineState quarantineState;

    public int feedId;
    public int percentageId;

    public int currentDays;
    public bool progressLosMor;

    public int totalDays;
    public float lowestMorality;
    public int desertions;
    public int totalSick;
    public int totalMoneyGained;

    public List<string> pageInfo = new List<string>();
    public List<string> headerInfo = new List<string>();

    public List<string> virusNames = new List<string>();
    public List<int> manualPage = new List<int>();

    public List<int> curedVirusId = new List<int>();
    public List<int> vaccinatedVirusId = new List<int>();

    public GameData()
    {
        //VillagerSavingSystem.cs
        villagerId = new List<int>();
        houseId = new List<int>();
        jobId = new List<int>();
        quarantineId = new List<int>();
        villagerPos = new List<Vector3>();
        villagerHunger = new List<float>();
        daysLeft = new List<int>();

        deadVillagerPos = new List<Vector3>();
        deadVillagerVirus = new List<Virus>();
        totalDead = 0;
        
        villagerHealth = new List<Health>();
        villagerVirus = new List<Virus>();

        motelId = new List<int>();
        motelTypeId = new List<int>();
        motelSellValue = new List<int>();
        motelPos = new List<Vector3>();

        workplaceId = new List<int>();
        workplaceTypeId = new List<int>();
        workplaceSellValue = new List<int>();
        workplacePos = new List<Vector3>();
        matSelectionId = new List<int>();

        dailyDemand = new float[5];
        demandPower = 1f;

        roadPos = new List<Vector2>();

        moneySave = 100;
        wheatSave = 3f;
        ironSave = 0;
        copperSave = 0;
        quartzSave = 0;
        titaniumSave = 0;
        globalMoralitySave = 0.5f;
        hasCheckedTomorrow = false;

        hourSave = 6; //Time when Villagers wake up
        minuteSave = 0;
        secondSave = 0;

        sfxValue = 100f;
        musicValue = 100f;
        muteSfx = false;
        muteMusic = false;
        graphicsIndex = 2;
        canScreenShake = true;
        fpsIndex = 1;

        viruses = new List<Virus>();

        timeInsideCurrent = 0f;
        timeoutCurrent = 0f;
        quarantineState = QuarantineState.Able;

        feedId = 0;
        percentageId = 0;

        currentDays = 0;
        progressLosMor = false;

        totalDays = 0;
        lowestMorality = 0f;
        desertions = 0;
        totalSick = 0;
        totalMoneyGained = 0;

        pageInfo = new List<string>();
        headerInfo = new List<string>();

        virusNames = new List<string>();
        manualPage = new List<int>();

        curedVirusId = new List<int>();
        vaccinatedVirusId = new List<int>();
    }
}
