using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VaccineItem : MonoBehaviour
{
    [Header("Visual")]
    public TextMeshProUGUI nameVisual;
    public TextMeshProUGUI statusVisual;
    public Image box;
    public Color[] nameStates;
    public Sprite[] boxStates;

    [Header("Values")]
    public string name;
    public int virusId;

    private bool vaccinated;

    public void Refresh()
    {
        nameVisual.text = name;
        vaccinated = VaccineSystem.instance.vaccinatedVirusId.Contains(virusId);
        
        nameVisual.color = vaccinated ? nameStates[1] : nameStates[0];
        statusVisual.color = vaccinated ? nameStates[1] : nameStates[0];
        statusVisual.text = vaccinated ? "Vaccinated" : "Not Vaccinated";
        box.sprite = vaccinated ? boxStates[1] : boxStates[0];
    }

    public void Choose()
    {
        if(vaccinated) 
        {
            PopupText.instance.Popup("You have vaccinated this virus.");
            return;
        }

        VaccineSystem.instance.ShowVaccine(VirusManager.instance.viruses[virusId].infection,
        VirusManager.instance.viruses[virusId].severity,
        VirusManager.instance.viruses[virusId].lethality, virusId);
    }
}