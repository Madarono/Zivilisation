using System.Collections.Generic;
using System.Collections;
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

    [Header("Attempt Price")]
    public int price;
    public int baseLabPrice = 20;
    public float sliderPriceMultiplyer = 2f;

    [Header("Attempt Win Con")]
    public float attemptWinPer = 0.85f;
    public float affinityPer;
    public bool cured;

    [Header("Animation")]
    public float diagnoseTime = 3f;
    public float diagnoseVisualTime = 1f;
    public float transitionTime = 2f;
    public float showTextTime = 5f;
    public GameObject[] textAnim;
    public float showResultDelay = 1f;
    public float showOptionsDelay = 0.5f;
    public GameObject[] afterAnim;

    public GameObject diagnoseWindow;
    public GameObject reportWindow;
    public Vector2 originalDimensions;
    public Vector2 reportDimensions;

    [Header("Visuals")]
    public GameObject lookAheadWindow;
    public GameObject laboratoryWindow;
    public Slider infectionSlider;
    public Slider severitySlider;
    public Slider lethalitySlider;
    public TMP_InputField virusNameVisual;
    public TextMeshProUGUI infectionValue;
    public TextMeshProUGUI severityValue;
    public TextMeshProUGUI lethalityValue;
    public TextMeshProUGUI priceVisual;

    [Header("Visuals - Report Window")]
    public TextMeshProUGUI diagnoseVisual;
    public TextMeshProUGUI infectionStat;
    public TextMeshProUGUI severityStat;
    public TextMeshProUGUI lethalityStat;
    public TextMeshProUGUI resultStat;
    public TextMeshProUGUI affinityStat;
    public TextMeshProUGUI backVisual;
    public Color[] resultStates;

    private RectTransform reportRect;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        HideReportVisuals();
        reportRect = reportWindow.GetComponent<RectTransform>();

        laboratoryWindow.SetActive(false);
        manual = ManualSystem.instance;
        diagnoseWindow.SetActive(false);

        reportRect.sizeDelta = originalDimensions;
    }

    public void HideAllVisuals()
    {
        HideReportVisuals();
        manualWindow.CloseWindow();
        reportWindow.SetActive(false);
        diagnoseWindow.SetActive(false);
    }

    public void UpdateVisuals()
    {
        GetVirusId();
        EnsureListSizes(virusId);
        UpdateMimicVirus();
        UpdateVirusDNA();
        UpdatePrice();

        if (manualPage[virusId] == -1)
        {
            SetManualPage();
        }

        virusNameVisual.text = virusNames[virusId];
        manual.headerInfo[manualPage[virusId]] = virusNames[virusId];

        priceVisual.text = $"Attempt - ${price}";
    }

    
    public void SliderUpdateMimicVirus()
    {
        UpdateMimicVirus();
        UpdatePrice();
        priceVisual.text = $"Attempt - ${price}";
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
        // TownManager.instance.availableLaboratory.HideVisuals();
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

    public int GetVirusIdCustom(Virus virus)
    {
        if (virus == null) return -1;

        for (int i = 0; i < VirusManager.instance.viruses.Count; i++)
        {
            Virus masterVirus = VirusManager.instance.viruses[i];
            if (virus.infection == masterVirus.infection &&
                virus.severity == masterVirus.severity &&
                virus.lethality == masterVirus.lethality && 
                virus.resistanceType == masterVirus.resistanceType &&
                virus.trait1 == masterVirus.trait1 && 
                virus.trait2 == masterVirus.trait2 &&
                virus.trait3 == masterVirus.trait3)
            {
                return i;
                break;
            }
        }

        return -1;
    }

    //Mimicing Viruses
    public void UpdateMimicVirus()
    {
        infectionValue.text = infectionSlider.value.ToString();
        severityValue.text = severitySlider.value.ToString();
        lethalityValue.text = lethalitySlider.value.ToString();
        VirusDNAMaker.instance.SetVirusDNA(infectionSlider.value, severitySlider.value, lethalitySlider.value, true);
    }

    public void UpdateVirusDNA()
    {
        VirusDNAMaker.instance.SetVirusDNA(VirusManager.instance.viruses[virusId].infection,
        VirusManager.instance.viruses[virusId].severity,
        VirusManager.instance.viruses[virusId].lethality,
        false);
    }

    public void UpdatePrice()
    {
        float finalPrice = 0;

        finalPrice += baseLabPrice + (infectionSlider.value * sliderPriceMultiplyer) + (severitySlider.value * sliderPriceMultiplyer) + (lethalitySlider.value * sliderPriceMultiplyer);
        finalPrice *= PriceMultiplyer();

        price = Mathf.CeilToInt(finalPrice);
    }

    float PriceMultiplyer()
    {
        switch(VirusManager.instance.viruses[virusId].resistanceType)
        {
            case VirusResistance.None:
                return 1f;
                break;

            case VirusResistance.OneQuarterX:
                return 1.25f;
                break;

            case VirusResistance.OneHalfX:
                return 1.5f;
                break;

            case VirusResistance.TwoX:
                return 2f;
                break;

            case VirusResistance.ThreeX:
                return 3f;
                break;
        }

        return 1f;
    }

    //Attemping
    public void Attempt()
    {
        if(TownStorage.instance.Money < price)
        {
            PopupText.instance.Popup("Insufficient Money. Can't attempt cure.");
            return;
        }
        
        TownStorage.instance.Money -= price;
        CheckWin();
        StartCoroutine(Diagnose());
    }

    IEnumerator Diagnose()
    {
        float t = 0;
        Coroutine visual = StartCoroutine(DiagnoseVisual());
        reportRect.sizeDelta = originalDimensions;
        diagnoseWindow.SetActive(true);
        reportWindow.SetActive(true);
        diagnoseVisual.gameObject.SetActive(true);
        HideReportVisuals();        

        while(t < diagnoseTime)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        t = 0;
        Vector2 transition = originalDimensions;
        diagnoseVisual.gameObject.SetActive(false);    
        StopCoroutine(visual); 

        while(t < transitionTime)
        {
            t += Time.unscaledDeltaTime;
            transition = Vector2.Lerp(originalDimensions, reportDimensions, t / transitionTime);
            reportRect.sizeDelta = transition;
            yield return null;
        }

        t = 0;
        float delay = showTextTime / textAnim.Length;
        int idOrder = 0;

        while(t < showTextTime)
        {
            t += Time.deltaTime;

            if(t >= delay * (idOrder + 1))
            {
                textAnim[idOrder].SetActive(true);
                idOrder++;
                AudioManager.instance.Play(AudioManager.instance.stats);
            }

            yield return null;
        }

        yield return new WaitForSecondsRealtime(showResultDelay);
        AudioManager.instance.Play(cured ? AudioManager.instance.cureSuccess : AudioManager.instance.cureFailure);
        resultStat.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(showOptionsDelay);

        foreach(var obj in afterAnim)
        {
            obj.SetActive(true);
        }
    }

    IEnumerator DiagnoseVisual()
    {
        float t = 0;
        float splitTime = diagnoseVisualTime / 3;
        string originalVisual = diagnoseVisual.text;
        while(true)
        {
            t += Time.unscaledDeltaTime;

            if(t < splitTime)
            {
                diagnoseVisual.text = originalVisual + ".";
            }
            else if(t < splitTime * 2)
            {
                diagnoseVisual.text = originalVisual + "..";
            }
            else if(t < diagnoseVisualTime)
            {
                diagnoseVisual.text = originalVisual + "...";
            }
            else
            {
                t = 0;
            }

            yield return null;
        }
    }

    void CheckWin()
    {
        float infectionPer = VirusManager.instance.viruses[virusId].infection > 0 ? Mathf.Clamp01(infectionSlider.value / VirusManager.instance.viruses[virusId].infection) : 1f;
        float severityPer = VirusManager.instance.viruses[virusId].severity > 0 ? Mathf.Clamp01(severitySlider.value / VirusManager.instance.viruses[virusId].severity) : 1f;
        float lethalityPer = VirusManager.instance.viruses[virusId].lethality > 0 ? Mathf.Clamp01(lethalitySlider.value / VirusManager.instance.viruses[virusId].lethality) : 1f;
        
        Debug.Log(infectionPer);
        Debug.Log(severityPer);
        Debug.Log(lethalityPer);
    
        float rawAffinity = (infectionPer + severityPer + lethalityPer) / 3f;

        cured = rawAffinity >= attemptWinPer;

        affinityPer = Mathf.Round(rawAffinity * 100f);

        infectionStat.text = $"Infection: {infectionSlider.value}";
        severityStat.text = $"Severity: {severitySlider.value}";
        lethalityStat.text = $"Lethality: {lethalitySlider.value}";
        
        resultStat.text = cured ? "- Success -" : "- Failure -";
        resultStat.color = cured ? resultStates[1] : resultStates[0];
        backVisual.text = cured ? "Leave" : "Retry";
        affinityStat.text = $"Affinity: {affinityPer}%";
    }

    public void Leave()
    {
        if(cured)
        {
            reportWindow.SetActive(false);
            diagnoseWindow.SetActive(false);
            if(TownManager.instance.availableQuarantine != null && TownManager.instance.availableQuarantine.villagers.Count > 0)
            {
                TownManager.instance.availableQuarantine.villagers[0].villagerHealth.Cure();
                TownManager.instance.availableQuarantine.RemoveHuman();
            }
            VaccineSystem.instance.curedVirusId.Add(virusId);
            TownManager.instance.availableLaboratory.HideVisuals();
            HideAllVisuals();
            return;
        }

        reportWindow.SetActive(false);
        diagnoseWindow.SetActive(false);
        HideReportVisuals();
    }

    void HideReportVisuals()
    {
        foreach(var obj in textAnim)
        {
            obj.SetActive(false);
        }
        resultStat.gameObject.SetActive(false);
        foreach(var obj in afterAnim)
        {
            obj.SetActive(false);
        }
    }

    
}