using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CustomPathfinding
{
    private int gridWidth;
    private int gridHeight;
    private PathNode[,] gridNodes;

    public CustomPathfinding(int width, int height, bool[,] walkableData)
    {
        this.gridWidth = width;
        this.gridHeight = height;
        gridNodes = new PathNode[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNodes[x, y] = new PathNode(x, y, walkableData[x, y]);
            }
        }
    }

    public List<Vector2Int> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = gridNodes[startX, startY];
        PathNode endNode = gridNodes[endX, endY];

        List<PathNode> openList = new List<PathNode> { startNode };
        HashSet<PathNode> closedList = new HashSet<PathNode>();

        // Reset all nodes from any previous pathfinding calculations
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                PathNode n = gridNodes[x, y];
                n.gCost = int.MaxValue; // Start at infinity
                n.CalculateFCost();
                n.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            // Find the node in the open list with the lowest F Cost
            PathNode currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            if (currentNode == endNode)
            {
                return CalculatePathRoute(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbor in GetNeighbors(currentNode))
            {
                if (closedList.Contains(neighbor) || !neighbor.isWalkable) continue;

                int tentativeGCost = currentNode.gCost + 10; // 10 represents a movement cost of 1 tile
                if (tentativeGCost < neighbor.gCost)
                {
                    neighbor.cameFromNode = currentNode;
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = CalculateDistanceCost(neighbor, endNode);
                    neighbor.CalculateFCost();

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null; 
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y)) * 10;
    }

    private List<PathNode> GetNeighbors(PathNode node)
    {
        List<PathNode> neighbors = new List<PathNode>();
        if (node.x - 1 >= 0) neighbors.Add(gridNodes[node.x - 1, node.y]); // Left
        if (node.x + 1 < gridWidth) neighbors.Add(gridNodes[node.x + 1, node.y]); // Right
        if (node.y - 1 >= 0) neighbors.Add(gridNodes[node.x, node.y - 1]); // Down
        if (node.y + 1 < gridHeight) neighbors.Add(gridNodes[node.x, node.y + 1]); // Up
        return neighbors;
    }

    private List<Vector2Int> CalculatePathRoute(PathNode endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        PathNode currentNode = endNode;
        while (currentNode != null)
        {
            path.Add(new Vector2Int(currentNode.x, currentNode.y));
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    public void UpdateNodeWalkability(int x, int y, bool isWalkable)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            gridNodes[x, y].isWalkable = isWalkable;
        }
    }
}