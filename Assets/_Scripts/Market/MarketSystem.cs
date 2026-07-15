using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class Demand 
{
    public Sprite icon;
    public float min;
    public float max;
}

public class MarketSystem : MonoBehaviour
{
    public static MarketSystem instance {get; private set;}
    [Header("Demand Power")]
    public float demandPower = 1f;
    public float minPower = 0.8f;
    public float maxPower = 2f;
    
    [Header("Demand")]
    public float maxDemand = 2f;
    public float minDemand = 0.5f;
    public float[] dailyDemand = new float[5];

    [Header("Visual")]
    public Demand[] demandIcons;
    public int[] iconsId = new int[5];

    void Awake()
    {
        instance = this;
    }

    [ContextMenu("RandomizeDemand")]
    public void RandomizeDemand()
    {
        demandPower = Random.Range(minPower, maxPower);
        for(int i = 0; i < dailyDemand.Length; i++)
        {
            float rarityMultiplyer = 1.0f - Mathf.Pow(UnityEngine.Random.value, demandPower);
            float randomDemand = Mathf.Lerp(minDemand, maxDemand, rarityMultiplyer);
            randomDemand = Mathf.FloorToInt(randomDemand * 10) / 10f;
            dailyDemand[i] = randomDemand;
            iconsId[i] = CheckDemandID(randomDemand);
        }
    }

    public void UpdateIconID()
    {
        for(int i = 0; i < iconsId.Length; i++)
        {
            iconsId[i] = CheckDemandID(dailyDemand[i]);
        }
    }

    int CheckDemandID(float demand)
    {
        for(int i = 0; i < demandIcons.Length; i++)
        {
            if(demand >= demandIcons[i].min && demand <= demandIcons[i].max)
            {
                return i;
            }
        }

        return 1; //For normal demand
    }
}