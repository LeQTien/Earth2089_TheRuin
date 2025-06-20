using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            if (!player.IsInvincible)
            {
                Player player = GetComponent<Player>();
                EnemyBullet enemyBullet = collision.GetComponent<EnemyBullet>();
                float enemyBulletDamage = enemyBullet.SetDamage();
                player.TakeDamage(enemyBulletDamage);
            }
        }
        else if (collision.CompareTag("Usb"))
        {
            //Player player = GetComponent<Player>();
            //player.TakeHeal(50f);
            Debug.Log("Congratulation! You have Won the game.");
            Destroy(collision.gameObject); // Destroy the USB object
            audioManager.PlayEnergySound();
            gameManager.WinGame();
        }
        else if (collision.CompareTag("EnergyBall"))
        {
            gameManager.AddPlayerEnergyPoint();
            gameManager.AddScore(15);
            Destroy(collision.gameObject);
            audioManager.PlayEnergySound();
        }
        else if (collision.CompareTag("HealPickup"))
        {
            gameManager.HealPlayerByHealPickup();
            gameManager.AddScore(10);
            Destroy(collision.gameObject);
            audioManager.PlayEnergySound();
        }
        else if (collision.CompareTag("Explosion"))
        {
            audioManager.PlayExplosionSound();
        }
        else if (collision.CompareTag("Chest"))
        {
            Chest chest = collision.GetComponent<Chest>();
            chest.TakeDamage(10);
        }
    }
    
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (attackCollider.enabled && collision.CompareTag("Chest"))
    //    {
    //        if (collision.TryGetComponent<Chest>(out var chest))
    //        {
    //            chest.TakeDamage(damage);
    //        }
    //    }
    //}
}

