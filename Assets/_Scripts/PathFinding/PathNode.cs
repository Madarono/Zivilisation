public class PathNode
{
    public int x;
    public int y;

    public int gCost; // Distance from start node
    public int hCost; // Distance to end node
    public int fCost; // Total cost (gCost + hCost)

    public bool isWalkable;
    public PathNode cameFromNode; // The previous tile in the path (trace backward)

    public PathNode(int x, int y, bool isWalkable)
    {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
}