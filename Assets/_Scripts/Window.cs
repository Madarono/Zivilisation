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
        window.SetActive(false);
        
        if(tabs == null)
        {
            return;
        }

        tabs.CloseAllMenus();
    }
}
