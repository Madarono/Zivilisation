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
    Wheat,
    Iron,
    Copper,
    Quartz,
    Titanium,
    DoubleAll,
    TripleAll
}

public enum VirusTrait 
{
    None,
    Airborne, //Infection 2x when villager lingers close to a 1-tile radius
    Overcrowding, //Infection 2x when sleeping with other villager inside homes
    Coughing, //Stop pathfinding to cough, can infect if anyone close by a higher percentage than airborne
    Exhaustion_Insomnia, //Break schedule from sleep and/or work
    Sudden_Collapse //Can die randomly and leave an infected corpse, need to be disposed of quickly or infection 3x around it
}

public class VirusManager : MonoBehaviour
{
    public List<Virus> viruses = new List<Virus>();

    [Header("Resistance Weight")]
    public VirusResistanceWeight[] virusResistanceWeights;

    [Header("Trait Weight")]
    public VirusTraitWeight[] virusTraitWeights;

    [Header("Make new Virus")]
    [Tooltip("Higher values make powerful viruses more common and more traits more common.")]
    [ContextMenuItem("Generate Random Virus", "MakeNewVirus")]
    public float power = 0.5f; 
    public float trait1Power = 0.5f;
    public float trait2Power = 0.25f;
    public float trait3Power = 0.2f;

    public void MakeNewVirus()
    {
        Virus newVirus = new Virus();
        float rarityMultiplyer = 1.0f - Mathf.Pow(UnityEngine.Random.value, power);

        float virusInfection = UnityEngine.Random.value * 100 * rarityMultiplyer;
        float virusSeverity = UnityEngine.Random.value * 100 * rarityMultiplyer;
        float virusLethality = UnityEngine.Random.value * 100 * rarityMultiplyer;
        VirusResistance virusResistance = GetRandomResistence();

        float trait1Multiplyer = 1.0f - Mathf.Pow(UnityEngine.Random.value, trait1Power * power);
        VirusTrait trait1 = VirusTrait.None;

        float trait2Multiplyer = 1.0f - Mathf.Pow(UnityEngine.Random.value, trait2Power * power);
        VirusTrait trait2 = VirusTrait.None;

        float trait3Multiplyer = 1.0f - Mathf.Pow(UnityEngine.Random.value, trait3Power * power);
        VirusTrait trait3 = VirusTrait.None;

        if(UnityEngine.Random.value <= trait1Multiplyer)
        {
            trait1 = GetRandomTrait(newVirus);
            newVirus.trait1 = trait1;

            if(UnityEngine.Random.value <= trait2Multiplyer)
            {
                trait2 = GetRandomTrait(newVirus);
                newVirus.trait2 = trait2;

                if(UnityEngine.Random.value <= trait3Multiplyer)
                {
                    trait3 = GetRandomTrait(newVirus);
                    newVirus.trait3 = trait3;
                }
            }
        }


        newVirus.infection = Mathf.Clamp(Mathf.RoundToInt(virusInfection), 0, 100);
        newVirus.severity = Mathf.Clamp(Mathf.RoundToInt(virusSeverity), 0, 100);
        newVirus.lethality = Mathf.Clamp(Mathf.RoundToInt(virusLethality), 0, 100);
        newVirus.resistanceType = virusResistance;

        viruses.Add(newVirus);
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
            bool isAlreadyAssigned = (currentVirus.trait1 == item.trait || currentVirus.trait2 == item.trait || currentVirus.trait3 == item.trait);
            
            if (!isAlreadyAssigned)
            {
                availableWeights.Add(item);
            }
        }

        if (availableWeights.Count == 0) 
        {
            return VirusTrait.None;
        }

        float totalWeight = 0;
        foreach (var item in availableWeights) totalWeight += item.weight;

        float roll = UnityEngine.Random.value * totalWeight;
        float cumulative = 0;

        foreach (var item in availableWeights)
        {
            cumulative += item.weight;
            if (roll <= cumulative) return item.trait;
        }

        return availableWeights[availableWeights.Count - 1].trait;
    }

}