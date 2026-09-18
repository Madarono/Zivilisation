using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum Mode 
{
    Normal,
    Endless
}

public class GameMode : MonoBehaviour
{
    public static GameMode instance {get; private set;}
    public Mode mode;
    public Difficulty difficulty;

    public DifficultyScale currentDifficulty;

    void Awake()
    {
        instance = this;
    }

    public void ApplyMode(bool newGame = false)
    {
        GetDifficulty();
        
        float multiplier = 1f;

        if (mode == Mode.Endless)
        {
            float growth = DifficultyScaling.instance.dailyMultiplyer * Stats.instance.totalDays;
            multiplier = Mathf.Min(1f + growth, DifficultyScaling.instance.maxMultiplyer);
        }

        // Viruses get stronger
        VirusManager.instance.power = currentDifficulty.virusPower * multiplier;

        // Market bounds
        MarketSystem.instance.minPower = currentDifficulty.minPower;
        MarketSystem.instance.maxPower = currentDifficulty.maxPower;
        MarketSystem.instance.maxDemand = currentDifficulty.maxDemand / multiplier;
        MarketSystem.instance.minDemand = currentDifficulty.minDemand / multiplier;

        BuildOptions.instance.priceValueMultiplyer = (1f - currentDifficulty.shopDiscount) * multiplier;

        TownStorage.instance.penaltyMultiplyer = (1f - currentDifficulty.penaltyDiscount) * multiplier;

        if (newGame)
        {
            TownStorage.instance.Money = currentDifficulty.startingCash;
        }
    }
    void GetDifficulty()
    {
        if(mode == Mode.Endless)
        {
            for(int i = 0; i < DifficultyScaling.instance.difficultyScales.Length; i++)
            {
                if(DifficultyScaling.instance.difficultyScales[i].difficulty == Difficulty.Normal)
                {
                    currentDifficulty = DifficultyScaling.instance.difficultyScales[i];
                    break;
                }
            }
            return;
        }

        for(int i = 0; i < DifficultyScaling.instance.difficultyScales.Length; i++)
        {
            if(DifficultyScaling.instance.difficultyScales[i].difficulty == difficulty)
            {
                currentDifficulty = DifficultyScaling.instance.difficultyScales[i];
                break;
            }
        }
    }

    bool IsNewGame()
    {
        VillagerSavingSystem save = VillagerSavingSystem.instance;

        return save.moneySave == 100 && save.wheatSave == 3f && save.ironSave == 0 && save.copperSave == 0 && save.quartzSave == 0 && save.titaniumSave == 0 && save.globalMoralitySave == 0.5f;
    }
}