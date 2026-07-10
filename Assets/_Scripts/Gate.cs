using UnityEngine;

public class Gate : MonoBehaviour
{
    public static Gate instance {get; private set;}
    public GridTaken gridTaken;
    public GameObject villagerPrefab;
    private Vector2Int underPos;
    
    private GameObject newVillagerSpawned;

    [Header("Villagers Spawn Debug")]
    public int villagerSpawnAmount = 1;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        PutRoadUnder();
    }

    public void PutRoadUnder()
    {
        underPos = new Vector2Int((int)transform.position.x, (int)transform.position.y - 1);
        RoadSystem.instance.PutRoad(underPos);
    }
    public void SpawnNewVillager()
    {
        Vector3 goPos = new Vector3(underPos.x, underPos.y, 0);

        GameObject go = Instantiate(villagerPrefab, goPos, Quaternion.identity);
        
        if(go.TryGetComponent(out VillagerAI goScript))
        {
            TownManager.instance.villagers.Add(goScript);
        }

        newVillagerSpawned = go;
    }

    public GameObject SpawnNewVillagerInfo()
    {
        SpawnNewVillager();

        return newVillagerSpawned;
    }

    [ContextMenu("SpawnVillagersDebug Function")]
    private void SpawnVillagersDebug()
    {
        for(int i = 0; i < villagerSpawnAmount; i++)
        {
            SpawnNewVillager();
        }
    }
}
