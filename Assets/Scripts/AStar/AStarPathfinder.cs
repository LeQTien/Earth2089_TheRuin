using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{
    public static AStarPathfinder Instance { get; private set; }

    private HashSet<Vector2Int> walkablePositions;
    private Dictionary<Vector2Int, Node> grid = new();

    private Dictionary<Vector2Int, HashSet<Vector2Int>> regionMap = new();
    private int regionSize = 16;

    private class Node
    {
        public Vector2Int Position;
        public Node Parent;
        public int G; // Cost từ start đến node này
        public int H; // Heuristic cost đến goal
        public int F => G + H; // Tổng cost

        public Node(Vector2Int pos) => Position = pos;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void InitializeGrid(HashSet<Vector2Int> floorPositions)
    {
        walkablePositions = new HashSet<Vector2Int>(floorPositions);
        grid.Clear();
        //foreach (var pos in walkablePositions)
        //{
        //    grid[pos] = new Node(pos);
        //}
        foreach (var pos in walkablePositions)
        {
            Vector2Int region = GetRegionForPosition(pos);

            if (!regionMap.ContainsKey(region))
                regionMap[region] = new HashSet<Vector2Int>();

            regionMap[region].Add(pos);

            grid[pos] = new Node(pos);
        }
    }
    private Vector2Int GetRegionForPosition(Vector2Int pos)
    {
        return new Vector2Int(
            Mathf.FloorToInt((float)pos.x / regionSize),
            Mathf.FloorToInt((float)pos.y / regionSize)
        );
    }

    public List<Vector2> FindPath(Vector2 startWorld, Vector2 goalWorld, int maxSearchRegion = 1)
    {
        Vector2Int start = Vector2Int.RoundToInt(startWorld);
        Vector2Int goal = Vector2Int.RoundToInt(goalWorld);

        Vector2Int startRegion = GetRegionForPosition(start);
        Vector2Int goalRegion = GetRegionForPosition(goal);

        // Nếu không cho phép đi quá nhiều vùng → trả null
        if (Mathf.Abs(startRegion.x - goalRegion.x) > maxSearchRegion ||
            Mathf.Abs(startRegion.y - goalRegion.y) > maxSearchRegion)
        {
            Debug.Log("Path tìm vượt quá số vùng cho phép.");
            return null;
        }

        if (!walkablePositions.Contains(start) || !walkablePositions.Contains(goal))
            return null;

        var openSet = new PriorityQueue<Node>();
        var closedSet = new HashSet<Vector2Int>();
        var allNodes = new Dictionary<Vector2Int, Node>();

        Node startNode = new Node(start);
        startNode.G = 0;
        startNode.H = Heuristic(start, goal);
        allNodes[start] = startNode;
        
        openSet.Enqueue(startNode, startNode.F);
        int safetyCounter = 0;
        int maxIterations = 100;

        while (openSet.Count > 0)
        {
            if (++safetyCounter > maxIterations)
            {
                Debug.LogWarning("A* pathfinding stopped: exceeded max iterations.");
                break;
            }

            Node current = openSet.Dequeue();

            if (current.Position == goal)
                return ReconstructPath(current);

            closedSet.Add(current.Position);

            foreach (Vector2Int dir in GetDirections())
            {
                Vector2Int neighborPos = current.Position + dir;

                // Không đi được hoặc đã xét rồi
                if (!walkablePositions.Contains(neighborPos) || closedSet.Contains(neighborPos))
                    continue;

                // ✅ Ngăn đi chéo xuyên qua tường
                if (IsDiagonal(dir))
                {
                    Vector2Int check1 = current.Position + new Vector2Int(dir.x, 0);
                    Vector2Int check2 = current.Position + new Vector2Int(0, dir.y);
                    if (!walkablePositions.Contains(check1) || !walkablePositions.Contains(check2))
                        continue;
                }

                // ✅ Có thể kiểm tra Border nếu cần
                if (Physics2D.OverlapCircle((Vector2)neighborPos, 0.1f, LayerMask.GetMask("Border")))
                    continue;

                int stepCost = GetStepCost(dir);
                int tentativeG = current.G + stepCost;

                if (!allNodes.TryGetValue(neighborPos, out Node neighbor))
                {
                    neighbor = new Node(neighborPos);
                    allNodes[neighborPos] = neighbor;
                }

                if (neighbor.Parent == null || tentativeG < neighbor.G)
                {
                    neighbor.Parent = current;
                    neighbor.G = tentativeG;
                    neighbor.H = Heuristic(neighborPos, goal);
                    openSet.Enqueue(neighbor, neighbor.F);
                }
            }
        }

        return null;
    }

    private List<Vector2> ReconstructPath(Node node)
    {
        List<Vector2> path = new();
        while (node != null)
        {
            path.Add((Vector2)node.Position);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return 10 * (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y)); // Manhattan * 10
    }

    private List<Vector2Int> GetDirections()
    {
        return new List<Vector2Int>
        {
            // 4 hướng chính
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,

            // 4 chéo
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1),

            // 8 hướng lệch (Knight-like)
            new Vector2Int(2, 1),
            new Vector2Int(1, 2),
            new Vector2Int(-2, 1),
            new Vector2Int(-1, 2),
            new Vector2Int(2, -1),
            new Vector2Int(1, -2),
            new Vector2Int(-2, -1),
            new Vector2Int(-1, -2),
        };
    }

    private int GetStepCost(Vector2Int dir)
    {
        // Thẳng = 10, chéo = 14, lệch xa = 30
        if (Mathf.Abs(dir.x) == 1 && Mathf.Abs(dir.y) == 1) return 14;
        if (Mathf.Abs(dir.x) == 2 || Mathf.Abs(dir.y) == 2) return 30;
        return 10;
    }

    private bool IsDiagonal(Vector2Int dir)
    {
        return Mathf.Abs(dir.x) == 1 && Mathf.Abs(dir.y) == 1;
    }

    // ⚠️ Cấu trúc hàng đợi ưu tiên đơn giản (dễ hiểu, không tối ưu cho map lớn)
    private class PriorityQueue<T>
    {
        private List<(T item, int priority)> elements = new();
        public int Count => elements.Count;

        public void Enqueue(T item, int priority)
        {
            elements.Add((item, priority));
            elements.Sort((a, b) => a.priority.CompareTo(b.priority)); // Có thể thay bằng heap
        }

        public T Dequeue()
        {
            var item = elements[0].item;
            elements.RemoveAt(0);
            return item;
        }
    }
}
