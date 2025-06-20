using UnityEngine;

public class AbstractPlayerBulletClass : MonoBehaviour
{
    protected float baseBulletMoveSpeed = 30f; // tốc độ di chuyển của viên đạn
    protected float bulletlifeTime = 2f; // thời gian tồn tại của viên đạn
    protected float baseDamage = 80f; // sát thương của viên đạn    
    [SerializeField] protected GameObject bloodPrefabs; // hiệu ứng khi enemy bị bắn
    
    protected Player player;

    [SerializeField] protected float baseMaxDistance = 10f; // Khoảng cách tối đa viên đạn bay được
    protected Vector3 spawnPosition;

    protected Enemy enemy;
    protected GameManager gameManager;

    protected float damage;
    protected float maxDistance;
    protected float bulletMoveSpeed;

    private bool lastRageState;
    protected virtual void Start()
    {
        player = FindAnyObjectByType<Player>(); // Tìm Player khi bắt đầu
        gameManager = FindAnyObjectByType<GameManager>(); // Tìm GameManager khi bắt đầu
        enemy = FindAnyObjectByType<Enemy>(); // lấy component Enemy từ enemy

        spawnPosition = transform.position;

        // Áp dụng chỉ số sát thương và khoảng cách
        ApplyRageModeStats();

        Destroy(gameObject, bulletlifeTime); // hủy viên đạn sau thời gian tồn tại
    }

    protected virtual void Update()
    {
        MoveBullet();
        CheckDistance();

        if (gameManager.IsRageActive() != lastRageState) // Nếu trạng thái Rage thay đổi
        {
            ApplyRageModeStats();
            lastRageState = gameManager.IsRageActive();
        }

    }
    protected virtual void CheckDistance()
    {
        // Nếu khoảng cách giữa vị trí hiện tại và nơi bắn ra lớn hơn maxDistance thì hủy đạn
        if (Vector3.Distance(spawnPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void MoveBullet()
    {
        // translate để di chuyển viên đạn theo thời gian, 
        // Vector2.right để di chuyển về phía bên phải theo trục x, bulletMoveSpeed là tốc độ di chuyển
        // Time.deltaTime để điều chỉnh tốc độ di chuyển không phụ thuộc vào tốc độ khung hình
        transform.Translate(bulletMoveSpeed * Time.deltaTime * Vector2.right);
    }

    private void ApplyRageModeStats()
    {
        if (gameManager.IsRageActive())
        {
            damage = baseDamage * player.GetRageDamageMultiplier();
            maxDistance = baseMaxDistance * player.GetRageMaxDistanceMultiplier();
            bulletMoveSpeed = baseBulletMoveSpeed * player.GetRageBulletSpeedMultiplier();
        }
        else
        {
            damage = baseDamage;
            maxDistance = baseMaxDistance;
            bulletMoveSpeed = baseBulletMoveSpeed;
        }
    }

    public float GetDamage()
    {
        return damage;
    }
    public float GetMaxDistance()
    {
        return maxDistance;
    }
}
