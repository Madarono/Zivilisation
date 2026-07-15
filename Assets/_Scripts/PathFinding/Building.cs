using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Building : MonoBehaviour, VillageBuildable
{
    public bool isShowing = false;
    protected Collider2D col;
    protected SpriteRenderer rend;

    public GridTaken gridTaken;
    public Sprite[] buildingStates;

    [Header("Sell Value")]
    public int sellValue = 7;

    [Header("Human Icons")]
    public SpriteRenderer[] humanIcons;
    public Sprite[] humanStates;
    public GameObject buildingStats;
    
    [Header("Add Humans")]
    public Collider2D addCol;
    public GameObject addButton;
    public List<VillagerAI> villagers = new List<VillagerAI>();
    
    [Header("Remove Selected Humans")]
    public Collider2D deleteCol;
    public GameObject deleteButton;

    [Header("For Saving System")]
    public int buildingId; //Basically what Id to use when referring to its prefab when loading

    public bool isChoosing;
    public bool isDisowning;

    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        gridTaken = GetComponent<GridTaken>();
    }

    protected virtual void Start()
    {
        if(!GridManager.instance.buildings.Contains(this))
        {
            GridManager.instance.buildings.Add(this);
        }
        HideVisuals();
    }

    protected virtual void Update()
    {
        if ((Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) && !ViewMode.instance.viewMode)
        {
            if (CameraPinch.Instance != null && CameraPinch.Instance.IsPanning) 
            {
                return;
            }

            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (col == Physics2D.OverlapPoint(clickPos) && !isChoosing && !TownManager.instance.isBuilding && TownManager.instance.activeBuilding == null)
            {
                OnSpriteClicked();
            }
            else if(addCol == Physics2D.OverlapPoint(clickPos) && villagers.Count < humanIcons.Length && !isChoosing)
            {
                AddHuman();
            }
            else if(deleteCol == Physics2D.OverlapPoint(clickPos) && humanIcons.Length > 0 && !isChoosing)
            {
                RemoveHuman();
            }
            // else if(isShowing && !isChoosing && (TownManager.instance.activeBuilding != this || TownManager.instance.activeBuilding == null))
            // {
            //     HideVisuals();
            // }
        }

        if(CameraPinch.Instance.IsPanning && isShowing && !isChoosing && (TownManager.instance.activeBuilding != this || TownManager.instance.activeBuilding == null))
        {
            HideVisuals();
        }
    }

    protected virtual void OnSpriteClicked()
    {
        BothVisuals();
    }

    public void BothVisuals()
    {
        if(TownManager.instance.availableMarket != null && TownManager.instance.availableMarket.isShowing) return;
        
        isShowing = !isShowing;

        if(isShowing)
        {
            ShowVisuals();
        }
        else
        {
            HideVisuals();
        }
    }

    public virtual void ShowVisuals()
    {
        UpdateVisuals();
        rend.sprite = buildingStates[1];
        buildingStats.gameObject.SetActive(true);
        isShowing = true;
        TownManager.instance.ShowSelectedHumans(this);
    }

    public virtual void HideVisuals()
    {
        rend.sprite = buildingStates[0];
        buildingStats.gameObject.SetActive(false);
        isShowing = false;
        TownManager.instance.HideSelectedHumans(this);
    }

    public virtual void UpdateVisuals()
    {
        addButton.SetActive(villagers.Count < humanIcons.Length);
        deleteButton.SetActive(villagers.Count > 0);

        for(int i = 0; i < humanIcons.Length; i++)
        {
            if(i < villagers.Count && villagers[i] != null)
            {
                humanIcons[i].sprite = humanStates[1];
            }
            else
            {
                humanIcons[i].sprite = humanStates[0];
            }
        }
    }

    protected virtual void AddHuman()
    {
        isChoosing = true;
        TownManager.instance.activeBuilding = this;
        TownManager.instance.SelectingHumanMode(this);
    }

    public void RemoveHuman()
    {
        isChoosing = true;
        isDisowning = true;
        TownManager.instance.activeBuilding = this;
        TownManager.instance.SelectingHumanMode(this, true);
    }

    public virtual void AssignVillagerRole(VillagerAI villager)
    {
        villager.house = this.transform;
    }
    public virtual void RemoveVillagerRole(VillagerAI villager)
    {
        villager.house = null;
    }
}
