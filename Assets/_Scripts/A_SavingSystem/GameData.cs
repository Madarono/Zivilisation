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

    public List<bool> doneTutorials = new List<bool>();

    public bool newGame;

    public Mode mode;
    public Difficulty difficulty;

    public bool hasWon;

    public GameData()
    {
        sfxValue = 100f;
        musicValue = 100f;
        muteSfx = false;
        muteMusic = false;
        graphicsIndex = 2;
        canScreenShake = true;
        fpsIndex = 1;
        mode = Mode.Normal;
        difficulty = Difficulty.Normal;

        ResetToNewGame();
    }

    public void ResetToNewGame() //For any new playthrough
    {
        villagerId.Clear();
        houseId.Clear();
        jobId.Clear();
        quarantineId.Clear();
        villagerPos.Clear();
        villagerHunger.Clear();
        daysLeft.Clear();

        deadVillagerPos.Clear();
        deadVillagerVirus.Clear();
        totalDead = 0;

        villagerHealth.Clear();
        villagerVirus.Clear();

        motelId.Clear();
        motelTypeId.Clear();
        motelSellValue.Clear();
        motelPos.Clear();

        workplaceId.Clear();
        workplaceTypeId.Clear();
        workplaceSellValue.Clear();
        workplacePos.Clear();
        matSelectionId.Clear();

        dailyDemand = new float[5];
        demandPower = 1f;

        roadPos.Clear();

        moneySave = 100;
        wheatSave = 3f;
        ironSave = 0;
        copperSave = 0;
        quartzSave = 0;
        titaniumSave = 0;
        globalMoralitySave = 0.5f;
        hasCheckedTomorrow = false;

        hourSave = 6;
        minuteSave = 0;
        secondSave = 0;

        viruses.Clear();

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

        pageInfo.Clear();
        headerInfo.Clear();

        virusNames.Clear();
        manualPage.Clear();

        curedVirusId.Clear();
        vaccinatedVirusId.Clear();

        doneTutorials.Clear();

        newGame = true;
        hasWon = true;
    }
}
