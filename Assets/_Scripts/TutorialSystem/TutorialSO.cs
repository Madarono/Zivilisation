using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum TutorialType
{
    Build,
    AssignVillagers,
    AssignWorkers,
    SellInMarket,
    BuildRoad,
    CureVirus
}

[System.Serializable]
public class FunctionStep
{
    public string scriptName;
    public string functionName;

    public string popupTextVisual;

    [Header("Show Selection")]
    public bool showSelection;
    public Vector2 position;
    public Vector2 size;
}

[CreateAssetMenu(fileName = "New Tutorial", menuName = "FeverFall/ Tutorial")]
public class TutorialSO : ScriptableObject
{
    public TutorialType type;

    [Header("Starting Popup")]
    public string startingPopup;

    [Header("Show Selection")]
    public bool showSelection;
    public Vector2 position;
    public Vector2 size;

    [Header("Steps")]
    public FunctionStep[] steps;

    [Header("Congrats")]
    public FunctionStep congratsStep;

    [Header("CancelTutorialForce")]
    public FunctionStep[] failTutorial;

    [Header("Fail By Building")]
    public string buildingName;
    public string buildingPopupFail;
}