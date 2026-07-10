using UnityEngine;
using UnityEngine.UI;

public class Tabs : MonoBehaviour
{
    public GameObject[] windows;
    public Image[] subMenus;
    public Sprite[] subMenuStates;
    public int defaultWindowId = 0;

    void Start()
    {
        DefaultWindow();
    }

    public void ChooseMenu(int id)
    {
        CloseAllMenus();

        windows[id].SetActive(true);
        subMenus[id].sprite = subMenuStates[1];

        if(id == 1) //Hardcoded for Inventory window
        {
            TownStorage.instance.UpdateVisuals();
        }
    }

    public void CloseAllMenus()
    {
        for(int i = 0; i < windows.Length; i++)
        {
            windows[i].SetActive(false);
            subMenus[i].sprite = subMenuStates[0];
        }
    }

    public void DefaultWindow()
    {
        ChooseMenu(defaultWindowId);
    }
}
