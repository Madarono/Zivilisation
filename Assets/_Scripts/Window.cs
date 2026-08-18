using UnityEngine;

public class Window : MonoBehaviour
{
    public Tabs tabs;
    public SwitchPages switchPages;
    public GameObject window;
    public bool isOpen;
    public bool stopBuild;

    [Header("Special")]
    public bool isHunger;
    public bool isManual;

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

        TownManager.instance.availableLaboratory?.HideVisuals();
        TownManager.instance.availableMarket?.HideVisuals();

        ActiveWindow.instance.currentActiveWindow = this;
        ActiveWindow.instance.isActive = true;
        window.SetActive(true);
        isOpen = true;
        if(stopBuild)
        {
            BuildSystem.instance.StopBuilding();
        }

        if(isManual) //For ManualSystem.cs
        {
            ManualSystem.instance.MakeInitialPages();
            ManualSystem.instance.ReturnToFirstPages();
        }
        if(isHunger) //For HungerManager.cs
        {
            HungerManager.instance.UpdateVisuals();
        }

        if(tabs != null)
        {
            tabs.DefaultWindow();
        }

        if(switchPages != null)
        {
            switchPages.ResetMode();
        }

    }

    public void CloseWindow()
    {
        if(ActiveWindow.instance.currentActiveWindow == null || (ActiveWindow.instance.currentActiveWindow != null && ActiveWindow.instance.currentActiveWindow != this)) return;

        ActiveWindow.instance.currentActiveWindow = null;
        ActiveWindow.instance.isActive = false;
        window.SetActive(false);
        isOpen = false;
        
        if(isManual)
        {
            LookAhead.instance.CloseWindow();
        }
        
        if(tabs != null)
        {
            tabs.CloseAllMenus();
        }
    }
}
