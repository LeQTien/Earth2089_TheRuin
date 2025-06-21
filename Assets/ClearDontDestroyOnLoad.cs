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
        yield return null; // đợi 1 frame để các object được chuyển vào scene

        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                // Giữ lại HighScoreManager
                if (obj.GetComponent<HighScoreManager>() != null)
                    continue;

                Destroy(obj);
            }
        }

        yield return null;
    }

}
