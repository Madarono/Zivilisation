using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum VillagerState
{
    Moving,
    Idle,
    Sleeping,
    Working
}

[System.Serializable]
public class RequirementSprite
{
    public float req;
    public Sprite sprite;
}

public class VillagerAI : MonoBehaviour
{
    public VillagerHealth villagerHealth;
    public VillagerState state;
    public VillagerPathFind villagerPF;
    public VillagerSprite villagerSprite;
    public Collider2D col;

    [Header("Attributes")]
    public float hunger = 1f;
    public float hungerDepletionAmount = 0.1f;
    public float hungerDepletionCooldown = 10f;
    public bool isCoughing = false;

    [Header("Icon Visuals")]
    public bool isShowing;
    public GameObject villagerStatsWindow;
    public SpriteRenderer hungerIcon;
    public RequirementSprite[] hungerStates;
    public SpriteRenderer houseIcon;
    public Sprite[] houseStates;

    public SpriteRenderer jobIcon;
    public Sprite[] jobStates;


    [Header("State: Moving")]
    public float villagerSpeed = 2f;

    private Vector3 finalDestination;
    private bool canWander;
    private Coroutine hungerDown;
    public SpriteRenderer rend;

    [Header("State: Wandering")]
    public float minWanderingCooldown = 1f;
    public float maxWanderingCooldown = 5f;

    private float wanderTimer;

    [Header("House")]
    public LayerMask villagerLayer;
    public GameObject failed;
    public Collider2D failedCol;
    public Transform house;
    public bool goingToHouse;

    [Header("Job")]
    public Transform jobPlace;
    public int jobPlaceID = 0;

    [Header("Quarantine")]
    public Transform quarantine;

    [Header("Death")]
    public GameObject deathPrefab;

    [Header("Death - ScreenShake")]
    public float deathDuration;
    public float deathMagnitude;

    public Transform cacheTarget {get; private set;}
    private int cacheOffsetX;
    private int cacheOffsetY;
    private List<Vector2Int> allPos = new List<Vector2Int>();

    public bool hasWarnedInsomnia = false;
    

    public void Start()
    {
        if(!TownManager.instance.villagers.Contains(this))
        {
            TownManager.instance.villagers.Add(this);
        }
        isShowing = false;
        goingToHouse = false;
        state = VillagerState.Idle;
        rend = GetComponent<SpriteRenderer>();
        villagerPF = GetComponent<VillagerPathFind>();
        villagerSprite = GetComponent<VillagerSprite>();
        villagerHealth = GetComponent<VillagerHealth>();
        failedCol = failed.GetComponent<Collider2D>();
        col = GetComponent<Collider2D>();
        canWander = false;
        hungerDown = StartCoroutine(HungerGoDown());
        HideVisuals(false);
        // StartCoroutine(Wandering());
    }

    void Update()
    {
        if(LoseCondition.instance.lost) return;
        if(Settings.instance.isOpen || ActiveWindow.instance.isActive || ActiveWindow.instance.briefActive) return;
        
        if(jobPlace == null) 
        {
            jobPlaceID = 0;
        }

        if(CameraPinch.Instance.IsPanning && isShowing && !goingToHouse)
        {
            HideVisuals();
        }

        if (goingToHouse || state == VillagerState.Moving || state == VillagerState.Working || state == VillagerState.Sleeping || isCoughing)
        {
            wanderTimer = 1f;
        }
        else 
        {
            if (wanderTimer > 0)
            {
                wanderTimer -= Time.deltaTime;
            }
            else
            {
                if (TownManager.instance.RequestWanderSlot())
                {
                    if (!CheckWaypoint())
                    {
                        TownManager.instance.ReleaseWanderSlot();
                        wanderTimer = Random.Range(5f, 15f);
                    }
                    else if (WaypointSystem.instance.waypoints.Count > 0)
                    {
                        int random = Random.Range(0, WaypointSystem.instance.waypoints.Count);
                        Vector2Int randomPos = WaypointSystem.instance.waypoints[random];
                        
                        MoveVillager(null, 0, 0, randomPos);
                        wanderTimer = Random.Range(10f, 30f); 
                    }
                    else
                    {
                        TownManager.instance.ReleaseWanderSlot();
                        wanderTimer = 5f;
                    }
                }
                else
                {
                    // Pool is full
                    wanderTimer = Random.Range(3f, 7f);
                }
            }
        }
    }

    public void HandleSelection() //From TownManager.cs
    {
        if (TownManager.instance.activeBuilding != null)
        {
            if (TownManager.instance.activeBuilding.isDisowning)
            {
                if (TownManager.instance.activeBuilding.villagers.Contains(this))
                {
                    TownManager.instance.RemoveHuman(this);
                }
            }
            else
            {
                bool isWorkplace = (TownManager.instance.activeBuilding is Farm) || (TownManager.instance.activeBuilding is Mines);
                bool isQuarantine = TownManager.instance.activeBuilding is Quarantine;
                
                if (isWorkplace && !isQuarantine && jobPlace == null)
                {
                    TownManager.instance.SelectedHuman(this);
                }
                else if (!isWorkplace && !isQuarantine && house == null)
                {
                    TownManager.instance.SelectedHuman(this);
                }
                else if (!isWorkplace && isQuarantine && villagerHealth.health != Health.Healthy)
                {
                    TownManager.instance.SelectedHuman(this);
                }
            }
        }
        else if (!goingToHouse && !TownManager.instance.isBuilding && quarantine == null)
        {          
            OnSpriteClicked();
        }
    }
    
