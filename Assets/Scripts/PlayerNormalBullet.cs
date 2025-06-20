using UnityEngine;

public class PlayerNormalBullet : AbstractPlayerBulletClass
{
    protected override void CheckDistance()
    {
        //base.CheckDistance();
        if (Vector3.Distance(spawnPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // nếu va chạm với enemy
        {
            enemy = collision.GetComponent<Enemy>(); // lấy component Enemy từ enemy
            //GameManager gameManager = FindObjectOfType<GameManager>();
            if (enemy != null) // nếu enemy không null
            {
                enemy.TakeDamage(damage); // gọi hàm TakeDamage() của enemy
                GameObject blood = Instantiate(bloodPrefabs, transform.position, Quaternion.identity); // tạo hiệu ứng blood khi enemy bị bắn
                Destroy(blood, 1f); // hủy hiệu ứng blood sau 1 giây
                gameManager.AddRagePoint(1);
                Debug.Log("Damage: " + damage + " Max Distance: " + maxDistance);

            }
            Destroy(gameObject); // hủy viên đạn
        }
        
    }

}
