using UnityEngine;

public class Window : MonoBehaviour
{
    public Tabs tabs;
    public GameObject window;
    public bool isOpen;
    public bool stopBuild;

    [Header("Special")]
    public bool isHunger;

    void Start()
    {
        window.SetActive(false);
    }

    public void BothWindow()
    {
        if(ActiveWindow.instance.briefActive) return;
        
        isOpen = !isOpen;
        if(isOpen)
        {
            OpenWindow();
        }
        else
        {
            CloseWindow();
        }
    }

    public void OpenWindow()
    {
        if(ActiveWindow.instance.currentActiveWindow != null)
        {
            ActiveWindow.instance.currentActiveWindow.CloseWindow();
        }

        ActiveWindow.instance.currentActiveWindow = this;
        ActiveWindow.instance.isActive = true;
        window.SetActive(true);
        isOpen = true;
        if(stopBuild)
        {
            BuildSystem.instance.StopBuilding();
        }

        if(tabs == null)
        {
            return;
        }

        if(isHunger) //For HungerManager.cs
        {
            HungerManager.instance.UpdateVisuals();
        }

        tabs.DefaultWindow();
    }

    public void CloseWindow()
    {
        if(ActiveWindow.instance.currentActiveWindow == null || (ActiveWindow.instance.currentActiveWindow != null && ActiveWindow.instance.currentActiveWindow != this)) return;

        ActiveWindow.instance.currentActiveWindow = null;
        ActiveWindow.instance.isActive = false;
        window.SetActive(false);
        isOpen = false;
        
        if(tabs == null)
        {
            return;
        }

        tabs.CloseAllMenus();
    }
}
