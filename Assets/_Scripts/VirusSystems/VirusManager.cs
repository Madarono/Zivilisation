using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class Virus 
{
    //All go from 0 till 100
    public int infection;
    public int severity;
    public int lethality;

    public VirusResistance resistanceType;
    public VirusTrait trait1;
    public VirusTrait trait2;
    public VirusTrait trait3;
}

[System.Serializable]
public class VirusResistanceWeight
{
    public VirusResistance resistance;
    public float weight;
}

[System.Serializable]
public class VirusTraitWeight
{
    public VirusTrait trait;
    public float weight;
}


public enum VirusResistance
{
    None,
    OneQuarterX,
    OneHalfX,
    TwoX,
    ThreeX
}

public enum VirusTrait 
{
    None,
    Airborne, //1.5x Infection and +1 Tilerange when infecting
    Overcrowding, //Infection 2x when sleeping with other villager inside homes
    MuscleAtrophy, //1.5x Severity and minFunction goes as low as 0.25f and not just 0.75f
    HyperMetabolism, //2x Hunger decrease
    Coughing, //Stop pathfinding to cough, can infect if anyone close by a higher percentage than airborne
    ExhaustionInsomnia, //Break schedule from sleep and/or work
    SuddenCollapse //Can die randomly and leave an infected corpse, need to be disposed of quickly or infection 2x around it
}

public class VirusManager : MonoBehaviour
{
    public static VirusManager instance {get; private set;}

    private TownManager townManager;
    private DayCycle dayCycle;

    public List<Virus> viruses = new List<Virus>();

    [Header("Resistance Weight")]
    public VirusResistanceWeight[] virusResistanceWeights;

    [Header("Trait Weight")]
    public VirusTraitWeight[] virusTraitWeights;

    [Header("Infect Chance")]
    public float gateVillagerChance = 5f;
    public float randomVillagerChance = 10f;
    public int[] villagerInfectTimes;

    private Coroutine randomInfect;

    [Header("Make new Virus")]
    [Tooltip("Higher values make powerful viruses more common and more traits more common.")]
    [ContextMenuItem("Generate Random Virus", "MakeNewVirus")]
    public float power = 0.5f; 
    public float trait1Power = 0.5f;
    public float trait2Power = 0.25f;
    public float trait3Power = 0.2f;
    public int incubationTimeInDays = 2;

