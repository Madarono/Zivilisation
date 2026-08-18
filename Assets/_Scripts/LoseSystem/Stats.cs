using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stats : MonoBehaviour
{
    public static Stats instance {get; private set;}
    public GameObject window;

    [Header("Variables")]
    public int totalDays; //save
    public int peakPopulation;
    public float lowestMorality; //save
    public int desertions; //save
    public int totalViruses;
    public float infectionPer;
    public int totalSick; //save
    public int totalMoneyGained; //save

    [Header("Visuals")]
    public TextMeshProUGUI totalDaysVisual;
    public TextMeshProUGUI peakPopulationVisual;
    public TextMeshProUGUI lowestMoralityVisual;
    public TextMeshProUGUI desertionsVisual;
    public TextMeshProUGUI totalVirusesVisual;
    public TextMeshProUGUI infectionPerVisual;
    public TextMeshProUGUI totalSickVisual;
    public TextMeshProUGUI totalMoneyGainedVisual;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        CloseWindow();
    }

    public void OpenWindow()
    {
        window.SetActive(true);
        UpdateVisuals();
    }

    public void CloseWindow()
    {
        window.SetActive(false);
    }

    public void UpdateVisuals()
    {
        peakPopulation = TownManager.instance.villagers.Count + TownManager.instance.totalDead;
        infectionPer = SickPercentage();
        totalViruses = VirusManager.instance.viruses.Count;

        totalDaysVisual.text = totalDays.ToString();
        peakPopulationVisual.text = peakPopulation.ToString();
        lowestMoralityVisual.text = lowestMorality.ToString("F2") + "%";
        desertionsVisual.text = desertions.ToString();
        totalVirusesVisual.text = totalViruses.ToString();
        infectionPerVisual.text = infectionPer.ToString("F2") + "%";
        totalSickVisual.text = totalSick.ToString();
        totalMoneyGainedVisual.text = totalMoneyGained.ToString();
    }

    public float SickPercentage()
    {
        int healthyVillagers = 0;
        int sickVillagers = 0;

        foreach (var villager in TownManager.instance.villagers)
        {
            if (villager.villagerHealth.health == Health.Healthy)
            {
                healthyVillagers++;
            }
            else
            {
                sickVillagers++;
            }
        }

        int totalPopulation = healthyVillagers + sickVillagers;
        
        if (totalPopulation == 0) return 0f;

        return ((float)sickVillagers / totalPopulation) * 100f;
    }
}