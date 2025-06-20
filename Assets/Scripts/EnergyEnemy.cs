using UnityEngine;

public class EnergyEnemy : Enemy
{
    [SerializeField] private GameObject energyBall;
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
        if (energyBall != null)
        {
            GameObject energy = Instantiate(energyBall, transform.position, Quaternion.identity);
            Destroy(energy, 10f);
        }
        base.Die();
    }
}
