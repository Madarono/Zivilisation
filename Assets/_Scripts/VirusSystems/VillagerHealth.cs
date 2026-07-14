using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum Health 
{
    Healthy,
    Incubation,
    Infected 
}

public class VillagerHealth : MonoBehaviour //ToDo: Infection that makes Infections and Severity slows down movement speed and work speed
{
    public VillagerAI villager;
    public Health health;

    [Header("<size=15>--- Incubation or Infected ---</size>")]
    public Virus inflictedVirus;
    public SpriteRenderer rend;
    public int daysLeft = 0;
    public Color[] infectedStates = new Color[3];

    [Line]

    [Header("<size=15>--- Infection ---</size>")]
    public LayerMask villagerLayer;
    public float infectionCooldown = 5f;
    public float tileRange = 1;

    [Space(10)]
    [Header("Airborne")]
    public float airborneBonus = 1; //Bonus Tile for Airborne Trait
    public float airborneInfectionMultiplyer = 1.5f;

    [Space(10)]
    [Header("Overcrowding")]
    public float overcrowdingMultiplyer = 0.2f; //Overcrowding multiplyer for infecting inside of households, must be low

    [Line]

    [Header("<size=15>--- Severity ---</size>")]
    public float functionSpeed = 1f;
    public float minFunction = 0.75f; //Function: Walk and Work
    
    [Space(10)]
    [Header("Muscle Atrophy")]
    public float minMuscleFunction = 0.25f; //When trait is muscleAtrophy;
    public float muscleSeverityMultiplyer = 1.5f;

    [Line]
    [Header("HyperMetabolism")]
    public float hungerMultiplyer = 2f; //Hunger depletion multiplyer when having this trait
    public float currentHungerMultiplyer = 1f;

    Coroutine infectionCoroutine;

    void Start()
    {
        UpdateVisuals();
    }

    public void Inflict(Virus virus, int days)
    {
        if(health != Health.Healthy) return;

        inflictedVirus = virus;
        health = Health.Incubation;
        daysLeft = days;
        UpdateVisuals();
    } 

    public void Infect(Virus virus)
    {
        inflictedVirus = virus;
        health = Health.Infected;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        switch(health)
        {
            case Health.Healthy:
                rend.color = infectedStates[0];
                break;
            
            case Health.Incubation:
                rend.color = infectedStates[1];
                break;

            case Health.Infected:
                rend.color = infectedStates[2];
                break;
        }

        if(health == Health.Infected && infectionCoroutine == null)
        {
            infectionCoroutine = StartCoroutine(Infection());
        }

        if(health == Health.Infected)
        {
            bool muscleAtrophy = HasCertainTrait(VirusTrait.MuscleAtrophy);
            float minFunc = muscleAtrophy ? minMuscleFunction : minFunction;
            float finalSeverity = Mathf.Min(muscleAtrophy ? inflictedVirus.severity * muscleSeverityMultiplyer : inflictedVirus.severity, 100f);

            Debug.Log(muscleAtrophy);
            Debug.Log(minFunc);

            functionSpeed = Mathf.Lerp(1f, minFunc, finalSeverity / 100f);

            currentHungerMultiplyer = HasCertainTrait(VirusTrait.HyperMetabolism) ? hungerMultiplyer : 1f;
        }
        else
        {
            functionSpeed = 1f;
        }
    }

    public void CheckIncubation()
    {
        if(daysLeft > 0) return;

        Infect(inflictedVirus);
    }

    public void ReduceDays()
    {
        daysLeft--;
        CheckIncubation();
    }

    IEnumerator Infection()
    {
        float infectionMultiplyer = HasCertainTrait(VirusTrait.Airborne) ? airborneInfectionMultiplyer : 1f;
        float infectionChance = Mathf.Min(inflictedVirus.infection * infectionMultiplyer, 100f);
        float overcrowdingChance = inflictedVirus.infection * overcrowdingMultiplyer; 
        float range = HasCertainTrait(VirusTrait.Airborne) ? tileRange + airborneBonus : tileRange;
        List<VillagerAI> villagers = new List<VillagerAI>();

        Vector2 boxSize = new Vector2(range, range);

        while(true)
        {
            yield return new WaitForSeconds(infectionCooldown);
            if(villager.state == VillagerState.Sleeping && !HasCertainTrait(VirusTrait.Overcrowding)) continue; //Has no Overcrowding Tait
            else if(villager.state == VillagerState.Sleeping && HasCertainTrait(VirusTrait.Overcrowding))
            {
                float householdChance = Random.Range(0, 100f);
                Debug.Log("Infection household attempt");

                if(householdChance > overcrowdingChance)
                {
                    continue;
                }


                if(villager.house.gameObject.TryGetComponent(out Building building))
                {
                    villagers.Clear();
                    foreach(var villagerBuilding in building.villagers)
                    {
                        if(villagerBuilding == this.villager || villagerBuilding.villagerHealth.health == Health.Healthy) continue;

                        villagers.Add(villagerBuilding);
                    }

                    VillagerAI randomVillager = villagers[Random.Range(0, villagers.Count)];
                    randomVillager.villagerHealth.Infect(inflictedVirus);
                    Debug.Log("Infected in household");
                }
                continue;
            }

            //Normal infection on the streets
            float chance = Random.Range(0, 100f);

            if(chance <= infectionChance)
            {
                Collider2D hit = Physics2D.OverlapBox(transform.position, boxSize, 0f, villagerLayer);

                if(hit != null)
                {
                    if(hit.gameObject.TryGetComponent(out VillagerAI villager))
                    {
                        villager.villagerHealth.Infect(inflictedVirus); //Infecting others
                    }
                }
            }
        }
    }

    bool HasCertainTrait(VirusTrait trait)
    {
        if (inflictedVirus == null) return false;
        
        return inflictedVirus.trait1 == trait || inflictedVirus.trait2 == trait || inflictedVirus.trait3 == trait;
    }
}