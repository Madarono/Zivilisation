using UnityEngine;
using UnityEngine.UI;

public class ViewMode : MonoBehaviour
{
    public static ViewMode instance {get; private set;}
    public GameObject viewVisual;
    public bool viewMode;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        viewMode = false;
        viewVisual.SetActive(viewMode);
    }

    public void ChangeViewMode()
    {
        viewMode = !viewMode;
        viewVisual.SetActive(viewMode);

        if(viewMode)
        {
            BuildSystem.instance.StopBuilding();
        }
    }

    public void StopViewMode()
    {
        viewMode = false;
        viewVisual.SetActive(false);
    }
}
