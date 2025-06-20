using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    //private static DontDestroyOnLoad instance;

    void Awake()
    {
        
            DontDestroyOnLoad(gameObject);
        
    }
}
