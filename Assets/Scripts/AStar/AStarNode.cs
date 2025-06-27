using UnityEngine;

public class AStarNode
{
    public Vector2Int Position;
    public AStarNode Parent;
    public float G; // Cost from start to current
    public float H; // Heuristic cost to target
    public float F => G + H;

    public AStarNode(Vector2Int position)
    {
        Position = position;
    }
}
