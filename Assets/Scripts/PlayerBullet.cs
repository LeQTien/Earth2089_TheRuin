using UnityEngine;
using System.Collections.Generic; // Import thư viện HashSet

public class PlayerBullet : AbstractPlayerBulletClass
{
    private HashSet<Enemy> damagedEnemies = new HashSet<Enemy>(); // Lưu danh sách kẻ địch đã trúng viên đạn này
    [SerializeField] protected float shotGunKnockbackForce = 10f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // nếu va chạm với enemy
        {
            Enemy enemy = collision.GetComponent<Enemy>(); // lấy component Enemy từ enemy

            if (enemy != null && !damagedEnemies.Contains(enemy)) // Kiểm tra xem đã gây sát thương chưa
            {
                damagedEnemies.Add(enemy); // Đánh dấu enemy đã bị trúng viên đạn này
                enemy.TakeDamage(damage); // Gây sát thương
                GameObject blood = Instantiate(bloodPrefabs, transform.position, Quaternion.identity);
                Destroy(blood, 1f);

                enemy.KnockBack(player.transform.position, shotGunKnockbackForce);
                gameManager.AddRagePoint(1);

                //Debug.Log("Damage: " + damage + " Max Distance: " + maxDistance);
            }

            //Destroy(gameObject); // Hủy viên đạn sau khi va chạm
        }
        
    }
}
