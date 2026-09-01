using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum LoseType
{
    Morality,
    Population,
    Sickness
}

public class LoseCondition : MonoBehaviour
{
    public static LoseCondition instance {get; private set;}

    [Header("Loss by morality #1")] //Checked by TownManager.cs upon villagerWake
    public float lowMorReq = 0.1f;
    public int lowMorDays = 3; //How many days under the requirement before losing #1
    public int currentDays = 0;
    public bool progressLosMor = false; //If on then the global morality right now is on a loss

    [Header("Villager leave by low morality")]
    public float leaveMorReq = 0.25f;
    public float minLeaveChance = 0.2f;
    public float maxLeaveChance = 0.4f; //The lesser the morality, the more villagers leave
    public float leaveCooldown = 25f; //25s makes 8 checks in a 5 minute duration awake day (200 seconds from 300)

    [Header("Loss by population #2")] //Checked by VillagerAI.cs upon death
    public float lossPopulationper = 0.25f; //percentage of max population before losing #2

    [Header("Loss by sickness #3")] //Checked by vilagerHealth upon Infect()
    public float lossSickPer = 1f; //% of sick vilagers of max population before losing #3 

    [Header("Visual")]
    public GameObject loseWindow;
    public TextMeshProUGUI deathInfo;
    public string[] moralityInfo;
    public string[] populationInfo;
    public string[] sicknessInfo;

    public bool lost;

    [Header("Other")]
    public Window[] windows;

    Coroutine lossPopulation;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        loseWindow.SetActive(false);
        CheckLossPopulation();
        CheckLossCondition();
    }

    public void CheckLossPopulation()
    {
        if(TownStorage.instance.globalMorality <= leaveMorReq && lossPopulation == null)
        {
            lossPopulation = StartCoroutine(LossPopulation());
        }
    }

    public void CheckLossCondition()
    {
        CheckLossByMorality();
        bool lossByMorality = LossByMorality();
        bool lossByPopulation = LossByPopulation();
        bool lossBySickness = LossBySickness();

        

        bool loss = lossByMorality || lossByPopulation || lossBySickness;

        if(loss)
        {
            LoseType loseType = LoseType.Morality;

            if(lossByMorality) loseType = LoseType.Morality;
            if(lossByPopulation) loseType = LoseType.Population;
            if(lossBySickness) loseType = LoseType.Sickness;

            Lose(loseType);
        }
    }

    public void Lose(LoseType loseType)
    {
        lost = true;
        foreach(var window in windows)
        {
            window.CloseWindow();
        }
        BuildSystem.instance.StopBuilding();
        if(TownManager.instance.availableMarket != null) TownManager.instance.availableMarket.HideVisuals();

        string info = "";

        switch(loseType)
        {
            case LoseType.Morality:
                info = moralityInfo[Random.Range(0, moralityInfo.Length)];
                break;

            case LoseType.Population:
                info = populationInfo[Random.Range(0, populationInfo.Length)];
                break;

            case LoseType.Sickness:
                info = sicknessInfo[Random.Range(0, sicknessInfo.Length)];
                break;
        }

        deathInfo.text = info;
        LensDistortWarp.instance.LoseSequence();
    }

    //Loss by morality #1
    public void CheckLossByMorality()
    {
        if(TownStorage.instance.globalMorality <= lowMorReq && !progressLosMor)
        {
            currentDays = lowMorDays;
            progressLosMor = true;
        }
        else if(TownStorage.instance.globalMorality > lowMorReq && progressLosMor)
        {
            currentDays = 0;
            progressLosMor = false;
        }
    }

    public void DecreaseLossMorality()
    {
        if(!progressLosMor) return;

        currentDays--;

        if(currentDays < 0) currentDays = 0;
    }

    public bool LossByMorality()
    {
        if(!progressLosMor) return false;

        return currentDays <= 0;
    }

    //Loss by population
    public bool LossByPopulation()
    {
        int maxPopulation = TownManager.instance.totalDead + TownManager.instance.villagers.Count;
        float currentPopulationPer = TownManager.instance.villagers.Count / (float)maxPopulation;
        return currentPopulationPer <= lossPopulationper;
    }

    //Loss by sickness
    public bool LossBySickness()
    {
        int healthyVillagers = 0;
        int sickVillagers = 0;

        foreach(var villager in TownManager.instance.villagers)
        {
            if(villager.villagerHealth.health == Health.Healthy)
            {
                healthyVillagers++;
            }
            else
            {
                sickVillagers++;
            }
        }

        int totalLiving = healthyVillagers + sickVillagers;

        if (totalLiving == 0) return false;

        float sickPer = (float)sickVillagers / totalLiving;

        return sickPer >= lossSickPer;
    }

    public void RandomVillagerDeparture()
    {
        if(TownManager.instance.villagers.Count == 0) return;

        VillagerAI randomVillager = TownManager.instance.villagers[Random.Range(0, TownManager.instance.villagers.Count)];

        randomVillager.Death(false);
    }

    IEnumerator LossPopulation()
    {
        while(true)
        {
            bool awakeHours = DayCycle.instance.hours >= TownManager.instance.hourAwakeReq && DayCycle.instance.hours < TownManager.instance.hourSleepReq;
            if(!awakeHours || TownManager.instance.villagers.Count == 0) break;

            yield return new WaitForSeconds(leaveCooldown);

            Debug.Log("Checking leaving Chance");

            float percentage = Mathf.InverseLerp(leaveMorReq, 0f, TownStorage.instance.globalMorality);

            float chance = Mathf.Lerp(minLeaveChance, maxLeaveChance, percentage);
            
            if(Random.value <= chance)
            {
                RandomVillagerDeparture();
            }

            yield return null;
        }
    }

}