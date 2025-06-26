//using UnityEngine;

//public class PlayerMovementEventTrigger : MonoBehaviour
//{
//    private Vector2 lastPosition;
//    public float moveThreshold = 0.5f; // Khoảng cách phải vượt qua để phát sự kiện

//    void Start()
//    {
//        lastPosition = transform.position;
//    }

//    void Update()
//    {
//        if (Vector2.Distance(transform.position, lastPosition) > moveThreshold)
//        {
//            lastPosition = transform.position;
//            EventManager.Instance.PlayerMoved(transform.position);
//        }
//    }
//}
