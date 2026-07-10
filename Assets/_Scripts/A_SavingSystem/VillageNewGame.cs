using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class BuildingPlacement
{
    public GameObject prefab;
    public Vector2Int placement;
}
public class VillageNewGame : MonoBehaviour
{
    public static VillageNewGame instance {get; private set;}

    [Header("Road Placement")]
    public Vector2Int[] roadPlacement;

    [Header("Villager Placement")]
    public Vector2Int[] villagerPlacement;

    [Header("Building Placement")]
    public BuildingPlacement[] buildingPlacement;

    void Awake()
    {
        instance = this;
    }

    public void DeletePrevious()
    {
        Gate gate = Gate.instance;
        RoadSystem road = RoadSystem.instance;
        GridManager grid = GridManager.instance;
        TownManager town = TownManager.instance;

        road.DeleteAllRoads();

        gate.PutRoadUnder();
        
        foreach(var building in grid.buildings)
        {
            Building buildingScript = building as Building;
            Destroy(buildingScript.gameObject);
        }

        grid.buildings.Clear();
        grid.placesTaken.Clear();

        foreach(var villager in town.villagers)
        {
            Destroy(villager.gameObject);
        }

        town.villagers.Clear();
    }
    public void InitializeNewGame()
    {
        Gate gate = Gate.instance;
        RoadSystem road = RoadSystem.instance;
        BuildOptions build = BuildOptions.instance;

        foreach(var roadPlacement in roadPlacement)
        {
            road.PutRoad(roadPlacement);
        }

        foreach(var villagerPos in villagerPlacement)
        {
            GameObject newVillager = gate.SpawnNewVillagerInfo();
            Vector3 newVillagerPos = new Vector3(villagerPos.x, villagerPos.y, 0);
            newVillager.transform.position = newVillagerPos;
        }

        foreach(var building in buildingPlacement)
        {
            Vector2 buildingPos = new Vector2(building.placement.x, building.placement.y);

            build.SpawnOption(building.prefab, buildingPos);
        }

        TownStorage.instance.money = 100;
        TownStorage.instance.wheat = 3f;
        TownStorage.instance.iron = 0;
        TownStorage.instance.copper = 0;
        TownStorage.instance.quartz = 0;
        TownStorage.instance.titanium = 0;

        DayCycle.instance.hours = 6;
        DayCycle.instance.minutes = 0;
        DayCycle.instance.seconds = 0;
        DayCycle.instance.UpdateClock();
    }

    [ContextMenu("Debug NewGame")]
    private void DebugNewGame()
    {
        DataPersistenceManager.instance.NewGame();
    }
}