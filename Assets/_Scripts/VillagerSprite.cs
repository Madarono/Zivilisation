using UnityEngine;

[System.Serializable]
public class VillagerLook
{
    public Sprite[] look;
    public int id;
}

public class VillagerSprite : MonoBehaviour
{
    public VillagerAI villager;
    public VillagerLook[] villagerLook;
    
    public bool isHighlighted; 

    void Start()
    {
        villager ??= GetComponent<VillagerAI>();
        UpdateLooks();
    }
    
    public void UpdateLooks()
    {
        if(villager.state == VillagerState.Sleeping)
        {
            villager.rend.enabled = false;
            return;
        }

        villager.rend.enabled = true;

        if (isHighlighted)
        {
            villager.rend.sprite = villagerLook[villager.jobPlaceID].look[1];
        }
        else
        {
            villager.rend.sprite = villagerLook[villager.jobPlaceID].look[villager.isShowing ? 1 : 0];
        }
    }

    public void Selected()
    {
        if(villager.quarantine != null || villager.state == VillagerState.Sleeping) return;

        isHighlighted = true;
        UpdateLooks();
    }

    public void DeSelected()
    {
        if(villager.quarantine != null || villager.state == VillagerState.Sleeping) return;

        isHighlighted = false;
        UpdateLooks();
    }
}