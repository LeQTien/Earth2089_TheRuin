using System.Collections.Generic;
using UnityEngine;

public static class SpawnAreaChecker
{
    public static List<Vector2> GetValidSpawnPositionsNearPlayer(Vector2 playerPos, float radius, int sampleCount, LayerMask obstacleLayer, float minDistanceFromPlayer = 2f)
    {
        List<Vector2> validPositions = new List<Vector2>();
        int attempts = 0;

        while (validPositions.Count < sampleCount && attempts < sampleCount * 5)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float dist = Random.Range(minDistanceFromPlayer, radius); // ✅ Đảm bảo không dưới minDistance
            Vector2 point = playerPos + dir * dist;

            // Raycast từ player đến điểm, nếu gặp tường thì bỏ qua
            RaycastHit2D hit = Physics2D.Raycast(playerPos, dir, dist, obstacleLayer);
            if (hit.collider != null) continue;

            // Kiểm tra có đè tường tại vị trí đó không
            Collider2D overlap = Physics2D.OverlapCircle(point, 0.2f, obstacleLayer);
            if (overlap != null) continue;

            validPositions.Add(point);
            attempts++;
        }

        return validPositions;
    }
}