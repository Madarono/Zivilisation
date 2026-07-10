using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance { get; private set; }

    public Gate gate;
    public HashSet<VillageBuildable> buildings = new HashSet<VillageBuildable>();
    public HashSet<Vector2Int> placesTaken = new HashSet<Vector2Int>(); 

    [Header("Dimensions")]
    public int width = 256;
    public int height = 256;

    public bool[,] walkableGrid;
    
    private CustomPathfinding pathfinder; 
    private Queue<PathRequest> requestQueue = new Queue<PathRequest>();
    private bool isProcessingPath = false;

    private struct PathRequest
    {
        public Vector2Int start;
        public Vector2Int end;
        public System.Action<List<Vector2Int>> callback;

        public PathRequest(Vector2Int start, Vector2Int end, System.Action<List<Vector2Int>> callback)
        {
            this.start = start;
            this.end = end;
            this.callback = callback;
        }
    }

    void Awake()
    {
        instance = this;
        InitializeGrid();
    }

    void Start()
    {
        foreach(var building in buildings)
        {
            if(building is Building buildingScript)
            {
                buildingScript.gridTaken.MeasureGridTaken();
                buildingScript.gridTaken.PutUnwalkable();
            }
        }

        gate.gridTaken.MeasureGridTaken();
        gate.gridTaken.PutUnwalkable();

        pathfinder = new CustomPathfinding(width, height, walkableGrid);
    }

    private void InitializeGrid()
    {
        walkableGrid = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                walkableGrid[x, y] = false; 
            }
        }
    }

    public void PlaceBuilding(int x, int y)
    {
        if (IsValidCoordinate(x, y))
        {
            walkableGrid[x, y] = false;
            pathfinder?.UpdateNodeWalkability(x, y, false);
        }
    }

    public void PlaceRoad(int x, int y)
    {
        if(IsValidCoordinate(x, y))
        {
            walkableGrid[x, y] = true;
            pathfinder?.UpdateNodeWalkability(x, y, true);
        }
    }

    public bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void RequestPath(Vector2Int startCoordinates, Vector2Int targetCoordinates, System.Action<List<Vector2Int>> callback)
    {
        requestQueue.Enqueue(new PathRequest(startCoordinates, targetCoordinates, callback));
        
        if (!isProcessingPath)
        {
            StartCoroutine(ProcessNextPath());
        }
    }

    private IEnumerator ProcessNextPath()
    {
        isProcessingPath = true;

        while (requestQueue.Count > 0)
        {
            PathRequest currentRequest = requestQueue.Dequeue();

            List<Vector2Int> calculatedPath = pathfinder.FindPath(
                currentRequest.start.x, currentRequest.start.y, 
                currentRequest.end.x, currentRequest.end.y
            );

            currentRequest.callback?.Invoke(calculatedPath);

            yield return null; 
        }

        isProcessingPath = false;
    }

    public Vector2Int GetVector2Int(Transform transform)
    {
        return new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }
}