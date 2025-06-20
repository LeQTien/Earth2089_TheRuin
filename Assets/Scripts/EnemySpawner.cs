//using System.Collections;
//using UnityEngine;

//public class EnemySpawner : MonoBehaviour
//{
//    [SerializeField] private GameObject[] enemies; // mảng chứa các enemy
//    [SerializeField] private GameObject[] spawnPoints; // mảng chứa các spawn point
//    [SerializeField] private float spawnRate = 0.5f; // thời gian giữa các lần spawn

//    private void Start()
//    {
//        StartCoroutine(SpawnEnemies()); // bắt đầu spawn enemy
//    }

//    private IEnumerator SpawnEnemies()
//    {
//        while (true)
//        {
//            yield return new WaitForSeconds(spawnRate); // chờ giữa các lần spawn
//            GameObject enemy = enemies[Random.Range(0, enemies.Length)]; // chọn ngẫu nhiên enemy trong mảng enemies
//            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)]; // chọn ngẫu nhiên spawn point trong mảng spawnPoints
//            Instantiate(enemy, spawnPoint.transform.position, Quaternion.identity); // tạo enemy tại spawn point
//        }
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private int enemiesPerWave = 3;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform player;
    [SerializeField] float minSpawnDistance = 5f; // Ví dụ, không spawn trong vòng 2 đơn vị quanh player
    [SerializeField] private GameObject spawnIndicatorPrefab;
    [SerializeField] private float indicatorDuration = 1.5f;

    //public bool isMapReady = false;

    private void Start()
    {
        // Chờ cho đến khi map được đánh dấu là đã sẵn sàng
        //yield return new WaitUntil(() => isMapGenerating);
        StartCoroutine(SpawnEnemies());
    }

    //private IEnumerator SpawnEnemies()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(spawnRate);

    //        List<Vector2> validPositions = SpawnAreaChecker.GetValidSpawnPositionsNearPlayer(
    //            player.position, spawnRadius, enemiesPerWave * 2, obstacleLayer, minSpawnDistance);

    //        if (validPositions.Count == 0)
    //        {
    //            Debug.LogWarning("No spawn positions found. Skipping spawn.");
    //            continue;
    //        }

    //        for (int i = 0; i < enemiesPerWave && validPositions.Count > 0; i++)
    //        {
    //            Vector2 spawnPos = validPositions[Random.Range(0, validPositions.Count)];
    //            GameObject enemy = enemies[Random.Range(0, enemies.Length)];
    //            Instantiate(enemy, spawnPos, Quaternion.identity);

    //            // Xóa vị trí đã dùng để không spawn trùng
    //            validPositions.Remove(spawnPos);
    //        }
    //    }
    //}
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);

            List<Vector2> validPositions = SpawnAreaChecker.GetValidSpawnPositionsNearPlayer(
                player.position, spawnRadius, enemiesPerWave * 2, obstacleLayer, minSpawnDistance);

            if (validPositions.Count == 0)
            {
                Debug.LogWarning("No spawn positions found. Skipping spawn.");
                continue;
            }

            for (int i = 0; i < enemiesPerWave && validPositions.Count > 0; i++)
            {
                Vector2 spawnPos = validPositions[Random.Range(0, validPositions.Count)];
                GameObject enemy = enemies[Random.Range(0, enemies.Length)];

                StartCoroutine(SpawnWithIndicator(enemy, spawnPos)); // gọi coroutine spawn

                validPositions.Remove(spawnPos);
            }
        }
    }
    private IEnumerator SpawnWithIndicator(GameObject enemyPrefab, Vector2 spawnPos)
    {
        GameObject indicator = Instantiate(spawnIndicatorPrefab, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(indicatorDuration); // phải trùng với duration trong Blink script

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}