using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Storage 
{
    Wheat,
    Iron,
    Copper,
    Quartz,
    Titanium
}

public enum VirusStats
{
    Infection,
    Severity,
    Lethality,
}

[System.Serializable]
public class MaxResource
{
    public Storage resource;
    public int minResource = 0;
    public int maxResource = 100;
}

[System.Serializable]
public class ResourceRequirement
{
    public VirusStats stat;
    public MaxResource[] resources;
}

[System.Serializable]
public class StorageMultiplyer
{
    public Storage resource;
    public float multiplyer;
}

[System.Serializable]
public class TraitMultiplyer
{
    public VirusTrait trait;
    public StorageMultiplyer[] multiplyers;
}

[System.Serializable]
public class ResistanceMultiplyer
{
    public VirusResistance resistance;
    public float multiplyer;
}

[System.Serializable]
public class VaccineVisual
{
    public GameObject icon;
    public TextMeshProUGUI iconVisual;
}

public class VaccineSystem : MonoBehaviour
{
    public static VaccineSystem instance {get; private set;}

    public VaccineLookAhead lookAhead;

    public HashSet<int> curedVirusId = new HashSet<int>();
    public HashSet<int> vaccinatedVirusId = new HashSet<int>();

    [Header("Visuals")]
    public GameObject window;
    public GameObject returnButton;
    public GameObject[] sideButtons;

    public GameObject vaccineWindow;

    [Header("Vaccine Visuals")]
    public Storage[] resourceType;
    public GameObject[] icons;
    public TextMeshProUGUI[] iconVisuals;
    public Color[] valueStates;
    private string[] valueHex;

    [Header("Vaccine Button Visuals")]
    public Image vaccineButton;
    public TextMeshProUGUI vaccineVisual;
    public Sprite[] vaccineButtonStates;
    public Color[] vaccineVisualStates;

