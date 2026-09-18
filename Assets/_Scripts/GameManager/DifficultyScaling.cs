using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum Difficulty 
{
    Easy,
    Normal,
    Hard,
    Extreme
}

[System.Serializable]
public class DifficultyScale 
{
    public Difficulty difficulty;

    [Header("Virus Power")]
    public float virusPower = 1;

    [Header("Demand Market Power (how powerful are the good demands)")]
    public float minPower = 0.8f;
    public float maxPower = 2f;

    [Header("Demand Market Boundries (how low and how high are demand)")]
    public float maxDemand = 2f;
    public float minDemand = 0.5f;

    [Header("Shop Discount")]
    [Range(-1f,1f)] public float shopDiscount = 0f;

    [Header("Starting Cash")]
    public int startingCash = 100;

    [Header("Penalty Discount")]
    [Range(-1f,1f)] public float penaltyDiscount = 0f;
}

public class DifficultyScaling : MonoBehaviour
{
    public static DifficultyScaling instance {get; private set;}

    [Header("Normal Mode")]
    public DifficultyScale[] difficultyScales;

    [Header("Endless Mode")]
    public float dailyMultiplyer = 0.01f; //Multiplys every normal mode value by calculation every day
    public float maxMultiplyer = 4f; 

    void Awake()
    {
        instance = this;
    }
}