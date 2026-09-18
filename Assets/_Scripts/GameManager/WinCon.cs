using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinCon : MonoBehaviour
{
    public static WinCon instance {get; private set;}

    [Header("Requirement To Win")]
    public int requiredVaccines = 5;

    [Header("Visual")]
    public Window envellope;
    public bool hasWon;
    public TMP_InputField returnText;
    public string requiredText = "return";

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        envellope.gameObject.SetActive(hasWon);
    }

    public void CheckWinCon()
    {
        if(VaccineSystem.instance.vaccinatedVirusId.Count >= requiredVaccines)
        {
            TriggerWin(true, true);
        }
    }

    [ContextMenu("TriggerWinCon")]
    public void TriggerWinForce()
    {
        TriggerWin(true, true);
    }

    public void TriggerWin(bool sound, bool popup)
    {
        hasWon = true;
        envellope.gameObject.SetActive(true);

        if(sound) AudioManager.instance.Play(AudioManager.instance.envellope);
        if(popup) PopupText.instance.Popup("You got mail!");
    }

    public void CloseWindow()
    {
        envellope.CloseWindow();
        Stats.instance.CloseWindow();
    }

    public void CheckSignature()
    {
        if(returnText.text.ToLower() == requiredText)
        {
            Debug.Log("Return To MainMenu!");
            MainMenu();
        }
    }

    public void MainMenu()
    {
        //Make some black transition to MainMenu
    }
}