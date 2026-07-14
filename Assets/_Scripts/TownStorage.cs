using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class TownStorage : MonoBehaviour
{
    public static TownStorage instance {get; private set;}

    public int money;

    [Header("Resources")]
    public float wheat;
    public int iron;
    public int copper;
    public int quartz;
    public int titanium;

    [Header("Resources Visuals")]
    public TextMeshProUGUI moneyVisual;
    public TextMeshProUGUI wheatVisual;
    public TextMeshProUGUI ironVisual;
    public TextMeshProUGUI copperVisual;
    public TextMeshProUGUI quartzVisual;
    public TextMeshProUGUI titaniumVisual;

    [Header("Global Morality")]
    public float globalMorality;
    public bool hasCheckedTomorrow;

    [Header("Requirements")]
    public float fedReq = 0.8f;
    public float starvingReq = 0.1f;

    [Header("Increasing Multiplyers")]
    public float fedMultiplyer = 0.02f;
    public float workingMultiplyer = 0.03f;

    [Header("Decreasing Multiplyers by absolute value")]
    public float homelessMultiplyer = 0.05f;
    public float starvingMultiplyer = 0.30f;

    [Header("Arrival of Villagers at awake times")]
    public float waitTime = 60f;
    public float scaleFactorPerPopulation = 15f;

    private TownManager townManager;
    private Coroutine currentVillagerCooldown;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        townManager = TownManager.instance;
    }    

    public void StartVillagerCooldown()
    {
        if(currentVillagerCooldown != null)
        {
            StopCoroutine(currentVillagerCooldown);
        }

        currentVillagerCooldown = StartCoroutine(VillagerSpawnCooldown());
    }

    public void StopVillagerCooldown()
    {
        if(currentVillagerCooldown == null)
        {
            return;
        }

        StopCoroutine(currentVillagerCooldown);
    }

    public void CalculateTomorrowMorality()
    {
        if(hasCheckedTomorrow) return;

        int totalPopulation = townManager.villagers.Count;
        int totalFed = 0;
        int totalWorking = 0;
        int totalHomeless = 0;
        int totalStarving = 0;
        float netDailyChange = 0f;

        foreach(var villager in townManager.villagers)
        {
            if(villager.hunger <= starvingReq) //If starving, then even working or housed won't add to the morality
            {
                totalStarving++;
                continue;
            }

            if(villager.house != null && villager.hunger >= fedReq)
            {
                totalFed++;
            }

            if(villager.jobPlace != null)
            {
                totalWorking++;
            }

            if(villager.house == null)
            {
                totalHomeless++;
            }
        }

        float starvationRatio = (float)totalStarving / (float)totalPopulation;
        float starvationPenalty = starvingMultiplyer * starvationRatio;

        netDailyChange = (fedMultiplyer * totalFed) + (workingMultiplyer * totalWorking) - (homelessMultiplyer * totalHomeless) - starvationPenalty;
        globalMorality = Mathf.Clamp01(globalMorality + netDailyChange);

        Debug.Log("Population: " + totalPopulation + "\nFed: " + totalFed + "\nWorking: " + totalWorking + "\nHomeless: " + totalHomeless + "\nStarving: " + totalStarving + 
        "\n\nNetDailyChange: " + netDailyChange + "\nNew Global Morality: " + globalMorality);
        hasCheckedTomorrow = true;
    }

    IEnumerator VillagerSpawnCooldown()
    {
        while(true)
        {
            int totalPopulation = townManager.villagers.Count;
            float cooldown = (waitTime + (totalPopulation * scaleFactorPerPopulation)) * (2 - globalMorality);
            float spawnChance = Mathf.Pow(globalMorality, 2f);

            // Debug.Log(cooldown);

            yield return new WaitForSeconds(cooldown);

            Debug.Log("Checking Chance");

            float randomChance = Random.Range(0, 1f);

            Debug.Log(randomChance + "/" + spawnChance);

            if(randomChance <= spawnChance)
            {
                Gate.instance.SpawnNewVillager();
            }

            yield return null;
        }
    }

    public void AddToInventoryID(int id, float amount)
    {
        switch(id)
        {
            case 0:
                wheat += amount;
                break;
            case 1:
                iron += (int)amount;
                break;
            case 2:
                copper += (int)amount;
                break;
            case 3:
                quartz += (int)amount;
                break;
            case 4:
                titanium += (int)amount;
                break;
        }

        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        moneyVisual.text = "Money: $" + money.ToString();
        wheatVisual.text = "Wheat: " + wheat.ToString("F2");
        ironVisual.text = "Iron: " + iron.ToString();
        copperVisual.text = "Copper: " + copper.ToString();
        quartzVisual.text = "Quartz: " + quartz.ToString();
        titaniumVisual.text = "Titanium: " + titanium.ToString();
    }
}
