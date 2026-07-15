using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class VillagerDead : MonoBehaviour
{
    public Virus inflictedVirus;
    public float infectionMultiplyer = 3f;
    public float infectionCooldown = 10f;
    public float tileRange = 1f;
    public LayerMask villagerLayer;

    private Collider2D col;

    void Start()
    {
        if(!TownManager.instance.deadVillagers.Contains(this))
        {
            TownManager.instance.deadVillagers.Add(this);
        }
        col = GetComponent<Collider2D>();
        StartCoroutine(InfectOthers());
    }

    void Update() //Debug for now, will remove when lab villagers exist
    {
        if ((Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) && !ViewMode.instance.viewMode)
        {
            if (CameraPinch.Instance != null && CameraPinch.Instance.IsPanning) 
            {
                return;
            }

            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (col == Physics2D.OverlapPoint(clickPos))
            {
                RemoveCorpse();
            }
        }
    }

    public void RemoveCorpse()
    {
        if(TownManager.instance.deadVillagers.Contains(this)) TownManager.instance.deadVillagers.Remove(this);

        Destroy(gameObject);
    }

    IEnumerator InfectOthers()
    {
        float infectionChance = Mathf.Min(inflictedVirus.infection * infectionMultiplyer, 100f);
        Vector2 boxSize = new Vector2(tileRange, tileRange);

        while(true)
        {
            yield return new WaitForSeconds(infectionCooldown);
            float chance = Random.Range(0, 100f);

            if(chance <= infectionChance)
            {
                Collider2D hit = Physics2D.OverlapBox(transform.position, boxSize, 0f, villagerLayer);

                if(hit != null)
                {
                    if(hit.gameObject.TryGetComponent(out VillagerAI villager))
                    {
                        villager.villagerHealth.Infect(inflictedVirus);
                    }
                }
            }
        }
    }

}