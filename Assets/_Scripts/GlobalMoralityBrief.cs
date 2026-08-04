using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalMoralityBrief : MonoBehaviour
{
    public Image itself;
    public Sprite selected;
    public Sprite idle;

    public GameObject window;
    public bool isOpen;

    [Header("Other windows to close")]
    public Window[] otherWindows;

    [Header("Visual")]
    public TextMeshProUGUI fedValue;
    public TextMeshProUGUI workingValue;
    public TextMeshProUGUI homelessValue;
    public TextMeshProUGUI starvingValue;
    public TextMeshProUGUI netChangeValue;

    void Start()
    {
        isOpen = false;
    }

    public void Select()
    {
        if(isOpen) return;

        itself.sprite = selected;
    }

    public void Deselect()
    {
        if(isOpen) return;

        itself.sprite = idle;
    }

    public void BothBrief()
    {
        isOpen = !isOpen;

        if(isOpen)
        {
            OpenBrief();
        }
        else
        {
            CloseBrief();
        }
    }
    
    public void CloseBrief()
    {
        ActiveWindow.instance.briefActive = false;
        itself.sprite = idle;
        isOpen = false;
        window.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OpenBrief()
    {
        ActiveWindow.instance.briefActive = true;

        foreach(var other in otherWindows)
        {
            other.CloseWindow();
        }
        BuildSystem.instance.StopBuilding();

        isOpen = true;
        itself.sprite = selected;
        window.SetActive(true);
    }

    public void UpdateValues(int totalFed, int totalWorking, int totalHomeless, int totalStarving, float netDailyChange, float starvationRatio, float starvationPenalty)
    {
        float fedContrib = Mathf.Floor(TownStorage.instance.fedMultiplyer * totalFed * 100f) / 100f;
        string fedStr = fedContrib > 0 ? $"+{fedContrib:0.00}" : $"{fedContrib:0.00}";

        float workingContrib = Mathf.Floor(TownStorage.instance.workingMultiplyer * totalWorking * 100f) / 100f;
        string workingStr = workingContrib > 0 ? $"+{workingContrib:0.00}" : $"{workingContrib:0.00}";

        float homelessContrib = Mathf.Floor(TownStorage.instance.homelessMultiplyer * totalHomeless * 100f) / 100f;
        string homelessStr = homelessContrib > 0 ? $"-{homelessContrib:0.00}" : $"{homelessContrib:0.00}";

        float flooredStarvation = Mathf.Floor(starvationPenalty * 100f) / 100f;
        string starvingStr = flooredStarvation > 0 ? $"-{flooredStarvation:0.00}" : $"{flooredStarvation:0.00}";

        string netChangeStr = netDailyChange > 0 ? $"+{netDailyChange:0.00}" : $"{netDailyChange:0.00}";

        float oldGlobalMorality = TownStorage.instance.globalMorality - netDailyChange;

        fedValue.text = $"{totalFed} ({fedStr})";
        workingValue.text = $"{totalWorking} ({workingStr})";
        homelessValue.text = $"{totalHomeless} ({homelessStr})";
        starvingValue.text = $"{totalStarving} ({starvingStr})";
        
        netChangeValue.text = $"Net Change: {netChangeStr}\n({oldGlobalMorality * 100f:F0}% > {TownStorage.instance.globalMorality * 100f:F0}%)";
    }
}