using UnityEngine;
using System.Collections;

public class LoadingScene1Manager : MonoBehaviour
{
    IEnumerator Start()
    {
        MultiZoneGenerator MultiZoneGenerator = FindObjectOfType<MultiZoneGenerator>();
        GameObject loadingScreen = GameObject.FindWithTag("LoadingScreen");

        loadingScreen.SetActive(true);

        yield return new WaitUntil(() =>
            MultiZoneGenerator != null &&
            MultiZoneGenerator.IsMapGenerated);

        loadingScreen.SetActive(false);
        
    }
}
