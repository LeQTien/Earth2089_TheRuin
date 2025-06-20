using UnityEngine;

public class HealEnemy : Enemy
{
    [SerializeField] private GameObject healPickup;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player != null)
            {
                player.TakeDamage(enterDamage);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player != null)
            {
                player.TakeDamage(stayDamage);
            }
        }
    }

    protected override void Die()
    {
        if (healPickup != null)
        {
            GameObject healP = Instantiate(healPickup, transform.position, Quaternion.identity);
            Destroy(healP, 10f);
        }
        base.Die();
    }
}
