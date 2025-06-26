//using System;
//using UnityEngine;

//public class EventManager : MonoBehaviour
//{
//    public static EventManager Instance { get; private set; }

//    public event Action<Vector2> OnPlayerMoved;

//    void Awake()
//    {
//        if (Instance == null) Instance = this;
//        else Destroy(gameObject);
//    }

//    public void PlayerMoved(Vector2 newPosition)
//    {
//        OnPlayerMoved?.Invoke(newPosition);
//    }
//}
