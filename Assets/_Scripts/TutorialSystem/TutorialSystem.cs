using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum TutorialStep
{
    AssignMotel, //Done
    BuildFarm, //Done
    BuildMotel, //Done
    BuildMines, //Done
    AssignWorkers, //Done
    BuildMarket, //Done
    SellInMarket, //Done
    BuildRoads, //Done
    BuildQuarantine, //Done
    BuildLaboratory, //Done
    CureVirus, //Done
    VaccinateVirus,
}

[System.Serializable]
public class TutorialObjective
{
    public TutorialStep step;
    public TutorialSO stepSO;
    public string visual;
    public bool done;
}

public class TutorialSystem : MonoBehaviour
{
    public static TutorialSystem instance {get; private set;}

    [Header("Tutorials")]
    public GameObject tutorialVisual;
    public GameObject denyButton;
    public TutorialObjective[] objectives;
    public TutorialSO currentTutorial;
    public int currentStep;
    public int objectivesId = -1;
    public bool isActive;

    [Header("Selection Visual")]
    public RectTransform selectionVisual;
    public float timeToMove = 1f;

    Coroutine currentAnim;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        tutorialVisual.SetActive(false);
        denyButton.SetActive(false);
        selectionVisual.gameObject.SetActive(false);
    }

    public void DoTutorial(TutorialStep step)
    {
        for(int i = 0; i < objectives.Length; i++)
        {
            if(objectives[i].step == step)
            {
                currentTutorial = objectives[i].stepSO;
                isActive = true;
                currentStep = 0;

                if(currentTutorial.buildingName == "Market" && TownManager.instance.availableMarket != null)
                {
                    isActive = false;
                    PopupText.instance.Popup("Can't show tutorial, you have built this building, and only one can be built at a time.");
                    StopTutorial();
                    return;
                }

                if(currentTutorial.type == TutorialType.SellInMarket && TownManager.instance.availableMarket == null)
                {
                    isActive = false;
                    PopupText.instance.Popup("Must have a market placed to proceed the tutorial.");
                    StopTutorial();
                    return;
                }

                if(currentTutorial.type == TutorialType.Build && BuildOptions.instance.item != null && BuildOptions.instance.item.name != currentTutorial.buildingName)
                {
                    isActive = false;
                    PopupText.instance.Popup("Can't show tutorial, you are currently placing a building.");
                    StopTutorial();
                    return;
                }

                //CureVirus
                if(currentTutorial.type == TutorialType.CureVirus && TownManager.instance.availableQuarantine == null)
                {
                    isActive = false;
                    PopupText.instance.Popup("Can't show tutorial, you need a Quarantine");
                    StopTutorial();
                    return;
                }

                if(currentTutorial.type == TutorialType.CureVirus && TownManager.instance.availableLaboratory == null)
                {
                    isActive = false;
                    PopupText.instance.Popup("Can't show tutorial, you need a Laboratory");
                    StopTutorial();
                    return;
                }

                if(currentTutorial.type == TutorialType.CureVirus && TownManager.instance.availableQuarantine.villagers.Count == 0)
                {
                    isActive = false;
                    PopupText.instance.Popup("Can't show tutorial, you need to quarantine an ill villager");
                    StopTutorial();
                    return;
                }

                //Building Step Skip
                if(currentTutorial.type == TutorialType.Build && TownManager.instance.isBuilding && !BuildOptions.instance.isOpen)
                {
                    currentStep = 1;
                }
                else if(currentTutorial.type == TutorialType.Build && TownManager.instance.isBuilding && BuildOptions.instance.isOpen)
                {
                    currentStep = 2;
                }
                
                if(currentTutorial.type == TutorialType.Build && BuildOptions.instance.item != null && BuildOptions.instance.item.name == currentTutorial.buildingName)
                {
                    currentStep = 3;
                }

                //Assign Building Step Skip
                if(TownManager.instance.currentBuilding != null)
                {
                    Building buildingScript = TownManager.instance.currentBuilding as Building;
                    Farm farmScript = TownManager.instance.currentBuilding as Farm;
                    Mines minesScript = TownManager.instance.currentBuilding as Mines;

                    if(currentTutorial.type == TutorialType.AssignVillagers && buildingScript != null && farmScript == null && minesScript == null && TownManager.instance.currentBuilding.isShowing)
                    {
                        currentStep = 1;
                    }
                    if(currentTutorial.type == TutorialType.AssignVillagers && buildingScript != null && farmScript == null && minesScript == null && TownManager.instance.currentBuilding.isChoosing)
                    {
                        currentStep = 2;
                    }

                    if(currentTutorial.type == TutorialType.AssignWorkers && buildingScript != null && (farmScript != null || minesScript != null) && TownManager.instance.currentBuilding.isShowing)
                    {
                        currentStep = 1;
                    }
                    if(currentTutorial.type == TutorialType.AssignWorkers && buildingScript != null && (farmScript != null || minesScript != null) && TownManager.instance.currentBuilding.isChoosing)
                    {
                        currentStep = 2;
                    }
                }

                //SellInMarket Step Skip
                if(currentTutorial.type == TutorialType.SellInMarket && TownManager.instance.availableMarket.isShowing)
                {
                    currentStep = 1;
                }

                //BuildRoad Step Skip
                if(currentTutorial.type == TutorialType.BuildRoad && TownManager.instance.isBuilding && !BuildOptions.instance.isOpen)
                {
                    currentStep = 1;
                }

                if(currentTutorial.type == TutorialType.BuildRoad && RoadSystem.instance.isActive)
                {
                    currentStep = 2;
                    RoadSystem.instance.StopMultiBrushMode();
                    RoadSystem.instance.StopShovelMode();
                }

                //CureVirus Step Skip
                if(currentTutorial.type == TutorialType.CureVirus && TownManager.instance.availableLaboratory.isShowing)
                {
                    currentStep = 1;
                }

                objectivesId = i;
                if(currentStep == 0) PopupText.instance.Popup(currentTutorial.startingPopup, true, true);
                else PopupText.instance.Popup(currentTutorial.steps[currentStep - 1].popupTextVisual, true, true);
                tutorialVisual.SetActive(true);
                denyButton.SetActive(true);

                if(!currentTutorial.showSelection && currentTutorial.type != TutorialType.SellInMarket && currentTutorial.type != TutorialType.CureVirus)
                {
                    selectionVisual.gameObject.SetActive(false);
                    return;
                }

                if(currentAnim != null) StopCoroutine(currentAnim);

                if(currentStep == 0) currentAnim = StartCoroutine(AnimateMove(currentTutorial.position, currentTutorial.size, true));
                else currentAnim = StartCoroutine(AnimateMove(currentTutorial.steps[currentStep - 1].position, currentTutorial.steps[currentStep - 1].size, true, currentTutorial.steps[currentStep - 1].showSelection));

                break;
            }
        }
    }

    public void NotifyTutorial(string scriptName, string functionName)
    {
        if(!isActive) 
        {
            CheckCongratsWithoutTutorial(scriptName, functionName); //To Complete Objectives even without using the tutorial
            return;
        }

        if(currentTutorial.congratsStep.scriptName == scriptName && currentTutorial.congratsStep.functionName == functionName)
        {
            isActive = false;
            objectives[objectivesId].done = true;
            ObjectiveSystem.instance.UpdateObjectives();
            PopupText.instance.Popup(currentTutorial.congratsStep.popupTextVisual);
            tutorialVisual.SetActive(false);
            denyButton.SetActive(false);
            selectionVisual.gameObject.SetActive(false);
            currentTutorial = null;
            DataPersistenceManager.instance.SaveGame();
            return;
        }

        bool failTutorial = CheckFailTutorial(scriptName, functionName);
        
        if(failTutorial || currentStep >= currentTutorial.steps.Length) return;

        if(currentTutorial.steps[currentStep].scriptName == scriptName && currentTutorial.steps[currentStep].functionName == functionName)
        {
            PopupText.instance.Popup(currentTutorial.steps[currentStep].popupTextVisual, true, true);

            if(!currentTutorial.steps[currentStep].showSelection)
            {
                selectionVisual.gameObject.SetActive(false);
                currentStep++;
                return;
            }

            if(currentAnim != null) StopCoroutine(currentAnim);

            currentAnim = StartCoroutine(AnimateMove(currentTutorial.steps[currentStep].position, currentTutorial.steps[currentStep].size, false));
            
            currentStep++;
        }
    }
    
    //This is to complete the Objective even without the Tutorial when !isActive
    public void CheckCongratsWithoutTutorial(string scriptName, string functionName)
    {
        if(isActive) return;

        foreach(var objective in objectives)
        {
            if(objective.stepSO.congratsStep.scriptName == scriptName && objective.stepSO.congratsStep.functionName == functionName)
            {
                objective.done = true;
                ObjectiveSystem.instance.UpdateObjectives();
                DataPersistenceManager.instance.SaveGame();
                break;
            }
        }
    }

    //Specific for FailSave when choosing another building while isActive
    public void CheckBuilding(string buildingName)
    {
        if((currentTutorial != null && currentTutorial.buildingName == "") || !isActive) return;

        if(buildingName != currentTutorial.buildingName)
        {
            isActive = false;
            PopupText.instance.Popup(currentTutorial.buildingPopupFail);
            tutorialVisual.SetActive(false);
            denyButton.SetActive(false);
            selectionVisual.gameObject.SetActive(false);
            currentTutorial = null;
        }
    }

    //Checking any failed situations in a List
    bool CheckFailTutorial(string scriptName, string functionName)
    {
        bool failTutorial = false;

        if(currentTutorial.failTutorial.Length == 0) return false;

        foreach(var fail in currentTutorial.failTutorial)
        {
            if(fail.scriptName == scriptName && fail.functionName == functionName)
            {
                isActive = false;
                PopupText.instance.Popup(fail.popupTextVisual);
                tutorialVisual.SetActive(false);
                denyButton.SetActive(false);
                selectionVisual.gameObject.SetActive(false);
                currentTutorial = null;
                failTutorial = true;
                break;
            }
        }

        return failTutorial;
    }

    public void StopTutorial()
    {
        if(!isActive) return;

        isActive = false;
        currentTutorial = null;
        denyButton.SetActive(false);
        tutorialVisual.SetActive(false);
        selectionVisual.gameObject.SetActive(false);
        PopupText.instance.StopPopup();
    }

    IEnumerator AnimateMove(Vector2 finalPos, Vector2 finalSize, bool instant, bool show = true)
    {
        selectionVisual.gameObject.SetActive(show);
        Vector2 transitionPos = selectionVisual.anchoredPosition;
        Vector2 transitionSize = selectionVisual.sizeDelta;
        float t = 0;

        if(instant)
        {
            selectionVisual.anchoredPosition = finalPos;
            selectionVisual.sizeDelta = finalSize;

            currentAnim = null;
            yield break;
        }

        while(t < timeToMove)
        {
            t += Time.unscaledDeltaTime;
            transitionPos = Vector2.Lerp(transitionPos, finalPos, t * timeToMove);
            transitionSize = Vector2.Lerp(transitionSize, finalSize, t * timeToMove);

            selectionVisual.anchoredPosition = transitionPos;
            selectionVisual.sizeDelta = transitionSize;
            yield return null;
        }

        selectionVisual.anchoredPosition = finalPos;
        selectionVisual.sizeDelta = finalSize;

        currentAnim = null;
    }
}