    public void HandleFailedClick() //From TownManager.cs
    {
        if (TownManager.instance.activeBuilding == null && !goingToHouse && !TownManager.instance.isBuilding)
        {
            MoveVillager(cacheTarget, cacheOffsetX, cacheOffsetY);
        }
    }


    void OnSpriteClicked()
    {
        if(TownManager.instance.availableMarket != null && TownManager.instance.availableMarket.isShowing) return;
        if(TownManager.instance.availableLaboratory != null && TownManager.instance.availableLaboratory.isShowing) return;

        if(goingToHouse)
        {
            return;
        }

        //This aims to only have one Villager's status shown and avoid doing too many villagerDeselect Audio from multiple villagers
        if(TownManager.instance.currentVillager != null && TownManager.instance.currentVillager != this) TownManager.instance.currentVillager.HideVisuals(false);

        TownManager.instance.currentVillager = this;
        BothVisuals();
    }

    public void BothVisuals()
    {
        if(rend.sprite == null)
        {
            return;
        }

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

    public void ShowVisuals()
    {
        UpdateVisuals();
        villagerStatsWindow.gameObject.SetActive(true);
        isShowing = true;
        villagerSprite.UpdateLooks();

        if(TownManager.instance.currentBuilding != null)
        {
            TownManager.instance.currentBuilding.HideVisuals(false);
            TownManager.instance.currentBuilding = null;
        }

        AudioManager.instance.Play(AudioManager.instance.villagerSelect);
    }

    public void HideVisuals(bool sound = true)
    {
        villagerStatsWindow.gameObject.SetActive(false);
        isShowing = false;
        villagerSprite.UpdateLooks();
        if(sound) AudioManager.instance.Play(AudioManager.instance.villagerDeselect);

        if(TownManager.instance.currentVillager != null && TownManager.instance.currentVillager == this) TownManager.instance.currentVillager = null;
    }

    void UpdateVisuals()
    {
        int hungerId = 0;
        for(int i = 0; i < hungerStates.Length; i++)
        {
            if(hunger > hungerStates[i].req)
            {
                continue;
            }

            hungerId = i;
            break;
        }
        hungerIcon.sprite = hungerStates[hungerId].sprite;

        houseIcon.sprite = house == null ? houseStates[0] : houseStates[1];
        jobIcon.sprite = jobPlace == null ? jobStates[0] : jobStates[1];
    }

    public void FinishedPathfinding()
    {
        if (state == VillagerState.Moving && cacheTarget == null)
        {
            TownManager.instance.ReleaseWanderSlot();
        }

        if(goingToHouse)
        {
            rend.sprite = null;
            state = VillagerState.Sleeping;
            return;
        }
        else if(state == VillagerState.Working)
        {
            if(jobPlace.TryGetComponent(out Farm farm)) farm.StartFarming();
            else if(jobPlace.TryGetComponent(out Mines mines)) mines.StartMining();
        }
        else if(quarantine != null && cacheTarget == quarantine)
        {
            if(quarantine.TryGetComponent(out Quarantine quarantineScript))
            {
                quarantineScript.HideVillager();
            }
        }
        else
        {    
            state = VillagerState.Idle;
        }
    }

    public void FailedPathfinding()
    {
        Debug.Log("Failed Pathfinding..");

        if (state == VillagerState.Moving && cacheTarget == null)
        {
            TownManager.instance.ReleaseWanderSlot();
        }

        failed.SetActive(true);
        state = VillagerState.Idle;
        goingToHouse = false;
        wanderTimer = Random.Range(5f, 15f); 
    }

    bool CheckWaypoint()
    {
        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        
        if (WaypointSystem.instance.waypointsHash.Contains(pos) ||
            WaypointSystem.instance.waypointsHash.Contains(new Vector2Int(pos.x - 1, pos.y)) ||
            WaypointSystem.instance.waypointsHash.Contains(new Vector2Int(pos.x + 1, pos.y)) ||
            WaypointSystem.instance.waypointsHash.Contains(new Vector2Int(pos.x, pos.y + 1)) ||
            WaypointSystem.instance.waypointsHash.Contains(new Vector2Int(pos.x, pos.y - 1)))
        {
            return true;
        }

        return false;
    }

    public void MoveVillager(Transform target, int offsetX, int offsetY, Vector2Int? assignedPos = null)
    {
        failed.SetActive(false);
        state = VillagerState.Moving;
        Vector2Int pos = Vector2Int.zero;

        if(assignedPos != null)
        {
            pos = assignedPos.Value;
        }
        else if(target != null)
        {
            pos = GridManager.instance.GetVector2Int(target);   
            pos = new Vector2Int(pos.x + offsetX, pos.y + offsetY);
        }
        else
        {
            state = VillagerState.Idle;
            return;
        }

        goingToHouse = (target == house && house != null);
        villagerPF.OrderMoveTo(pos);

        cacheTarget = target;
        cacheOffsetX = offsetX;
        cacheOffsetY = offsetY;
    }

    public void MoveVillagerToHouse()
    {
        if(house == null || villagerHealth.hasExhausionInsomnia)
        {
            if(villagerHealth.hasExhausionInsomnia && house != null && !hasWarnedInsomnia) 
            {
                PopupText.instance.Popup("A villager can't sleep due to insomnia");
                hasWarnedInsomnia = true;
            }
            return;
        }

        failed.SetActive(false);
        state = VillagerState.Moving;
        Vector2Int pos = GridManager.instance.GetVector2Int(house);
        pos = new Vector2Int(pos.x, pos.y - 1);
        goingToHouse = true;
        villagerPF.OrderMoveTo(pos);

        cacheTarget = house;
        cacheOffsetX = 0;
        cacheOffsetY = -1;
    }

    public void GoToWork()
    {
        if(jobPlace == null || villagerHealth.hasExhausionInsomnia)
        {
            if(villagerHealth.hasExhausionInsomnia && jobPlace != null && !hasWarnedInsomnia) 
            {
                PopupText.instance.Popup("A villager can't work due to insomnia");
                hasWarnedInsomnia = true;
            }
            return;
        }

        villagerPF.CancelMovement();
        state = VillagerState.Working;
        Vector2Int pos = GridManager.instance.GetVector2Int(jobPlace);
        pos = new Vector2Int(pos.x, pos.y - 1);
        villagerPF.OrderMoveTo(pos);

        cacheTarget = jobPlace;
        cacheOffsetX = 0;
        cacheOffsetY = -1;
    }

    public void WakeUpFromHouse()
    {
        if(house == null)
        {
            return;
        }

        goingToHouse = false;
        villagerSprite.UpdateLooks();
        state = VillagerState.Idle;
        hasWarnedInsomnia = false;
    }

    public void Cough(float duration) //From VillagerHealth.cs
    {
        villagerPF.CancelMovement();
        StartCoroutine(StopToCough(duration));
    }

    [ContextMenu("Kill this villager")]
    public void DeathWithCorpse()
    {
        Death(true);
    }

    [ContextMenu("Desert this villager")]
    public void DeathWithoutCorpse()
    {
        Death(false);
    }

    public void Death(bool withCorpse = true) //From VillagerHealth.cs
    {
        TownManager.instance.villagers.Remove(this);
        if(house != null)
        {
            if(house.TryGetComponent(out Building houseBuilding))
            {
                houseBuilding.RemoveVillagerRole(this);
            }
        }
        if(jobPlace != null)
        {
            if(jobPlace.TryGetComponent(out Building jobBuilding))
            {
                jobBuilding.RemoveVillagerRole(this);
            }
        }

        TownManager.instance.totalDead++;
        if(LoseCondition.instance.LossByPopulation())
        {
            LoseCondition.instance.CheckLossCondition();
        }

        if(!withCorpse) //Leave due to morality
        {
            Stats.instance.desertions++;

            PopupText.instance.Popup("A villager has left your village");
            AudioManager.instance.Play(AudioManager.instance.villagerLeave);
            Destroy(gameObject);
            return;
        }

        Vector2 posInt = new Vector2((int)transform.position.x, (int)transform.position.y);
        GameObject go = Instantiate(deathPrefab, posInt, Quaternion.identity);

        if(go.TryGetComponent(out VillagerDead goScript))
        {
            goScript.inflictedVirus = villagerHealth.inflictedVirus;
        }

        PopupText.instance.Popup("A villager has died.");

        if(Settings.instance.canScreenShake) PixelCameraShake.instance.Shake(deathDuration, deathMagnitude);
        AudioManager.instance.Play(AudioManager.instance.villagerDie);

        Destroy(gameObject);
    }

    //Coroutines
    IEnumerator HungerGoDown()
    {
        float depletion = hungerDepletionAmount * villagerHealth.currentHungerMultiplyer;
        while(true)
        {
            yield return new WaitForSeconds(hungerDepletionCooldown);
            if(goingToHouse)
            {
                continue;
            }

            depletion = hungerDepletionAmount * villagerHealth.currentHungerMultiplyer;
            hunger = Mathf.Clamp01(hunger - depletion);

            //For Anytime Feed
            if(HungerManager.instance.feedState == FeedTime.Anytime)
            {
                
                if(hunger > HungerManager.instance.minFullFeed && HungerManager.instance.percentageState == FeedPer.FullFeed) continue;
                if(hunger > HungerManager.instance.minBareFeed && HungerManager.instance.percentageState == FeedPer.BareMinimum) continue;

                HungerManager.instance.FeedVillager(this);
            }
        }
    }

    IEnumerator StopToCough(float duration)
    {
        isCoughing = true;
        yield return new WaitForSeconds(duration);
        isCoughing = false;

        wanderTimer = 0.5f;
    }
}
