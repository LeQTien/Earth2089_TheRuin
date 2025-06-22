using UnityEngine;

public class Chest : MonoBehaviour
{
    public float maxHP = 120;
    private float currentHP;

    private Animator animator;
    private bool isOpened = false;

    [Header("Prefabs to drop")]
    public GameObject goldPrefab;
    public GameObject energyPickupPrefab;
    public GameObject healPickupPrefab;

    //public AbstractMeleeWeapon meleeWeapon; // Tham chiếu đến vũ khí cận chiến

    private void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
        //meleeWeapon = FindObjectOfType<AbstractMeleeWeapon>(); // Tìm kiếm vũ khí cận chiến trong scene

    }

    //public void TakeDamage(float damage)
    //{
    //    if (isOpened) return;
    //    if (meleeWeapon.)
    //    {
    //        // Kiểm tra xem vũ khí có đang tấn công không
    //        // damage = meleeWeapon.damage; // Lấy sát thương từ vũ khí cận chiến
    //        currentHP -= damage;
    //        animator.SetTrigger("Hit");

    //        if (currentHP <= 0)
    //        {
    //            OpenChest();
    //            DestroyChest();
    //        }
    //    }
    //}
    public void TakeDamage(float damage)
    {
        if (isOpened) return;

        currentHP -= damage;
        animator.SetTrigger("Hit");

        if (currentHP <= 0)
        {
            OpenChest();
            DestroyChest();
        }

    }

    void OpenChest()
    {
        isOpened = true;
        animator.SetTrigger("Open");

        // Sinh vật phẩm rơi ra
        Instantiate(goldPrefab, transform.position + Vector3.right * 0.5f, Quaternion.identity);
        Instantiate(energyPickupPrefab, transform.position + Vector3.left * 0.5f, Quaternion.identity);
        Instantiate(healPickupPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);

        // Tùy chọn: hủy sau vài giây hoặc không
        //Destroy(gameObject, 1.5f);
    }

    void DestroyChest()
    {
        Destroy(gameObject, 1f);
    }
    //void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (!playerIsAttacking) return;

    //    // Chest
    //    else if (playerIsAttacking && collision.CompareTag("Chest"))
    //    {
    //        Chest chest = collision.GetComponent<Chest>();
    //        if (chest != null)
    //        {
    //            chest.TakeDamage(30);
    //            Debug.Log("Hit Chest!");
    //        }
    //    }
    //}
}
