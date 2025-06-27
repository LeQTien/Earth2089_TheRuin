using System.Collections.Generic;
using UnityEngine;

public class NodeGraphGenerator : MonoBehaviour
{
    public LayerMask borderMask; // set "Border" trong Editor

    public Dictionary<Vector2Int, Node> gridNodes = new();

    public void GenerateGraph(HashSet<Vector2Int> floorPositions)
    {
        gridNodes.Clear();

        foreach (var pos in floorPositions)
        {
            if (!Physics2D.OverlapCircle((Vector2)pos + Vector2.one * 0.5f, 0.1f, borderMask))
            {
                gridNodes[pos] = new Node(pos);
            }
        }

        foreach (var node in gridNodes.Values)
        {
            foreach (var dir in GetDirections())
            {
                Vector2Int neighborPos = node.gridPos + dir;
                if (gridNodes.ContainsKey(neighborPos))
                {
                    node.connections.Add(gridNodes[neighborPos]);
                }
            }
        }

        AStarManager.Instance.SetGraph(gridNodes);
    }

    private List<Vector2Int> GetDirections()
    {
        return new List<Vector2Int>
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            new Vector2Int(1,1), new Vector2Int(-1,1), new Vector2Int(1,-1), new Vector2Int(-1,-1)
        };
    }
}
