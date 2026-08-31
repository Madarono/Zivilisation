using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Laboratory : Building, VillageBuildable
{
    [Line]
    [CenteredHeader("--- For Laboratory ---", 20)]
    public GameObject laboratoryWindow;

    protected override void Start()
    {
        laboratoryWindow = TownManager.instance.laboratoryWindow;

        if(TownManager.instance.availableLaboratory == null) TownManager.instance.availableLaboratory = this;

        if (!GridManager.instance.buildings.Contains(this))
        {
            GridManager.instance.buildings.Add(this);
        }

        HideVisuals();
    }

    protected override void Update()
    {
        if(LoseCondition.instance.lost) return;
        if(Settings.instance.isOpen || ActiveWindow.instance.isActive || ActiveWindow.instance.briefActive) return;
        
        bool isClick = Input.GetMouseButtonDown(0);
        bool isTouch = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;

        if ((isClick || isTouch) && !ViewMode.instance.viewMode)
        {
            if (EventSystem.current != null)
            {
                if (isClick && EventSystem.current.IsPointerOverGameObject() && isShowing) 
                {
                    return; 
                }
                if (isTouch && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && isShowing) 
                {
                    return; 
                }
            }

            if (CameraPinch.Instance != null && CameraPinch.Instance.IsPanning) 
            {
                return;
            }

            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (col == Physics2D.OverlapPoint(clickPos) && !isChoosing && !TownManager.instance.isBuilding && TownManager.instance.activeBuilding == null)
            {
                OnSpriteClicked();
            }
        }
    }

    protected override void OnSpriteClicked()
    {
        ShowVisuals();
    }

    public override void ShowVisuals()
    {
        TownManager.instance.OpenLaboratoryWindow();
    }

    public override void HideVisuals()
    {
        // isShowing = false;
        // LaboratorySystem.instance.HideAllVisuals();
    }

    public override void UpdateVisuals()
    {
    }
}