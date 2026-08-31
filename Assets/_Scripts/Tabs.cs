using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class SubMenu 
{
    public Image option;
    public Sprite[] optionStates;
    public GameObject window;

    [Header("Tab Open Event")]
    public UnityEvent onTabOpened;
}

public class Tabs : MonoBehaviour
{
    public SubMenu[] subMenus;
    public int defaultWindowId = 0;
    
    private int activeWindowId = -1;

    private void Start()
    {
        DefaultWindow();
    }

    public void ChooseMenu(int id)
    {
        if (id < 0 || id >= subMenus.Length) return;

        if (activeWindowId > -1 && activeWindowId < subMenus.Length)
        {
            subMenus[activeWindowId].window.SetActive(false);
            subMenus[activeWindowId].option.sprite = subMenus[activeWindowId].optionStates[0];
        }

        subMenus[id].window.SetActive(true);
        subMenus[id].option.sprite = subMenus[id].optionStates[1];
        activeWindowId = id;

        subMenus[id].onTabOpened?.Invoke();
    }

    public void CloseAllMenus()
    {
        for (int i = 0; i < subMenus.Length; i++)
        {
            subMenus[i].window.SetActive(false);
            subMenus[i].option.sprite = subMenus[i].optionStates[0];
        }
        activeWindowId = -1;
    }

    public void DefaultWindow()
    {
        ChooseMenu(defaultWindowId);
    }
}