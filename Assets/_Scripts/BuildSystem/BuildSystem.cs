using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class OptionVisual
{
    public GameObject option;
    public Image optionVisual;
    public Sprite[] optionStates;
}

public class BuildSystem : MonoBehaviour
{
    public static BuildSystem instance {get; private set;}
    public Window statsWindow;
    private TownManager townManager;
    private RoadSystem roadSystem;
    private BuildOptions buildOptions;

    public GameObject buildVisual;

    [Header("Options")]
    public OptionVisual[] options;
    private List<Vector3> optionsPos = new List<Vector3>();
    public float yOffset = -100f;
    public float speed = 5f;
    public float returnSpeed = 7f;

    private Coroutine activeMoving;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        townManager = TownManager.instance;
        roadSystem = RoadSystem.instance;
        buildOptions = BuildOptions.instance;
        StopBuilding();
        DefinePositions();
    }

    void DefinePositions()
    {
        for(int i = 0; i < options.Length; i++)
        {
            Vector3 pos = new Vector3(0, yOffset * (i + 1), 0);
            optionsPos.Add(pos);
        }
    }

    public void BothBuilding()
    {
        townManager.isBuilding = !townManager.isBuilding;
        buildVisual.SetActive(townManager.isBuilding);

        if(!townManager.isBuilding)
        {
            StopBuilding();
        }
        else
        {
            StartBuilding();
        }
    }

    public void StopBuilding()
    {
        townManager.isBuilding = false;
        WaypointSystem.instance.GenerateWaypoints();
        roadSystem.isActive = false;
        roadSystem.StopShovelMode();
        roadSystem.StopMultiBrushMode();
        buildOptions.isActive = false;
        buildVisual.SetActive(false);
        if(activeMoving != null)
        {
            StopCoroutine(activeMoving);
        }
        activeMoving = StartCoroutine(ReturnAllOptions());
        CloseOther(1);
    }

    public void StartBuilding()
    {
        statsWindow.CloseWindow();
        ViewMode.instance.StopViewMode();
        if(activeMoving != null)
        {
            StopCoroutine(activeMoving);
        }
        activeMoving = StartCoroutine(MoveAllOptions());
    }

    public void ChooseRoads(int id)
    {
        if(!townManager.isBuilding)
        {
            return;
        }

        roadSystem.isActive = !roadSystem.isActive;

        if(roadSystem.isActive)
        {
            options[id].optionVisual.sprite = options[id].optionStates[1];
            CloseOther(1);
        }
        else
        {
            options[id].optionVisual.sprite = options[id].optionStates[0];
            roadSystem.StopShovelMode();
            roadSystem.StopMultiBrushMode();
        }
    }

    public void CloseRoads(int id)
    {
        roadSystem.isActive = false;
        options[id].optionVisual.sprite = options[id].optionStates[0];
        roadSystem.StopShovelMode();
        roadSystem.StopMultiBrushMode();
    }

    public void ChooseOther(int id)
    {
        if(!townManager.isBuilding)
        {
            return;
        }

        buildOptions.isActive = !buildOptions.isActive;

        if(buildOptions.isActive)
        {
            options[id].optionVisual.sprite = options[id].optionStates[1];
            buildOptions.OpenWindow();
            CloseRoads(0);
        }
        else
        {
            options[id].optionVisual.sprite = options[id].optionStates[0];
            CloseOther(1);
        }
    }

    public void CloseOther(int id)
    {
        buildOptions.isActive = false;
        options[id].optionVisual.sprite = options[id].optionStates[0];
        buildOptions.window.SetActive(false);
        buildOptions.isOpen = false;
        buildOptions.item = null;
        buildOptions.debugItem.gameObject.SetActive(false);
        buildOptions.priceObj.SetActive(false);
        buildOptions.cancelButton.SetActive(false);
        buildOptions.DeactivateShovel();
        buildOptions.DeactivateMove();
        buildOptions.shovelBuilds.gameObject.SetActive(false);
        buildOptions.moveBuilds.gameObject.SetActive(false);
    }

    IEnumerator MoveAllOptions()
    {
        foreach(var option in options)
        {
            option.option.SetActive(true);
        }

        while(true)
        {
            for(int i = 0; i < options.Length; i++)
            {
                options[i].option.transform.localPosition = Vector3.Lerp(options[i].option.transform.localPosition, optionsPos[i], speed * Time.unscaledDeltaTime);
            }

            if(Vector2.Distance(options[0].option.transform.localPosition, optionsPos[0]) <= 0.1f)
            {
                for(int i = 0; i < options.Length; i++)
                {
                    options[i].option.transform.localPosition = optionsPos[i];
                }
            }
            yield return null;
        }
    }

    IEnumerator ReturnAllOptions()
    {
        foreach(var option in options)
        {
            option.optionVisual.sprite = option.optionStates[0];
        }

        while(true)
        {
            for(int i = 0; i < options.Length; i++)
            {
                options[i].option.transform.localPosition = Vector3.Lerp(options[i].option.transform.localPosition, Vector3.zero, returnSpeed * Time.unscaledDeltaTime);
            }

            if(Vector2.Distance(options[0].option.transform.localPosition, Vector3.zero) <= 0.1f)
            {
                for(int i = 0; i < options.Length; i++)
                {
                    options[i].option.transform.localPosition = Vector3.zero;
                    options[i].option.SetActive(false);
                }
            }
            yield return null;
        }
    }


}