    [Header("Values")]
    public ResourceRequirement[] resourceReq;
    public TraitMultiplyer[] traitMultiplyer;
    public ResistanceMultiplyer[] resistanceMultiplyer;
    public int currentVaccinateVirusId;
    [SerializeField] private bool canVaccinate;
    public Dictionary<Storage, int> resources = new Dictionary<Storage, int>();
    public Dictionary<Storage, VaccineVisual> vaccineVisuals = new Dictionary<Storage, VaccineVisual>();


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        PopulateResourceDic();
        PopulateHex();
        CloseWindow();
    }

    public void OpenWindow(bool showReturn) //showReturn decides if there are SideButtons or not
    {
        foreach(var sideButton in sideButtons)
        {
            sideButton.SetActive(!showReturn);
        }
        returnButton.SetActive(showReturn);
        window.SetActive(true);
        UpdateVisuals();
    }

    public void CloseWindow()
    {
        window.SetActive(false);
        returnButton.SetActive(false);
        vaccineWindow.SetActive(false);
    }

    public void UpdateVisuals()
    {
        lookAhead.PopulateListString("");
        lookAhead.UpdateList();
    }

    //Vaccinate
    public void ShowVaccine(int infection, int severity, int lethality, int virusId)
    {
        currentVaccinateVirusId = virusId;
        canVaccinate = true;
        int amountCantBuy = 0;

        vaccineWindow.SetActive(true);
        DetermineStats(infection, severity, lethality, virusId);

        foreach(var resource in resources)
        {
            Debug.Log($"{resource.Key}: {resource.Value}");

            if(resource.Value == 0)
            {
                vaccineVisuals[resource.Key].icon.SetActive(false);
                continue;
            }

            vaccineVisuals[resource.Key].icon.SetActive(true);

            int availableStorage = AvailableStorage(resource.Key);
            int requiredResource = resource.Value;
            bool canBuy = availableStorage >= requiredResource;
            amountCantBuy += canBuy ? 0 : 1;

            string hex = canBuy ? valueHex[1] : valueHex[0];
            
            vaccineVisuals[resource.Key].iconVisual.text = $"{resource.Key.ToString()}: <color=#{hex}>{requiredResource}</color> ({availableStorage})";
        }

        canVaccinate = amountCantBuy == 0;

        vaccineButton.sprite = canVaccinate ? vaccineButtonStates[1] : vaccineButtonStates[0];
        vaccineVisual.color = canVaccinate ? vaccineVisualStates[1] : vaccineVisualStates[0];
    }

    public void HideVaccinate()
    {
        vaccineWindow.SetActive(false);
    }

    void DetermineStats(int infection, int severity, int lethality, int virusId)
    {
        float value = 0;
        resources[Storage.Wheat] = 0;
        resources[Storage.Iron] = 0;
        resources[Storage.Copper] = 0;
        resources[Storage.Quartz] = 0;
        resources[Storage.Titanium] = 0;
        

        for(int i = 0; i < resourceReq.Length; i++)
        {
            switch(resourceReq[i].stat)
            {
                case VirusStats.Infection:
                    value = infection / 100f;
                    break;
                
                case VirusStats.Severity:
                    value = severity / 100f;
                    break;
                
                case VirusStats.Lethality:
                    value = lethality / 100f;
                    break;
            }

            Debug.Log(value);

            foreach(var resource in resourceReq[i].resources)
            {
                int amount = Mathf.RoundToInt(Mathf.Lerp(resource.minResource, resource.maxResource, value));
                float traitMultiplyer = TraitMultiplyerSuggester(resource.resource, virusId);
                float resistanceMultiplyer = ResistanceMultiplyerSuggester(virusId);
                resources[resource.resource] += Mathf.RoundToInt(traitMultiplyer * resistanceMultiplyer * amount);
            }
        }
    }

    float TraitMultiplyerSuggester(Storage resource, int virusId)
    {
        if(virusId < 0 || virusId >= VirusManager.instance.viruses.Count || VirusManager.instance.viruses[virusId] == null) return 1f;

        float totalMultiplier = 1f;

        Virus targetVirus = VirusManager.instance.viruses[virusId];

        for (int i = 0; i < traitMultiplyer.Length; i++)
        {
            bool hasTrait = targetVirus.trait1 == traitMultiplyer[i].trait ||
            targetVirus.trait2 == traitMultiplyer[i].trait ||
            targetVirus.trait3 == traitMultiplyer[i].trait;

            if (!hasTrait) continue;

            foreach (var multiplyer in traitMultiplyer[i].multiplyers)
            {
                if (multiplyer.resource == resource)
                {
                    totalMultiplier *= multiplyer.multiplyer;
                }
            }
        }

        return totalMultiplier;
    }

    float ResistanceMultiplyerSuggester(int virusId)
    {
        if(virusId < 0 || virusId >= VirusManager.instance.viruses.Count || VirusManager.instance.viruses[virusId] == null) return 1f;
        
        for(int i = 0; i < resistanceMultiplyer.Length; i++)
        {
            bool hasResistance = VirusManager.instance.viruses[virusId].resistanceType == resistanceMultiplyer[i].resistance;

            if(!hasResistance) continue;

            return resistanceMultiplyer[i].multiplyer;
        }

        return 1;
    }

    int AvailableStorage(Storage resource)
    {
        switch(resource)
        {
            case Storage.Wheat:
                return Mathf.FloorToInt(TownStorage.instance.wheat);
            case Storage.Iron:
                return TownStorage.instance.iron;
            case Storage.Copper:
                return TownStorage.instance.copper;
            case Storage.Quartz:
                return TownStorage.instance.quartz;
            case Storage.Titanium:
                return TownStorage.instance.titanium; 
        }

        return 0; //If I somehow made another resource, just return 0
    }

    void ChangeInventory(Storage resource, int amount)
    {
        switch(resource)
        {
            case Storage.Wheat:
                TownStorage.instance.wheat -= amount;
                break;
            case Storage.Iron:
                TownStorage.instance.iron -= amount;
                break;
            case Storage.Copper:
                TownStorage.instance.copper -= amount;
                break;
            case Storage.Quartz:
                TownStorage.instance.quartz -= amount;
                break;
            case Storage.Titanium:
                TownStorage.instance.titanium -= amount;
                break; 
        }
    }

    void PopulateResourceDic()
    {
        resources.Clear();
        resources.Add(Storage.Wheat, 0);
        resources.Add(Storage.Iron, 0);
        resources.Add(Storage.Copper, 0);
        resources.Add(Storage.Quartz, 0);
        resources.Add(Storage.Titanium, 0);

        vaccineVisuals.Clear();
        for(int i = 0; i < icons.Length; i++)
        {
            vaccineVisuals[resourceType[i]] = new VaccineVisual 
            {
                icon = icons[i],
                iconVisual = iconVisuals[i]
            };
        }
    }

    void PopulateHex()
    {
        valueHex = new string[valueStates.Length];
        
        for(int i = 0; i < valueHex.Length; i++)
        {
            valueHex[i] = ColorUtility.ToHtmlStringRGB(valueStates[i]);
        }
    }

    public void Vaccinate()
    {
        if(!canVaccinate) 
        {
            PopupText.instance.Popup("Insufficient Resources.");
            return;
        }

        foreach(var resource in resources)
        {
            ChangeInventory(resource.Key, resource.Value);
        }
        HideVaccinate();
        vaccinatedVirusId.Add(currentVaccinateVirusId);
        CureAllVaccinated(currentVaccinateVirusId);
        UpdateVisuals();
    }

    public void CureAllVaccinated(int virusId)
    {
        int amountVaccinated = 0;
        foreach(var villager in TownManager.instance.villagers)
        {
            if(villager.villagerHealth.health != Health.Healthy && villager.villagerHealth.inflictedVirus == VirusManager.instance.viruses[virusId])
            {
                villager.villagerHealth.Cure();
                amountVaccinated++;
            }
        }
        
        if(amountVaccinated > 0) PopupText.instance.Popup($"Vaccinated {amountVaccinated} Villagers. This virus is no longer dangerous.");
        else PopupText.instance.Popup($"{LaboratorySystem.instance.virusNames[virusId]} is now fully vaccinated, No active infections were found.");
    }
}