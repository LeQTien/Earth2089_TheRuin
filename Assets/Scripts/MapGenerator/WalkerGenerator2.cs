using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MultiZoneGenerator2 : MonoBehaviour
{
    [Header("Ground Tilemap Setup")]
    public Tilemap groundTilemap;
    public TileBase[] floorTiles;
    public Tilemap decorationTilemap; // Kéo vào từ Inspector
    public TileBase[] decorationTiles; // Mảng chứa các tile trang trí (sỏi, đá,...)

    [Header("Wall Setup")]
    public Tilemap wallTilemap;
    public TileBase[] wallTiles;

    [Header("Chest Setup")]
    public GameObject[] chestPrefabs; // Đổi từ GameObject thành mảng
    public float chestSpawnChance = 0.05f;

    [Header("Trap Setup")]
    public GameObject[] fireTrapPrefabs; // Đổi từ GameObject thành mảng
    public float trapSpawnChance = 0.05f;

    [Header("Map Settings")]
    public int mapWidth = 300;
    public int mapHeight = 300;
    public int numberOfZones = 6;
    public int stepsPerZone = 3000;
    public int maxRadius = 600;
    public int corridorWidth = 4;
    public float zoneDistance = 30f; // khoảng cách trung bình
    public float addWalkerChance = 0.5f; // Tỷ lệ thêm walker mới
    public float removeWalkerChance = 0.1f; // Tỷ lệ loại bỏ walker
    public float placeDecorationChance = 0.1f; // Tỷ lệ đặt trang trí
    public int maxWalkerNumber = 50; // Giới hạn số lượng walker tối đa
    public int minWalkerNumber = 1;

    private List<Vector2Int> spawnPoints = new List<Vector2Int>();
    private HashSet<Vector2Int> allFloorPositions = new HashSet<Vector2Int>();

    [Header("Player")]
    public GameObject player;
    public Vector3 spawnWorldPos;
    public Vector3 playerSpawnPoint;

    public bool IsMapGenerated = false;

    public static MultiZoneGenerator2 Instance { get; private set; }
    public HashSet<Vector2Int> FloorPositions => allFloorPositions;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        GenerateSpawnPoints();
        foreach (var point in spawnPoints)
        {
            GenerateZone(point, stepsPerZone, maxRadius);
        }
        ConnectZones(spawnPoints);
        PlaceWallsAroundFloor();
        PlaceChestsNearWalls();
        PlaceFireTraps();
        if (player != null)
        {
            spawnWorldPos = groundTilemap.CellToWorld((Vector3Int)spawnPoints[0]) + new Vector3(0.5f, 0.5f, 0); // điểm spawn player
            player.transform.position = spawnWorldPos; // đặt vị trí player vào điểm spawn
            playerSpawnPoint = spawnWorldPos; // lưu vị trí spawn player
        }
        IsMapGenerated = true; // ✅ Đánh dấu đã tạo xong bản đồ
        Debug.Log("✅ Bản đồ đã được tạo xong!");
    }

    // Tạo các điểm trung tâm cho vùng
    void GenerateSpawnPoints()
    {
        spawnPoints.Clear();
        int maxAttempts = 10000;
        int attempts = 0;

        while (spawnPoints.Count < numberOfZones && attempts < maxAttempts)
        {
            attempts++;

            Vector2Int point = new Vector2Int(
                UnityEngine.Random.Range(-mapWidth / 2, mapWidth / 2),
                UnityEngine.Random.Range(-mapHeight / 2, mapHeight / 2)
            );

            bool isFarEnough = true;
            foreach (var existing in spawnPoints)
            {
                float distance = Vector2Int.Distance(point, existing);
                if (distance < zoneDistance) // chỉ cần kiểm tra khoảng cách tối thiểu
                {
                    isFarEnough = false;
                    break;
                }
            }

            if (isFarEnough)
            {
                spawnPoints.Add(point);
            }
        }

        if (spawnPoints.Count < numberOfZones)
        {
            Debug.LogWarning("Không tìm đủ vùng spawn hợp lệ. Hãy giảm số zone hoặc giảm khoảng cách zoneDistance.");
        }
    }


    // Sinh vùng bằng Walker
    void GenerateZone(Vector2Int center, int steps, int radiusLimit)
    {
        List<Vector2Int> walkers = new List<Vector2Int> { center };
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        for (int i = 0; i < steps; i++)
        {
            for (int w = 0; w < walkers.Count; w++)
            {
                Vector2Int walker = walkers[w];

                if ((walker - center).sqrMagnitude > radiusLimit * radiusLimit)
                {
                    walker = center;
                }

                floorPositions.Add(walker);
                walker += GetRandomDirection();
                walkers[w] = walker;

                // 20% cơ hội nhân bản walker để mở rộng ngẫu nhiên
                if (Random.value < addWalkerChance && walkers.Count < maxWalkerNumber)
                {
                    walkers.Add(walker);
                }

                // 10% cơ hội loại bỏ walker
                if (Random.value < removeWalkerChance && walkers.Count > minWalkerNumber)
                {
                    walkers.RemoveAt(w);
                    w--;
                }
            }
        }

        foreach (var pos in floorPositions)
        {
            // Chọn ngẫu nhiên tile sàn từ mảng floorTiles
            TileBase selectedFloorTile = floorTiles[UnityEngine.Random.Range(0, floorTiles.Length)];
            groundTilemap.SetTile((Vector3Int)pos, selectedFloorTile);
            allFloorPositions.UnionWith(floorPositions);
            // Xác suất random trang trí (ví dụ 10%)
            if (UnityEngine.Random.value < placeDecorationChance)
            {
                TileBase deco = decorationTiles[UnityEngine.Random.Range(0, decorationTiles.Length)];
                decorationTilemap.SetTile((Vector3Int)pos, deco);
            }
        }
        // Đặt tường xung quanh sàn
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            new Vector2Int(1,1), new Vector2Int(-1,1), new Vector2Int(1,-1), new Vector2Int(-1,-1)
        };
    }
    void PlaceWallsAroundFloor()
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        Vector2Int[] directions = {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
        new Vector2Int(1,1), new Vector2Int(-1,1), new Vector2Int(1,-1), new Vector2Int(-1,-1)
    };

        foreach (var pos in allFloorPositions)
        {
            foreach (var dir in directions)
            {
                Vector2Int neighborPos = pos + dir;
                if (!allFloorPositions.Contains(neighborPos))
                {
                    wallPositions.Add(neighborPos);
                }
            }
        }

        foreach (var wallPos in wallPositions)
        {
            Vector3Int tilemapPos = (Vector3Int)wallPos;
            wallTilemap.SetTile(tilemapPos, wallTiles[UnityEngine.Random.Range(0, wallTiles.Length)]);

            // Tạo collider (tilemap collider cần gắn sẵn vào wallTilemap)
        }
    }
    void PlaceChestsNearWalls()
    {
        Vector2Int[] directions = {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
        new Vector2Int(2,2), new Vector2Int(-2,2), new Vector2Int(2,-2), new Vector2Int(-2,-2)
    };

        foreach (var pos in allFloorPositions)
        {
            bool nearWall = false;

            foreach (var dir in directions)
            {
                Vector2Int neighbor = pos + dir;
                if (!allFloorPositions.Contains(neighbor)) // không phải sàn => có thể là tường
                {
                    nearWall = true;
                    break;
                }
            }

            //if (nearWall && Random.value < chestSpawnChance)
            //{
            //    Vector3 worldPos = groundTilemap.CellToWorld((Vector3Int)pos) + new Vector3(0.5f, 0.5f, 0); // căn giữa
            //    Instantiate(chestPrefab, worldPos, Quaternion.identity);
            //}
            if (nearWall && Random.value < chestSpawnChance && chestPrefabs.Length > 0)
            {
                Vector3 worldPos = groundTilemap.CellToWorld((Vector3Int)pos) + new Vector3(0.5f, 0.5f, 0);
                int index = Random.Range(0, chestPrefabs.Length);
                Instantiate(chestPrefabs[index], worldPos, Quaternion.identity);
            }
        }
    }
    void PlaceFireTraps()
    {
        foreach (var pos in allFloorPositions)
        {
            //if (Random.value < trapSpawnChance && (Vector2Int)spawnPoints[0] != pos) // tránh vị trí player
            //{
            //    Vector3 worldPos = groundTilemap.CellToWorld((Vector3Int)pos) + new Vector3(0.5f, 0.5f, 0);
            //    Instantiate(fireTrapPrefab, worldPos, Quaternion.identity);
            //}
            if (Random.value < trapSpawnChance && (Vector2Int)spawnPoints[0] != pos && fireTrapPrefabs.Length > 0)
            {
                Vector3 worldPos = groundTilemap.CellToWorld((Vector3Int)pos) + new Vector3(0.5f, 0.5f, 0);
                int index = Random.Range(0, fireTrapPrefabs.Length);
                Instantiate(fireTrapPrefabs[index], worldPos, Quaternion.identity);
            }
        }
    }

    // Nối các vùng lại bằng hành lang
    void ConnectZones(List<Vector2Int> points)
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            CreateCorridor(points[i], points[i + 1]);
        }
    }

    // Tạo đường nối hình chữ L
    void CreateCorridor(Vector2Int from, Vector2Int to)
    {
        Vector2Int current = from;

        // Đi theo trục X trước
        while (current.x != to.x)
        {
            current.x += System.Math.Sign(to.x - current.x);
            CarveCorridor(current);
        }

        // Sau đó đi theo trục Y
        while (current.y != to.y)
        {
            current.y += System.Math.Sign(to.y - current.y);
            CarveCorridor(current);
        }
    }

    void CarveCorridor(Vector2Int pos)
    {
        // Vẽ một ô vuông hành lang rộng corridorWidth x corridorWidth
        for (int dx = -corridorWidth / 2; dx <= corridorWidth / 2; dx++)
        {
            for (int dy = -corridorWidth / 2; dy <= corridorWidth / 2; dy++)
            {
                Vector2Int tilePos = new Vector2Int(pos.x + dx, pos.y + dy);
                Vector3Int tilemapPos = new Vector3Int(tilePos.x, tilePos.y, 0);
                groundTilemap.SetTile(tilemapPos, floorTiles[UnityEngine.Random.Range(0, floorTiles.Length)]); // Chọn ngẫu nhiên tile sàn cho hành lang
                allFloorPositions.Add(tilePos);
            }
        }

    }

    // Hướng đi ngẫu nhiên
    Vector2Int GetRandomDirection()
    {
        int rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0: return Vector2Int.up;
            case 1: return Vector2Int.down;
            case 2: return Vector2Int.left;
            case 3: return Vector2Int.right;
        }
        return Vector2Int.zero;
    }
}
