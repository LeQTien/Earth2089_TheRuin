using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float damage = 60f;
    private Collider2D explosionCollider;

    private void Start()
    {
        explosionCollider = GetComponent<Collider2D>();

        // Tắt collider sau 0.1 giây để tránh gây sát thương nhiều lần
        Invoke("DisableCollider", 0.1f);

        DestroyExplosion();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
        else if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    public void DestroyExplosion()
    {
        // Hủy vụ nổ sau 0.5 giây
        Destroy(gameObject, 0.5f);
    }

    private void DisableCollider()
    {
        if (explosionCollider != null)
        {
            explosionCollider.enabled = false;
        }
    }

    private void OnDestroy()
    {
        // Nếu collider vẫn còn thì tắt nó
        if (explosionCollider != null)
        {
            explosionCollider.enabled = false;
        }
    }
}
