using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SwitchPages : MonoBehaviour
{
    public GameObject[] windows;
    public string[] windowNames;
    public TextMeshProUGUI modeVisual;
    public int choose;

    void Start()
    {
        ResetMode();
    }

    public void ResetMode()
    {
        choose = 0;
        UpdateVisuals();
    }

    public void ChangeMode()
    {
        choose++;

        if(choose >= windows.Length) choose = 0;

        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        int chooseVisual = choose + 1;
        if(chooseVisual >= windows.Length) chooseVisual = 0;

        modeVisual.text = windowNames[chooseVisual];
        for(int i = 0; i < windows.Length; i++)
        {
            windows[i].SetActive(i == choose);
        }
    }
}