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
    }
    
    public void UpdateLooks()
    {
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
        isHighlighted = true;
        UpdateLooks();
    }

    public void DeSelected()
    {
        isHighlighted = false;
        UpdateLooks();
    }
}