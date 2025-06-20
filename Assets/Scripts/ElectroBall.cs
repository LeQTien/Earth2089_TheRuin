using TMPro;
using UnityEngine;

public class ElectroBall : AbstractPlayerBulletClass
{
    [SerializeField] private GameObject explosionPrefab; // prefab của vụ nổ
    [SerializeField] private float baseElectroBulletMoveSpeed = 15f; // tốc độ di chuyển của viên đạn
    [SerializeField] private float electroBulletMoveSpeed; // sát thương của viên đạn
    
    protected override void Start()
    {
        base.Start();
        UpdateRageStats(); // cập nhật tốc độ di chuyển của viên đạn
    }
    protected override void Update()
    {
        base.Update();
        UpdateRageStats(); // cập nhật tốc độ di chuyển của viên đạn
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // nếu va chạm với enemy
        {
            Enemy enemy = collision.GetComponent<Enemy>(); // lấy component Enemy từ enemy
            //Player thePlayer = collision.GetComponent<Player>();
            //enemy.GetComponent<Enemy>().Stun();
            if (enemy != null) // nếu enemy không null
            {
                enemy.TakeDamage(damage); // gọi hàm TakeDamage() của enemy
                GameObject blood = Instantiate(bloodPrefabs, transform.position, Quaternion.identity); // tạo hiệu ứng blood khi enemy bị bắn
                Destroy(blood, 1f); // hủy hiệu ứng blood sau 1 giây
                gameManager.AddRagePoint(1);
                // Gọi hàm KnockBack từ enemy, truyền vị trí viên đạn là nguồn lực
                // nếu vị trí nguồn chỉ có transform.position thì nó là vị trí của viên đạn
                //enemy.KnockBack(player.transform.position);
            }
            //Destroy(gameObject); // hủy viên đạn
        }
    }

    protected override void CheckDistance()
    {
        // Nếu khoảng cách giữa vị trí hiện tại và nơi bắn ra lớn hơn maxDistance thì hủy đạn
        if (Vector3.Distance(spawnPosition, transform.position) >= maxDistance)
        {
            CreateExplosion();
            //Destroy(gameObject);
        }
    }

    public void CreateExplosion()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject); // Hủy viên đạn sau khi nổ
        }
    }

    protected void UpdateRageStats()
    {
        bool isRageActive = gameManager.IsRageActive();
        float rageBulletMoveSpeedMultiplier = isRageActive ? player.GetRageFireRateDivider() : 1f;

        electroBulletMoveSpeed = baseElectroBulletMoveSpeed * rageBulletMoveSpeedMultiplier;
    }
}
