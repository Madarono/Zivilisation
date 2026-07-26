using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public enum QuarantineState
{
    Able,
    Timeout
}

public class Quarantine : Building, VillageBuildable
{
    [Header("Time Inside")]
    public float timeInsideDuration = 900f; // 15 mins
    public float timeInsideCurrent = 0f;

    [Header("Time Outside (Cooldown)")]
    public float timeoutDuration = 1200f;  // 20 mins
    public float timeoutCurrent = 0f;
    
    public QuarantineState state = QuarantineState.Able;

    public Coroutine insideCoroutine;
    public Coroutine outsideCoroutine;

    protected override void Start()
    {
        if(TownManager.instance.availableQuarantine == null) TownManager.instance.availableQuarantine = this;

        if (!GridManager.instance.buildings.Contains(this))
        {
            GridManager.instance.buildings.Add(this);
        }

        HideVisuals();
    }
    
    public override void ShowVisuals()
    {
        UpdateVisuals();
        rend.sprite = buildingStates[1];
        buildingStats.gameObject.SetActive(true);
        isShowing = true;
    }

    public override void HideVisuals()
    {
        rend.sprite = buildingStates[0];
        buildingStats.gameObject.SetActive(false);
        isShowing = false;
    }

    protected override void AddHuman()
    {
        if (state == QuarantineState.Timeout)
        {
            float minutesLeft = Mathf.Ceil((timeoutDuration - timeoutCurrent) / 60f);
            PopupText.instance.Popup($"This quarantine is on cooldown for another {minutesLeft} minutes.");
            return;
        }

        isChoosing = true;
        TownManager.instance.activeBuilding = this;
        TownManager.instance.SelectingHumanMode(this, false, true);
    }

    public override void RemoveHuman()
    {
        if (villagers.Count == 0) return;

        TownManager.instance.RemoveHumanManually(villagers[0], this);

        if (insideCoroutine != null) 
        {
            StopCoroutine(insideCoroutine);
            insideCoroutine = null;
            timeInsideCurrent = 0f;
        }

        StopAllCoroutines();

        if (outsideCoroutine == null) 
        {
            outsideCoroutine = StartCoroutine(OutsideDuration());
        }
    }

    public override void AssignVillagerRole(VillagerAI villager)
    {
        villager.quarantine = this.transform;
        villager.villagerSprite.DeSelected();
        villager.MoveVillager(transform, 0, -1);

        // Clear existing roles/locations
        if (villager.house != null && villager.house.gameObject.TryGetComponent(out Building houseBuilding))
        {
            TownManager.instance.RemoveHumanManually(villager, houseBuilding);
        }

        if (villager.jobPlace != null && villager.jobPlace.gameObject.TryGetComponent(out Building jobBuilding))
        {
            TownManager.instance.RemoveHumanManually(villager, jobBuilding);
        }

        if (insideCoroutine == null) 
        {
            insideCoroutine = StartCoroutine(InsideDuration());
        }
    }

    public override void RemoveVillagerRole(VillagerAI villager)
    {
        villager.quarantine = null;
        villager.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
    }

    public void HideVillager()
    {
        if (villagers.Count == 0) return;
        villagers[0].rend.sprite = null;
    }

    public IEnumerator InsideDuration()
    {
        while (timeInsideCurrent < timeInsideDuration)
        {
            timeInsideCurrent += Time.deltaTime;
            yield return null;
        }

        timeInsideCurrent = 0f;
        insideCoroutine = null;

        RemoveHuman();
    }

    public IEnumerator OutsideDuration()
    {
        state = QuarantineState.Timeout;

        while (timeoutCurrent < timeoutDuration)
        {
            timeoutCurrent += Time.deltaTime;
            yield return null;
        }

        timeoutCurrent = 0f;
        state = QuarantineState.Able;
        outsideCoroutine = null;
    }

    public void StartInside()
    {
        if(insideCoroutine == null) StartCoroutine(InsideDuration());
    }

    public void StartOutside()
    {
        if(outsideCoroutine == null) StartCoroutine(OutsideDuration());
    }
}