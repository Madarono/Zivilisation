using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LaboratorySystem : MonoBehaviour
{
    public static LaboratorySystem instance { get; private set; }
    private ManualSystem manual;
    public Window manualWindow;

    [Header("Values")]
    public List<string> virusNames = new List<string>();
    public List<int> manualPage = new List<int>();
    public int virusId;

    [Header("Visuals")]
    public GameObject lookAheadWindow;
    public GameObject laboratoryWindow;
    public Slider infectionSlider;
    public Slider severitySlider;
    public Slider lethalitySlider;
    public TMP_InputField virusNameVisual;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        laboratoryWindow.SetActive(false);
        manual = ManualSystem.instance;
    }

    public void UpdateVisuals()
    {
        GetVirusId();
        EnsureListSizes(virusId);

        if (manualPage[virusId] == -1)
        {
            SetManualPage();
        }

        virusNameVisual.text = virusNames[virusId];
        manual.headerInfo[manualPage[virusId]] = virusNames[virusId];
    }

    private void EnsureListSizes(int targetId)
    {
        while (virusNames.Count <= targetId)
        {
            virusNames.Add($"Virus #{virusNames.Count}");
        }

        while (manualPage.Count <= targetId)
        {
            manualPage.Add(-1);
        }
    }

    public void ChangeVirusName()
    {
        virusNames[virusId] = virusNameVisual.text;
        manual.headerInfo[manualPage[virusId]] = virusNames[virusId];
        UpdateVisuals();
    }

    public void ChangeNotePage()
    {
        LookAhead.instance.OpenWindow(true);
    }

    public void SetManualPage()
    {
        EnsureListSizes(virusId);

        // Check if another virus is already occupying a page index
        for (int i = 0; i < manual.headerInfo.Count; i++)
        {
            bool isPageTakenByAnotherVirus = manualPage.Contains(i);
            bool isPageEmptyInManual = string.IsNullOrEmpty(manual.headerInfo[i]) && string.IsNullOrEmpty(manual.pageInfo[i]);

            if (!isPageTakenByAnotherVirus && isPageEmptyInManual)
            {
                manualPage[virusId] = i;
                manual.headerInfo[i] = virusNames[virusId]; 
                return;
            }
        }

        manual.MakeNewPage();
        manualPage[virusId] = manual.leftPageId;
        manual.headerInfo[manual.leftPageId] = virusNames[virusId];
    }

    public void SetNewPage(int newPage)
    {
        EnsureListSizes(virusId);

        if (manualPage[virusId] != -1 && manualPage[virusId] < manual.headerInfo.Count)
        {
            if (manual.headerInfo[manualPage[virusId]] == virusNames[virusId])
            {
                manual.headerInfo[manualPage[virusId]] = "";
            }
        }

        manualPage[virusId] = newPage;

        LookAhead.instance.CloseWindow();
        LookAhead.instance.SetValues(false);
        lookAheadWindow.SetActive(false);

        manual.headerInfo[manualPage[virusId]] = virusNames[virusId];
    }

    public void GoToManual()
    {
        TownManager.instance.availableLaboratory.HideVisuals();
        manualWindow.OpenWindow();
        LookAhead.instance.ChosePage(manualPage[virusId]);
    }

    public void GetVirusId()
    {
        if (TownManager.instance.availableQuarantine == null) return;
        if (TownManager.instance.availableQuarantine.villagers.Count == 0) return;

        Virus inflictedVirus = TownManager.instance.availableQuarantine.villagers[0].villagerHealth.inflictedVirus;
        if (inflictedVirus == null) return;

        for (int i = 0; i < VirusManager.instance.viruses.Count; i++)
        {
            Virus masterVirus = VirusManager.instance.viruses[i];
            if (inflictedVirus.infection == masterVirus.infection &&
                inflictedVirus.severity == masterVirus.severity &&
                inflictedVirus.lethality == masterVirus.lethality && 
                inflictedVirus.resistanceType == masterVirus.resistanceType &&
                inflictedVirus.trait1 == masterVirus.trait1 && 
                inflictedVirus.trait2 == masterVirus.trait2 &&
                inflictedVirus.trait3 == masterVirus.trait3)
            {
                virusId = i;
                break;
            }
        }
    }
}