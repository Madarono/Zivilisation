using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BuildOptions : MonoBehaviour
{
    public static BuildOptions instance {get; private set;}
    public bool isActive;
    public GameObject cancelButton;
    public GameObject acceptButton;
    public GameObject window;
    public int id = 1;
    public bool isOpen;

    [Header("Sell Value")]
    public float sellValueMultiplyer = 0.7f;

    [Header("Current chosen")]
    public Camera cam;
    public SpriteRenderer debugItem;
    public GameObject priceObj;
    public TextMeshProUGUI priceVisual;
    public BuildItem item;

    [Header("Delete Buildings")]
    public Image shovelBuilds;
    public Sprite[] shovelStates;
    public bool isShovelling;

    [Header("Move Buildings")]
    public Image moveBuilds;
    public Sprite[] moveStates;
    public bool isMoving;
    private Building moveableBuilding;

    private Vector2Int finalPos;
    private Vector2 finalWorldPos;
    private bool isPositionLocked;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        window.SetActive(false);
        isOpen = false;
        priceObj.SetActive(false);
        debugItem.gameObject.SetActive(false);
        shovelBuilds.gameObject.SetActive(false);
        moveBuilds.gameObject.SetActive(false);
    }

    void Update()
    {
        cancelButton.SetActive((item != null || moveableBuilding != null));
        acceptButton.SetActive((item != null || moveableBuilding != null));

        if(item == null && !isShovelling && !isMoving)
        {
            return;
        }

        if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject() && !isShovelling && (!isMoving || (isMoving && moveableBuilding != null)))
        {
            return; 
        }
        if(Input.touchCount > 0 && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && !isShovelling && (!isMoving || (isMoving && moveableBuilding != null)))
        {
            return;
        }

        Vector3 screenPos = Input.mousePosition;
        screenPos.z = Mathf.Abs(cam.transform.position.z);
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        int xPos = Mathf.RoundToInt(worldPos.x);
        int yPos = Mathf.RoundToInt(worldPos.y);

        if((Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) && isShovelling)
        {
            Collider2D hit = Physics2D.OverlapPoint(worldPos);

            if(hit != null)
            {
                if(hit.gameObject.TryGetComponent(out Building hitBuilding))
                {
                    //Checking if villagers are already sleeping
                    foreach(var villager in hitBuilding.villagers)
                    {
                        if(villager.state == VillagerState.Sleeping)
                        {
                            PopupText.instance.Popup("Villager sleeping inside. Can't delete.");
                            return;
                        }
                    }

                    //Remove Building from stored places
                    foreach(var placeTaken in hitBuilding.gridTaken.placesTaken)
                    {
                        GridManager.instance.placesTaken.Remove(placeTaken);
                    }
                    GridManager.instance.buildings.Remove(hitBuilding);

                    //Remind Villagers
                    foreach(var villager in hitBuilding.villagers)
                    {
                        if(villager.goingToHouse)
                        {
                            villager.state = VillagerState.Idle;
                            villager.goingToHouse = false;
                            villager.villagerPF.CancelMovement();
                        }
                    }

                    TownStorage.instance.money += hitBuilding.sellValue;
                    Destroy(hit.gameObject);
                }
                else if(hit.gameObject.TryGetComponent(out Farm hitFarm))
                {
                    if(hitFarm.villagers.Count > 0)
                    {
                        hitFarm.RemindVillagerStop();
                    }
                    TownStorage.instance.money += hitBuilding.sellValue;
                    Destroy(hit.gameObject);
                }
                else if(hit.gameObject.TryGetComponent(out Mines hitMines))
                {
                    if(hitMines.villagers.Count > 0)
                    {
                        hitMines.RemindVillagerStop();
                    }
                    TownStorage.instance.money += hitBuilding.sellValue;
                    Destroy(hit.gameObject);
                }
                else if(hit.gameObject.TryGetComponent(out Gate hitGate))
                {
                    PopupText.instance.Popup("The Gate occupies this tile. Can't delete.");
                    return;
                }
            }
        }

        if((Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) && isMoving)
        {
            Collider2D hit = Physics2D.OverlapPoint(worldPos);

            if(hit != null)
            {
                if(hit.gameObject.TryGetComponent(out Building hitBuilding))
                {
                    SelectedMovableBuilding(hitBuilding);
                    return;
                }
            }
        }

        if(!isPositionLocked)
        {
            finalWorldPos = new Vector2(xPos, yPos);
            debugItem.gameObject.transform.position = finalWorldPos;
        }
        
        if(Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            finalPos = new Vector2Int(xPos, yPos);
            finalWorldPos = new Vector2(xPos, yPos);
            debugItem.gameObject.transform.position = finalWorldPos;
            isPositionLocked = true;
        }
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
        priceObj.SetActive(false);
        shovelBuilds.gameObject.SetActive(true);
        moveBuilds.gameObject.SetActive(true);
    }

    public void CloseWindow()
    {
        window.SetActive(false);
        isOpen = false;
        // BuildSystem.instance.options[id].optionVisual.sprite = BuildSystem.instance.options[id].optionStates[0];
    }

    public void BothShovel()
    {
        isShovelling = !isShovelling;

        if(isShovelling)
        {
            ActivateShovel();
        }
        else
        {
            DeactivateShovel();
        }
    }

    public void ActivateShovel()
    {
        this.item = null;
        isPositionLocked = false;
        finalPos = Vector2Int.zero;
        debugItem.gameObject.SetActive(false);
        priceObj.SetActive(false);
        cancelButton.SetActive(false);
        acceptButton.SetActive(false);
        shovelBuilds.sprite = shovelStates[1];
        isShovelling = true;
        DeactivateMove();
        CloseWindow();
    }

    public void DeactivateShovel()
    {
        shovelBuilds.sprite = shovelStates[0];
        isShovelling = false;
    }

    public void BothMove()
    {
        isMoving = !isMoving;

        if(isMoving)
        {
            ActivateMove();
        }
        else
        {
            DeactivateMove();
        }
    }

    public void ActivateMove()
    {
        DeactivateShovel();
        this.item = null;
        isPositionLocked = false;
        finalPos = Vector2Int.zero;
        debugItem.gameObject.SetActive(false);
        priceObj.SetActive(false);
        cancelButton.SetActive(false);
        acceptButton.SetActive(false);
        moveBuilds.sprite = moveStates[1];
        isMoving = true;
        CloseWindow();
    }

    public void DeactivateMove()
    {
        moveBuilds.sprite = moveStates[0];
        isMoving = false;
        CancelMove();
    }

    public void SelectedMovableBuilding(Building building)
    {
        if(!CheckBuildingUnTouched(building))
        {
            return;     
        }

        building.gridTaken.RevertUnwalkable();
        building.gameObject.SetActive(false);
        moveableBuilding = building;
        isPositionLocked = false;
        debugItem.gameObject.SetActive(true);
        debugItem.sprite = building.buildingStates[0]; //Unworked building at normal state or house at normal state
        finalPos = Vector2Int.zero;
    }

    public bool CheckBuildingUnTouched(Building building)
    {
        Farm farmBuilding = building as Farm;
        Mines minesBuilding = building as Mines;

        if(farmBuilding != null || minesBuilding != null)
        {
            if(DayCycle.instance.hours > TownManager.instance.hourWorkReq && DayCycle.instance.hours < TownManager.instance.hourStopReq)
            {
                PopupText.instance.Popup("Can't move workplace during work hours.");
                return false;
            }
        }
        else
        {
            if(DayCycle.instance.hours > TownManager.instance.hourSleepReq && DayCycle.instance.hours < TownManager.instance.hourAwakeReq)
            {
                PopupText.instance.Popup("Can't move residential building during sleeping hours.");
                return false;
            }
        }

        return true;
    }
    
    public void ChooseOption(BuildItem item)
    {
        this.item = item;
        if(TownStorage.instance.money < item.price)
        {
            this.item = null;
            return;
        }

        isPositionLocked = false;
        finalPos = Vector2Int.zero;
        debugItem.gameObject.SetActive(true);
        priceObj.SetActive(true);
        priceVisual.text = item.priceVisual.text;
        debugItem.sprite = item.itemSprite;
        CloseWindow();
    }

    public void CancelButton()
    {
        if(!isMoving)
        {
            CancelOption();
        }
        else
        {
            CancelMove();
        }
    }

    public void CancelMove()
    {
        if(moveableBuilding == null)
        {
            return;
        }

        moveableBuilding.gameObject.SetActive(true);
        moveableBuilding.gridTaken.PutUnwalkable();
        moveableBuilding = null;
        debugItem.gameObject.SetActive(false);
    }

    public void CancelOption()
    {
        this.item = null;
        isPositionLocked = false;
        finalPos = Vector2Int.zero;
        debugItem.gameObject.SetActive(false);
        priceObj.SetActive(false);
        cancelButton.SetActive(false);
        acceptButton.SetActive(false);
        BuildSystem.instance.CloseOther(id);
    }

    public void AcceptButton()
    {
        if(!isMoving)
        {
            AcceptOption();
        }
        else
        {
            AcceptMove();
        }
    }

    public void AcceptMove()
    {
        if(!CheckBoundries(finalPos, moveableBuilding) || !isPositionLocked || moveableBuilding == null)
        {
            return;
        }

        Vector2 buildingPos = new Vector2(finalPos.x, finalPos.y);
        moveableBuilding.gameObject.transform.position = buildingPos;
        moveableBuilding.gameObject.SetActive(true);
        debugItem.gameObject.SetActive(false);
        moveableBuilding.gridTaken.MeasureGridTaken();
        moveableBuilding.gridTaken.PutUnwalkable();
        moveableBuilding = null;
    }

    public void AcceptOption()
    {
        if(TownStorage.instance.money < item.price || !isPositionLocked)
        {
            return;
        }

        if(!CheckBoundries(finalPos))
        {
            return;
        }

        TownStorage.instance.money -= item.price;
        SpawnOption(item.itemPrefab, finalWorldPos, item.price * sellValueMultiplyer);

        // GameObject go = Instantiate(item.itemPrefab, finalWorldPos, Quaternion.identity);
        // if(go.TryGetComponent(out Building goBuilding)) 
        // {
        //     goBuilding.gridTaken.MeasureGridTaken();
        //     goBuilding.gridTaken.PutUnwalkable();
        //     goBuilding.sellValue = Mathf.FloorToInt(item.price * sellValueMultiplyer);
        //     GridManager.instance.buildings.Add(goBuilding);
        // }
    }

    public void SpawnOption(GameObject prefab, Vector2 prefabPos, float sellValue = 0f)
    {
        GameObject go = Instantiate(prefab, prefabPos, Quaternion.identity);
        if(go.TryGetComponent(out Building goBuilding)) 
        {
            goBuilding.gridTaken.MeasureGridTaken();
            goBuilding.gridTaken.PutUnwalkable();
            goBuilding.sellValue = Mathf.FloorToInt(sellValue);
            GridManager.instance.buildings.Add(goBuilding);
        }
    }

    bool CheckBoundries(Vector2Int pos, Building? building = null)
    {
        int width = building?.gridTaken.width ?? item.width;
        int height = building?.gridTaken.height ?? item.height;

        for(int w = 0; w < width; w++)
        {
            for(int h = 0; h < height; h++)
            {
                Vector2Int checkPos = new Vector2Int(pos.x + w, pos.y + h); //Check for roads
                if(RoadSystem.instance.roadPos.ContainsKey(checkPos))
                {
                    return false;
                }

                if(GridManager.instance.placesTaken.Contains(checkPos)) //Check for buildings
                {
                    return false;
                }
            }
        }

        Vector2Int underPos = new Vector2Int(pos.x, pos.y - 1);
        if(GridManager.instance.placesTaken.Contains(underPos)) //Check for buildings
        {
            return false;
        }

        for(int side = 0; side < width; side++)
        {
            Vector2Int overPos = new Vector2Int(pos.x + side, pos.y + height);
            Vector2Int gatePos = new Vector2Int((int)GridManager.instance.gate.gameObject.transform.position.x, (int)GridManager.instance.gate.gameObject.transform.position.y);
            if(overPos == gatePos) //Check for gate
            {
                return false;
            }
        }


        return true;
    }
}