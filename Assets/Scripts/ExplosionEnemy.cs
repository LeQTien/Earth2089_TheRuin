using UnityEngine;

public class ExplosionEnemy : Enemy
{
    [SerializeField] private GameObject explosionPrefab;
    private bool hasExploded = false;

    private void CreateExplosion()
    {
        if (!hasExploded && explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            hasExploded = true;
        }
    }

    protected override void Die()
    {
        CreateExplosion(); // Chỉ gọi một lần khi chết
        base.Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasExploded && collision.CompareTag("Player"))
        {
            CreateExplosion();
            base.Die();
        }
    }
}
