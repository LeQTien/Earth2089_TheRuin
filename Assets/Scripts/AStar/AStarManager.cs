using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager Instance { get; private set; }

    private Dictionary<Vector2Int, Node> graph;

    private void Awake()
    {
        Instance = this;
    }

    public void SetGraph(Dictionary<Vector2Int, Node> graphData)
    {
        graph = graphData;
    }

    public Node FindNearestNode(Vector2 worldPos)
    {
        Vector2Int rounded = Vector2Int.RoundToInt(worldPos);
        if (graph.ContainsKey(rounded))
            return graph[rounded];

        // fallback: tìm gần nhất
        float minDist = float.MaxValue;
        Node nearest = null;

        foreach (var node in graph.Values)
        {
            float dist = Vector2.Distance(worldPos, node.gridPos);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = node;
            }
        }

        return nearest;
    }

    public List<Node> GeneratePath(Node start, Node end)
    {
        List<Node> openSet = new() { start };

        foreach (var node in graph.Values)
            node.gScore = float.MaxValue;

        start.gScore = 0;
        start.hScore = Vector2.Distance(start.gridPos, end.gridPos);

        while (openSet.Count > 0)
        {
            Node current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
                if (openSet[i].FScore() < current.FScore())
                    current = openSet[i];

            if (current == end)
            {
                var path = new List<Node>();
                while (current != null)
                {
                    path.Add(current);
                    current = current.cameFrom;
                }

                path.Reverse();
                return path;
            }

            openSet.Remove(current);

            foreach (var neighbor in current.connections)
            {
                float tentativeG = current.gScore + Vector2.Distance(current.gridPos, neighbor.gridPos);
                if (tentativeG < neighbor.gScore)
                {
                    neighbor.cameFrom = current;
                    neighbor.gScore = tentativeG;
                    neighbor.hScore = Vector2.Distance(neighbor.gridPos, end.gridPos);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }
}
