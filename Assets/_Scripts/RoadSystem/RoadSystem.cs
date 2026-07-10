using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoadSystem : MonoBehaviour
{
    public static RoadSystem instance {get; private set;}
    public GameObject roadPrefab;
    public Transform roadParent;
    public Dictionary<Vector2Int, GameObject> roadPos = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<GameObject, Vector2Int> positionPerRoad = new Dictionary<GameObject, Vector2Int>();
    public bool isActive = true; //ToMake sure it is the one building; not others

    [Header("Shovel Mode")]
    public Image shovelVisual;
    public Sprite[] shovelStates;
    public bool shovelMode;

    [Header("MultiBrush Mode")]
    public LayerMask brushLayers;
    public Image brushButton;
    public Sprite[] multiBrushStates;
    public bool isMultiBrush = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if(positionPerRoad.Count > 0)
        {
            UpdateAllRoads();
        }
    }

    public void Update()
    {
        shovelVisual.gameObject.SetActive(isActive);
        brushButton.gameObject.SetActive(isActive);

        if (!ViewMode.instance.viewMode && TownManager.instance.isBuilding && isActive)
        {
            Vector3 inputScreenPos = Vector3.zero;
            bool inputDetected = false;
            int pointerId = -1;
        
            if(Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began || (touch.phase == TouchPhase.Moved && isMultiBrush))
                {
                    inputScreenPos = touch.position;
                    pointerId = touch.fingerId;
                    inputDetected = true;
                }
            }
            else if(Input.GetMouseButtonDown(0) || (Input.GetMouseButton(0) && isMultiBrush))
            {
                inputScreenPos = Input.mousePosition;
                pointerId = -1;
                inputDetected = true;
            }

            if(inputDetected)
            {
                if(EventSystem.current != null && EventSystemExtensions.IsPointerOverLayer(brushLayers))
                {
                    return;
                }

                CheckForRoad(inputScreenPos);
            }
        }
    }

    public void ShovelMode()
    {
        shovelMode = !shovelMode;
        shovelVisual.sprite = shovelMode ? shovelStates[1] : shovelStates[0];
    }

    public void StopShovelMode()
    {
        shovelMode = false;
        shovelVisual.sprite = shovelStates[0];
    }

    public void MultiBrushMode()
    {
        isMultiBrush = !isMultiBrush;
        brushButton.sprite = isMultiBrush ? multiBrushStates[1] : multiBrushStates[0];
    }

    public void StopMultiBrushMode()
    {
        isMultiBrush = false;
        brushButton.sprite = multiBrushStates[0];
    }

    public void CheckForRoad(Vector3 inputScreenPos)
    {
        Vector3 clickPos = Camera.main.ScreenToWorldPoint(inputScreenPos);
        Vector2Int roadPos = new Vector2Int(Mathf.RoundToInt(clickPos.x), Mathf.RoundToInt(clickPos.y));
        
        if(CheckNeighbors(roadPos) && !shovelMode)
        {
            PutRoad(roadPos);
        }
        else if(shovelMode)
        {
            DeleteRoad(roadPos);
        }
    }

    public void PutRoad(Vector2Int pos)
    {
        Vector3 goPos = new Vector3(pos.x, pos.y, 0);
        GameObject go = Instantiate(roadPrefab, goPos, Quaternion.identity);
        go.transform.SetParent(roadParent);
        
        if(!roadPos.ContainsKey(pos))
        {
            roadPos.Add(pos, go);
        }
        if(!positionPerRoad.ContainsKey(go))
        {
            positionPerRoad.Add(go, pos);
        }

        UpdateOneRoad(pos);
    }

    bool CheckNeighbors(Vector2Int pos)
    {
        //First Road can be placed anywhere
        if(roadPos.Count == 0)
        {
            return true;
        }

        Vector2Int posUp = new Vector2Int(pos.x, pos.y + 1);
        Vector2Int posDown = new Vector2Int(pos.x, pos.y - 1);
        Vector2Int posLeft = new Vector2Int(pos.x + 1, pos.y);
        Vector2Int posRight = new Vector2Int(pos.x - 1, pos.y);

        List<Vector2Int> posChecks = new List<Vector2Int>();
        posChecks.Add(posUp);
        posChecks.Add(posDown);
        posChecks.Add(posLeft);
        posChecks.Add(posRight);

        foreach(var check in posChecks)
        {
            if(roadPos.ContainsKey(check) && !roadPos.ContainsKey(pos))
            {
                return true;
            }
        }

        return false;
    }

    public void UpdateAllRoads()
    {
        foreach(var road in positionPerRoad)
        {
            Vector2Int pos = road.Value;
            GridManager.instance.PlaceRoad(pos.x, pos.y);
        }
    }

    public void UpdateOneRoad(Vector2Int pos)
    {
        GridManager.instance.PlaceRoad(pos.x, pos.y);
    }

    public void DeleteRoad(Vector2Int pos)
    {
        if(roadPos.ContainsKey(pos))
        {
            GameObject road = roadPos[pos];
            roadPos.Remove(pos);
            Destroy(road);
            GridManager.instance.PlaceBuilding(pos.x, pos.y); //To make the tile unwalkable
        }
    }

    public void DeleteAllRoads()
    {
        foreach(var roadPos in roadPos)
        {
            GameObject road = roadPos.Value;
            Destroy(road);
            GridManager.instance.PlaceBuilding(roadPos.Key.x, roadPos.Key.y);
        }

        roadPos.Clear();
    }


}
