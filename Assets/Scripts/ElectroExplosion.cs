using UnityEngine;

public class ElectroExplosion : MonoBehaviour
{
    private float baseElectroExplotionDamage = 150f;
    private float baseElectroExplosionKnockbackForce = 25f;
    private Collider2D explosionCollider;

    private Player player; // Vị trí của player
    private GameManager gameManager;
    [SerializeField] private float damage; // Sát thương của vụ nổ
    [SerializeField] private float electroExplosionKnockbackForce; // Lực knockback của vụ nổ
    private void Start()
    {
        explosionCollider = GetComponent<Collider2D>();
        player = FindAnyObjectByType<Player>(); // Tìm player trong scene để theo dõi vị trí của player
        gameManager = FindAnyObjectByType<GameManager>();
        // Tắt collider sau 0.1 giây để tránh gây sát thương vô hình
        Invoke("DisableCollider", 0.1f);
        UpdateRageStats(); // Cập nhật chỉ số sát thương và knockback force khi Rage Mode được kích hoạt
        // Hủy vụ nổ sau 1 giây
        Destroy(gameObject, 1f);
    }
    private void Update()
    {
        // Cập nhật chỉ số sát thương và knockback force mỗi frame
        UpdateRageStats();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Player"))
        //{
        //    Player player = collision.GetComponent<Player>();
        //    if (player != null)
        //    {
        //        player.TakeDamage(damage);
        //    }
        //}
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.KnockBack(transform.position, electroExplosionKnockbackForce);
                //enemy.Stun(electroStunDuration);
                enemy.TakeDamage(damage);
                gameManager.AddRagePoint(1);
            }
        }
    }

    private void DisableCollider()
    {
        if (explosionCollider != null)
        {
            explosionCollider.enabled = false; // Tắt collider để vụ nổ không gây sát thương nữa
        }
    }

    private void UpdateRageStats()
    {
        if (gameManager.IsRageActive()) // Nếu bật Rage Mode
        {
            damage = baseElectroExplotionDamage * player.GetRageDamageMultiplier();
            electroExplosionKnockbackForce = baseElectroExplosionKnockbackForce * player.GetRageKnockbackForceMultiplier();
        }
        else // Nếu tắt Rage Mode
        {
            damage = baseElectroExplotionDamage;
            electroExplosionKnockbackForce = baseElectroExplosionKnockbackForce;
        }
    }
}
