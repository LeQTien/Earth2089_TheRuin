using UnityEngine;
using System.Collections;

public class PlayerSpawnManager : MonoBehaviour
{
    IEnumerator Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        MultiZoneGenerator2 multiZoneGenerator2 = FindObjectOfType<MultiZoneGenerator2>();
        GameObject loadingScreen = GameObject.FindWithTag("LoadingScreen");

        yield return new WaitUntil(() =>
            multiZoneGenerator2 != null &&
            multiZoneGenerator2.IsMapGenerated);

        if (player != null)
        {
            Debug.Log("Đặt vị trí của Player thành: " + multiZoneGenerator2.spawnWorldPos);
            player.transform.position = multiZoneGenerator2.spawnWorldPos;
            loadingScreen.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Player!");
        }
    }
}
