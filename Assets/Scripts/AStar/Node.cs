using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2Int gridPos;
    public Node cameFrom;
    public List<Node> connections = new List<Node>();

    public float gScore;
    public float hScore;

    public float FScore() => gScore + hScore;

    public Node(Vector2Int pos)
    {
        gridPos = pos;
    }
}
