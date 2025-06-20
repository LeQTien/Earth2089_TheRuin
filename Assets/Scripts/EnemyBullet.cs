using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Vector3 movementDirection;
    [SerializeField] private float enemyBulletDamage = 20f;
    [SerializeField] private float moveSpeed = 2f;

    void Start()
    {
        Destroy(gameObject, 5f); // sau 5 giây sẽ hủy đối tượng
    }

    void Update()
    {
        if (movementDirection == Vector3.zero) return; // nếu không có hướng di chuyển thì không làm gì cả
        transform.position += moveSpeed * movementDirection * Time.deltaTime; // di chuyển theo hướng đã được set

    }

    public void SetMovementDirection(Vector3 direction)
    {
        movementDirection = direction;
    }

    public float SetDamage()
    {
        return enemyBulletDamage;
    }

}
