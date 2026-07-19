using UnityEngine;

public class Window : MonoBehaviour
{
    public Tabs tabs;
    public GameObject window;
    public bool isOpen;
    public bool stopBuild;

    void Start()
    {
        window.SetActive(false);
    }

    public void BothWindow()
    {
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
            Debug.Log("Another window is active");
            return;
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

        tabs.DefaultWindow();
    }

    public void CloseWindow()
    {
        if(ActiveWindow.instance.currentActiveWindow == null || (ActiveWindow.instance.currentActiveWindow != null && ActiveWindow.instance.currentActiveWindow != this)) return;

        ActiveWindow.instance.currentActiveWindow = null;
        ActiveWindow.instance.isActive = false;
        window.SetActive(false);
        
        if(tabs == null)
        {
            return;
        }

        tabs.CloseAllMenus();
    }
}