    private List<VillagerAI> healthyVillagers = new List<VillagerAI>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        townManager = TownManager.instance;
        dayCycle = DayCycle.instance;
    }

    public void MakeNewVirus()
    {
        Virus newVirus = new Virus();
        float rarityMultiplyer = 1.0f - Mathf.Pow(UnityEngine.Random.value, power);

        float virusInfection = UnityEngine.Random.value * 100 * rarityMultiplyer;
        float virusSeverity = UnityEngine.Random.value * 100 * rarityMultiplyer;
        float virusLethality = UnityEngine.Random.value * 100 * rarityMultiplyer;
        VirusResistance virusResistance = GetRandomResistence();

        float trait1Multiplyer = 1.0f - Mathf.Pow(UnityEngine.Random.value, trait1Power * power);
        if (UnityEngine.Random.value <= trait1Multiplyer)
        {
            newVirus.trait1 = GetRandomTrait(newVirus);
        }

        float trait2Multiplyer = 1.0f - Mathf.Pow(UnityEngine.Random.value, trait2Power * power);
        if (UnityEngine.Random.value <= trait2Multiplyer)
        {
            newVirus.trait2 = GetRandomTrait(newVirus);
        }

        float trait3Multiplyer = 1.0f - Mathf.Pow(UnityEngine.Random.value, trait3Power * power);
        if (UnityEngine.Random.value <= trait3Multiplyer)
        {
            newVirus.trait3 = GetRandomTrait(newVirus);
        }

        newVirus.infection = Mathf.Clamp(Mathf.RoundToInt(virusInfection), 0, 100);
        newVirus.severity = Mathf.Clamp(Mathf.RoundToInt(virusSeverity), 0, 100);
        newVirus.lethality = Mathf.Clamp(Mathf.RoundToInt(virusLethality), 0, 100);
        newVirus.resistanceType = virusResistance;

        viruses.Add(newVirus);
    }

    public void InflictRandomVillager(bool newest)
    {
        if(viruses.Count == 0) return;
        
        Virus randomVirus = newest ? viruses[viruses.Count - 1] : viruses[Random.Range(0, viruses.Count)];

        GetHealthyVillagers();

        if(healthyVillagers.Count == 0) return;

        VillagerAI randomVillager = healthyVillagers[Random.Range(0, healthyVillagers.Count)];

        randomVillager.villagerHealth.Inflict(randomVirus, incubationTimeInDays);

        Debug.Log("Infected a villager");
    }

    public void InflictVillager(VillagerAI villager, bool newest)
    {
        if(viruses.Count == 0 || villager.villagerHealth.health != Health.Healthy) return;
        
        Virus randomVirus = newest ? viruses[viruses.Count - 1] : viruses[Random.Range(0, viruses.Count)];

        villager.villagerHealth.Inflict(randomVirus, incubationTimeInDays);
        PopupText.instance.Popup("A sick traveller has entered your villager. Monitor your villagers closely.");
    }


    [ContextMenu("Inflict Newest")]
    public void InflictNewest()
    {
        InflictRandomVillager(true);
    }

    [ContextMenu("Inflict Random")]
    public void InflictRandom()
    {
        InflictRandomVillager(false);
    }

    List<VillagerAI> GetHealthyVillagers()
    {
        healthyVillagers.Clear();

        foreach(var villager in TownManager.instance.villagers)
        {
            if(villager.villagerHealth.health == Health.Healthy)
            {
                healthyVillagers.Add(villager);
            }
        }

        if(healthyVillagers.Count == 0) return null;

        return healthyVillagers;
    }
    
    public VirusResistance GetRandomResistence()
    {
        float totalWeight = 0;
        foreach (var item in virusResistanceWeights) 
        {
            totalWeight += item.weight;
        }

        float roll = UnityEngine.Random.value * totalWeight;

        float cumulative = 0;
        foreach (var item in virusResistanceWeights)
        {
            cumulative += item.weight;
            if (roll <= cumulative) return item.resistance;
        }
        return VirusResistance.None;
    }

    public VirusTrait GetRandomTrait(Virus currentVirus)
    {
        List<VirusTraitWeight> availableWeights = new List<VirusTraitWeight>();

        foreach (var item in virusTraitWeights)
        {
            if (item.trait == VirusTrait.None) continue;

            bool isAlreadyAssigned = (currentVirus.trait1 == item.trait || currentVirus.trait2 == item.trait || currentVirus.trait3 == item.trait);

            if (!isAlreadyAssigned)
            {
                availableWeights.Add(item);
            }
        }

        if (availableWeights.Count == 0) return VirusTrait.None;

        float totalWeight = 0;
        foreach (var item in availableWeights) totalWeight += item.weight;

        if (totalWeight <= 0f) return VirusTrait.None;

        float roll = UnityEngine.Random.value * totalWeight;
        float cumulative = 0;

        foreach (var item in availableWeights)
        {
            cumulative += item.weight;
            if (roll <= cumulative) return item.trait;
        }

        return VirusTrait.None;
    }

    public void CheckInfect(bool gate = false, VillagerAI villager = null)
    {
        if(!CanInfect() && !gate) return;

        float chance = Random.Range(0, 100f);

        if (gate)
        {
            if (villager != null && chance <= gateVillagerChance)
            {
                MakeNewVirus();
                InflictVillager(villager, true);
            }
        }
        else
        {
            if (chance <= randomVillagerChance)
            {
                if(viruses.Count == 0) MakeNewVirus();

                InflictRandom();
            }
        }
    }

    bool CanInfect()
    {
        dayCycle = DayCycle.instance;
        foreach(var time in villagerInfectTimes)
        {
            if(dayCycle.hours == time)
            {
                return true;
            }
        }

        return false;
    }

}