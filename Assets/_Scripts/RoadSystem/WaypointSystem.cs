using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class WaypointSystem : MonoBehaviour
{
    public static WaypointSystem instance {get; private set;}
    private RoadSystem roadSystem;
    private List<Vector2Int> allPos = new List<Vector2Int>();
    public List<Vector2Int> waypoints = new List<Vector2Int>();
    public HashSet<Vector2Int> waypointsHash = new HashSet<Vector2Int>();

    void Awake()
    {
        instance = this;
        roadSystem = RoadSystem.instance;
    }

    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.U))
    //     {
    //         GenerateWaypoints();
    //     }
    // }

    public void GenerateWaypoints()
    {
        waypoints.Clear();

        if(roadSystem.roadPos.Count == 0)
        {
            return;
        }

        foreach(var road in roadSystem.roadPos)
        {
            bool suitable = CheckWaypointSuitability(road.Key);
            if(suitable)
            {
                waypoints.Add(road.Key);
                waypointsHash.Add(road.Key);
                // Debug.Log(waypoints.Count);
            }
        }
    }

    bool CheckWaypointSuitability(Vector2Int roadPos)
    {
        allPos.Clear();

        Vector2Int leftPos = new Vector2Int(roadPos.x - 1, roadPos.y);
        Vector2Int rightPos = new Vector2Int(roadPos.x + 1, roadPos.y);
        Vector2Int upPos = new Vector2Int(roadPos.x, roadPos.y + 1);
        Vector2Int downPos = new Vector2Int(roadPos.x, roadPos.y - 1);

        allPos.Add(leftPos);
        allPos.Add(rightPos);
        allPos.Add(upPos);
        allPos.Add(downPos);

        //Dead end road
        int roadNearby = 0;
        foreach(var pos in allPos)
        {
            if(roadSystem.roadPos.ContainsKey(pos))
            {
                roadNearby++;
            }
        }

        if(roadNearby == 1 || roadNearby == 4) //Checking for dead ends and 4-Way Intersections
        {
            return true;
        }

        //Checking corners and/or Intersections
        if(roadSystem.roadPos.ContainsKey(leftPos) && (roadSystem.roadPos.ContainsKey(downPos) || roadSystem.roadPos.ContainsKey(upPos)) && !roadSystem.roadPos.ContainsKey(rightPos))
        {
            return true;
        }
        else if(roadSystem.roadPos.ContainsKey(rightPos) && (roadSystem.roadPos.ContainsKey(downPos) || roadSystem.roadPos.ContainsKey(upPos)) && !roadSystem.roadPos.ContainsKey(leftPos))
        {
            return true;
        }
        else if(roadSystem.roadPos.ContainsKey(downPos) && (roadSystem.roadPos.ContainsKey(leftPos) || roadSystem.roadPos.ContainsKey(rightPos)) && !roadSystem.roadPos.ContainsKey(upPos))
        {
            return true;
        }
        else if(roadSystem.roadPos.ContainsKey(upPos) && (roadSystem.roadPos.ContainsKey(leftPos) || roadSystem.roadPos.ContainsKey(rightPos)) && !roadSystem.roadPos.ContainsKey(downPos))
        {
            return true;
        }


        return false;
    }
}
