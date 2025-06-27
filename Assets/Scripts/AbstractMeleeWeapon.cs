using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using static UnityEditor.PlayerSettings;
#endif

public class AbstractMeleeWeapon : MonoBehaviour
{
    protected float rotateOffset = 0f;
    [SerializeField] protected GameObject bloodPrefab;

    protected float baseDamage = 100f; // Sát thương cơ bản
    protected float baseAttackDelay = 0.1f; // Thời gian chờ giữa hai đòn đánh
    protected float baseAttackingDuration; // Thời gian vung kiếm
    [Header("Animation Clip")]
    [SerializeField] protected AnimationClip attackClip; // <- Gán clip "Attack" trong Inspector

    protected float baseMeleeWeaponKnockbackForce = 10f;
    protected float baseMeleeWeaponStunDuration = 0.1f;

    protected Animator animator;
    protected bool isAttacking = false;
    private float attackTime = 0f; // Thời điểm được phép ra đòn tiếp theo

    protected GameManager gameManager;
    protected Player player;
    public Transform weaponPivot; // Weapon Pivot (đặt trong Inspector)

    private HashSet<Enemy> damagedEnemies = new HashSet<Enemy>(); // Lưu danh sách kẻ địch đã trúng đòn

    [Header("Rage Mode Stats")]
    protected float damage;
    protected float attackDelay;
    protected float attackingDuration;
    protected float meleeWeaponKnockbackForce;
    protected float meleeWeaponStunDuration;

    protected bool lastRageState;
    [SerializeField] protected Collider2D weaponCollider;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        gameManager = FindAnyObjectByType<GameManager>();
        player = FindAnyObjectByType<Player>();

        attackDelay = baseAttackDelay;
        damage = baseDamage;

        // Gán độ dài animation nếu có clip
        attackingDuration = attackClip != null ? attackClip.length : baseAttackingDuration;

        meleeWeaponKnockbackForce = baseMeleeWeaponKnockbackForce;
        meleeWeaponStunDuration = baseMeleeWeaponStunDuration;
    }

    protected virtual void Update()
    {
        RotateMeleeWeapon();
        Attack();

        // Log để kiểm tra giá trị
        Debug.Log($"Damage: {damage}, Attack Delay: {attackDelay}");
    }

    public virtual void Attack()
    {
        if (Input.GetMouseButton(0) && Time.time > attackTime)
        {
            if (isAttacking)
                return;

            animator.SetTrigger("Attack");
            isAttacking = true;
            weaponCollider.enabled = true; // ← Bật collider khi tấn công
            StartCoroutine(AttackingDuration());

            attackTime = Time.time + attackingDuration + attackDelay;
        }
    }

    protected virtual IEnumerator AttackingDuration()
    {
        yield return new WaitForSeconds(attackingDuration);

        isAttacking = false;
        weaponCollider.enabled = false; // ← Tắt collider khi kết thúc đòn đánh
        damagedEnemies.Clear();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttacking) return;
        else
        {
            if (collision.CompareTag("Enemy"))
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null && !damagedEnemies.Contains(enemy))
                {
                    enemy.TakeDamage(damage);
                    GameObject blood = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
                    Destroy(blood, 1f);
                    enemy.KnockBack(player.transform.position, meleeWeaponKnockbackForce);
                    //enemy.Stun(meleeWeaponStunDuration);
                    gameManager.AddRagePoint(1);

                    Debug.Log($"Hit Enemy! Current Damage: {damage}");
                }
            }
            if (collision.gameObject.CompareTag("Chest"))
            {
                Chest chest = collision.gameObject.GetComponent<Chest>();
                if (chest != null)
                {
                    chest.TakeDamage(damage);
                    Debug.Log("Hit Chest!");
                }
            }
        }
    }



    public bool GetIsAttacking()
    {
        return isAttacking;
    }
    public void RotateMeleeWeapon()
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width ||
            Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
        {
            return;
        }

        // Xác định hướng từ nhân vật đến con trỏ
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePos - weaponPivot.position).normalized;

        // Tính góc quay từ vị trí nhân vật đến con trỏ chuột
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Xoay Weapon Pivot thay vì vũ khí
        weaponPivot.rotation = Quaternion.Euler(0, 0, angle + rotateOffset);

        // Lật toàn bộ Weapon Pivot để đổi hướng đánh
        bool shouldFlip = angle > 90 || angle < -90;
        weaponPivot.localScale = new Vector3(1, shouldFlip ? -1 : 1, 1);
    }

    
}
