using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ClearDontDestroyOnLoad : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Cleanup());
    }

    IEnumerator Cleanup()
    {
        yield return null; // đợi 1 frame để các đối tượng được chuyển qua đúng scene

        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Xoá mọi object đến từ scene DontDestroyOnLoad
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                Destroy(obj);
            }
        }

        yield return null;
    }
}
