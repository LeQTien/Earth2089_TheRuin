using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEngine;
using System.Linq;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] public float enemyMoveSpeed = 5f; // tốc độ di chuyển

    protected Player player; // vị trí của player
    protected GameManager gameManager;
    private Rigidbody2D rb;

    // các biến xử lý thanh máu
    [SerializeField] protected float maxHP = 200f;
    [SerializeField] protected float currentHP;
    [SerializeField] protected Image HPBar;

    [SerializeField] protected float enterDamage = 20f; // sát thương khi va chạm với player
    [SerializeField] protected float stayDamage = 5f; // sát thương khi enemy tiếp xúc liên tục với player

    private bool isKnockedBack = false;

    //private float baseKnockbackForce = 20f;

    private float knockbackDuration = 0.3f; // Khoảng thời gian enemy sẽ bị ảnh hưởng bởi knockback. Sau khoảng này, enemy mới quay lại trạng thái bình thường.
    private float knockbackTimer = 0f; // Biến đếm ngược thời gian hiệu lực của knockback. Mỗi frame (Update()), giá trị này sẽ giảm dần về 0.

    private float knockbackCooldown = 0.3f; // khoảng thời gian miễn nhiễm knockback để người chơi chỉ có thể knockback 1 lần mỗi Burst
    private float knockbackCooldownTimer = 0f;

    private bool isStunned = false;
    private float stunDuration;
    [SerializeField] private float baseStunDuration = 0.2f; // Thời gian stun mặc định, có thể thay đổi theo từng loại enemy
    private float stunTimer = 0f;
    /*
    knockbackDuration: Là thời gian cố định mà một enemy nên bị knockback (giống như cấu hình gốc).
    knockbackTimer: Là thời gian còn lại mà enemy vẫn còn bị knockback (giảm dần theo thời gian thực).
    Tách knockbackDuration & knockbackTimer:
    - Dễ quản lý thời lượng chuẩn
    - Dễ điều chỉnh hoặc tùy biến theo loại enemy
    - Giúp code rõ ràng, có tính mở rộng cao
     */
    protected bool isDead = false; // Đánh dấu enemy đã chết hay chưa

    // ------------------ THUỘC TÍNH ------------------
    [Header("Obstacle Avoidance")]
    public float avoidTime = 4f;                 // thời gian giữ hướng né
    public float obstacleDetectDistance = 3f;    // tầm phát hiện chướng ngại
    public LayerMask obstacleLayer;              // gán Border ở Inspector
    public int maxAvoidAttempts = 30;            // tránh vòng lặp vô hạn

    private Vector2 currentDirection;
    private float avoidTimer;
    private bool isAvoiding;

    private readonly float selfCastOffset = 0.1f;   // đẩy raycast khỏi collider enemy

    private float pathUpdateRate = 0.5f; // Chỉ tìm đường mới mỗi 0.5s

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        obstacleLayer = LayerMask.GetMask("Border");
    }
    protected virtual void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        player = FindAnyObjectByType<Player>(); // tìm player trong scene để theo dõi vị trí của player <Player> là tên class của player
        gameManager = FindAnyObjectByType<GameManager>();
        currentHP = maxHP;
        UpdateHPBar(); // khi game bắt đầu, cập nhật thanh máu
        UpdateRageModeStats();

        //EventManager.Instance.OnPlayerMoved += OnPlayerMoved;
        //groundTilemap = MultiZoneGenerator.Instance.groundTilemap;
        //StartCoroutine(UpdatePathRoutine());
    }
    //IEnumerator UpdatePathRoutine()
    //{
    //    while (true)
    //    {
    //        CalculatePath();
    //        yield return new WaitForSeconds(1f); // cập nhật đường đi mỗi giây
    //    }
    //}
    private void OnPlayerMoved(Vector2 newPos)
    {
        var newPath = AStarPathfinder.Instance.FindPath(transform.position, newPos);
        if (newPath != null)
        {
            currentPath = newPath;
            currentPathIndex = 0;
        }
    }

    protected virtual void Update()
    {
        // Nếu không bị knockback hay stun thì mới di chuyển
        //if (!isKnockedBack && !isStunned)
        //{
        //    //MoveToPlayer();
        //    if (path == null || path.Count == 0 || currentPathIndex >= path.Count) return;

        //    targetWorldPos = groundTilemap.CellToWorld((Vector3Int)path[currentPathIndex]) + new Vector3(0.5f, 0.5f, 0);
        //    transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, enemyMoveSpeed * Time.deltaTime);

        //    if (Vector3.Distance(transform.position, targetWorldPos) < 0.1f)
        //    {
        //        currentPathIndex++;
        //    }
        //}
        if (!isKnockedBack && !isStunned)
        {
            //if (player == null) return;

            //Vector2 direction = (player.transform.position - transform.position).normalized;

            //// Kiểm tra có chướng ngại vật phía trước
            //RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, avoidDistance, obstacleMask);
            //if (hit.collider != null)
            //{
            //    // Có tường phía trước, thử tìm hướng khác
            //    direction = FindAlternativeDirection(direction);
            //}

            //rb.linearVelocity = direction * enemyMoveSpeed;

            //FlipEnemy(direction);
            MoveToPlayer();
            //MoveAlongPath();
        }
        

        UpdateRageModeStats();
        HandleKnockbackWithStun();
    }
    //void FlipEnemy(Vector2 dir)
    //{
    //    if (dir.x > 0.1f)
    //        transform.localScale = new Vector3(1, 1, 1);
    //    else if (dir.x < -0.1f)
    //        transform.localScale = new Vector3(-1, 1, 1);
    //}

    //Vector2 FindAlternativeDirection(Vector2 originalDir)
    //{
    //    Vector2[] alternatives = {
    //        Vector2.Perpendicular(originalDir).normalized,
    //        -Vector2.Perpendicular(originalDir).normalized,
    //        new Vector2(originalDir.y, -originalDir.x).normalized,
    //        new Vector2(-originalDir.y, originalDir.x).normalized
    //    };

    //    foreach (var dir in alternatives)
    //    {
    //        if (!Physics2D.Raycast(transform.position, dir, avoidDistance, obstacleMask))
    //            return dir;
    //    }

    //    return Vector2.zero; // không tìm được đường nào tránh
    //}

    //protected virtual void FixedUpdate()
    //{
    //    HandleKnockbackWithStun(stunDuration);
    //}
    protected void UpdateRageModeStats()
    {
        bool isRageActive = gameManager.IsRageActive();
        float rageStunDurationMultiplier = isRageActive ? player.GetRageStunDurationMultiplier() : 1f;

        stunDuration = baseStunDuration * rageStunDurationMultiplier;
    }
    public void HandleKnockbackWithStun()
    {
        // Giảm cooldown knockback theo thời gian
        if (knockbackCooldownTimer > 0f)
        {
            knockbackCooldownTimer -= Time.deltaTime;
        }

        // Nếu đang bị choáng → không làm gì cả
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
                isStunned = false;

            return;
        }

        // Nếu đang bị knockback → xử lý knockback timer
        if (isKnockedBack) // Kiểm tra nếu enemy đang bị knockback → không gọi MoveToPlayer() nữa.
        {
            knockbackTimer -= Time.deltaTime; // Mỗi frame giảm giá trị timer dựa vào thời gian thực
            if (knockbackTimer <= 0f) // Khi timer hết, trạng thái knockback kết thúc → enemy trở lại bình thường, có thể bị knockback tiếp.
            {
                isKnockedBack = false;

                // Nếu bạn vẫn muốn stun sau knockback
                Stun(stunDuration);
            }
            return; // không cho enemy di chuyển nếu đang bị knockback
        }

    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem va chạm có phải với Player không
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!player.IsInvincible)
            {
                Player player = collision.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    Debug.Log("Enemy va chạm Player → gây damage và knockback");
                    // Gọi phương thức TakeDamage của Player để trừ máu
                    player.TakeDamage(enterDamage * 0.5f);
                    player.KnockBack(transform.position, 10f); // Gọi phương thức KnockBack của Player để đẩy lùi player
                }
            }
            
        }
    }
    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (!player.IsInvincible)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                Debug.Log("Enemy va chạm Player → gây damage và knockback");
                // Gọi phương thức TakeDamage của Player để trừ máu
                player.TakeDamage(enterDamage);
                player.KnockBack(transform.position, 10f); // Gọi phương thức KnockBack của Player để đẩy lùi player
            }
        }
    }
    //bool IsPathBlocked(Vector2 direction)
    //{
    //    float distance = Vector2.Distance(transform.position, player.transform.position);
    //    RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, LayerMask.GetMask("Border"));
    //    return hit.collider != null;
    //}
    //Vector2[] directions = new Vector2[]
    //{
    //    new Vector2(1, 0).normalized,     // Right
    //    new Vector2(1, 1).normalized,     // Up-Right
    //    new Vector2(0, 1).normalized,     // Up
    //    new Vector2(-1, 1).normalized,    // Up-Left
    //    new Vector2(-1, 0).normalized,    // Left
    //    new Vector2(-1, -1).normalized,   // Down-Left
    //    new Vector2(0, -1).normalized,    // Down
    //    new Vector2(1, -1).normalized,    // Down-Right

    //    // Các hướng phụ để làm mượt hơn
    //    new Vector2(1, 0.5f).normalized,
    //    new Vector2(0.5f, 1).normalized,
    //    new Vector2(-1, 0.5f).normalized,
    //    new Vector2(-0.5f, 1).normalized,
    //    new Vector2(-1, -0.5f).normalized,
    //    new Vector2(-0.5f, -1).normalized,
    //    new Vector2(1, -0.5f).normalized,
    //    new Vector2(0.5f, -1).normalized
    //};

    //Vector2 FindAlternativeDirection()
    //{
    //    foreach (var dir in directions)
    //    {
    //        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1f, LayerMask.GetMask("Border"));
    //        if (hit.collider == null)
    //        {
    //            return dir;
    //        }
    //    }

    //    return Vector2.zero; // không tìm được hướng né
    //}


    //protected void MoveToPlayer()
    //{
    //    if (player != null)
    //    {
    //        Vector2 direction = (player.transform.position - transform.position).normalized;
    //        rb.linearVelocity = direction * enemyMoveSpeed;
    //        FlipEnemy(); // nếu bạn muốn flip hình ảnh enemy
    //    }
    //}
    // ------------------ MOVE LOOP ------------------
    //protected void MoveToPlayer()
    //{
    //    if (player == null) return;

    //    Vector2 toPlayerDir = (player.transform.position - transform.position).normalized;

    //    // -------------------- KHÔNG né --------------------
    //    if (!isAvoiding)
    //    {
    //        currentDirection = toPlayerDir;

    //        if (IsObstacleAhead(currentDirection))
    //        {
    //            TryPickNewDirection();  // sẽ bật isAvoiding = true
    //        }
    //    }
    //    // -------------------- ĐANG né --------------------
    //    else
    //    {
    //        /* ➊ Kiểm tra liên tục đường thẳng tới player */
    //        if (!IsObstacleAhead(toPlayerDir))
    //        {
    //            // Đã thông → quay lại truy đuổi ngay
    //            isAvoiding = false;
    //            currentDirection = toPlayerDir;
    //        }
    //        else
    //        {
    //            /* ➋ Vẫn bị chặn → tiếp tục logic né cũ */
    //            avoidTimer -= Time.deltaTime;

    //            if (avoidTimer <= 0f)
    //            {
    //                // Nếu hướng hiện tại vẫn bị chặn, chọn hướng mới
    //                if (IsObstacleAhead(currentDirection))
    //                {
    //                    TryPickNewDirection();
    //                }
    //                else
    //                {
    //                    // Hết né, nhưng còn chặn? (hiếm) → vẫn cứ thoát
    //                    isAvoiding = false;
    //                    currentDirection = toPlayerDir;
    //                }
    //            }
    //        }
    //    }

    //    // Di chuyển
    //    rb.linearVelocity = currentDirection * enemyMoveSpeed;
    //    FlipEnemy();
    //}


    //// ------------------ HỖ TRỢ ------------------
    //private bool IsObstacleAhead(Vector2 dir)
    //{
    //    Vector2 origin = (Vector2)transform.position + dir * selfCastOffset;
    //    RaycastHit2D hit = Physics2D.CircleCast(origin, 1.5f, dir, obstacleDetectDistance, obstacleLayer);
    //    return hit.collider != null;

    //}

    //private void TryPickNewDirection()
    //{
    //    Vector2 toPlayer = (player.transform.position - transform.position).normalized;

    //    // Danh sách góc lệch cố định
    //    List<int> angles = new List<int> { 45, -45, 90, -90, 135, -135, 180, -180 };

    //    // Xáo 1 lần duy nhất
    //    for (int i = angles.Count - 1; i > 0; --i)
    //    {
    //        int j = UnityEngine.Random.Range(0, i + 1);
    //        (angles[i], angles[j]) = (angles[j], angles[i]);
    //    }

    //    int attempts = 0;
    //    bool found = false;

    //    // Lặp lại quá trình thử cho đến khi tìm được hướng hợp lệ hoặc hết số lần cho phép
    //    while (!found && attempts < maxAvoidAttempts)
    //    {
    //        foreach (int angle in angles)
    //        {
    //            Vector2 dir = Quaternion.Euler(0, 0, angle) * currentDirection;

    //            // Chỉ chọn các hướng phía trước (dot ≥ 0)
    //            if (Vector2.Dot(dir.normalized, toPlayer) < 0f) continue;

    //            if (!IsObstacleAhead(dir))
    //            {
    //                currentDirection = dir.normalized;
    //                found = true;
    //                break;
    //            }
    //        }

    //        attempts++;
    //    }

    //    // Nếu không tìm được hướng tránh hợp lệ, fallback: cứ lao tới player
    //    if (!found)
    //    {
    //        currentDirection = toPlayer;
    //    }

    //    isAvoiding = true;
    //    avoidTimer = avoidTime;
    //}
    //---
    private float pathUpdateInterval = 0.5f;
    private float pathUpdateTimer;
    private List<Vector2> currentPath;
    private int currentPathIndex;

    protected void MoveToPlayer()
    {
        if (player == null)
            return;

        pathUpdateTimer -= Time.deltaTime;

        // Nếu không có vật cản trực tiếp, di chuyển thẳng
        if (!Physics2D.Linecast(transform.position, player.transform.position, LayerMask.GetMask("Border")))
        {
            // Xóa path cũ vì không cần nữa
            currentPath = null;
            currentPathIndex = 0;

            // Di chuyển thẳng
            Vector2 direction = (player.transform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemyMoveSpeed * Time.deltaTime);

            // Flip enemy nếu cần
            // FlipEnemy(direction.x);
            return;
        }

        // Nếu có vật cản, dùng A* pathfinding
        if (AStarPathfinder.Instance == null)
            return;

        if (pathUpdateTimer <= 0f)
        {
            Vector2 start = transform.position;
            Vector2 goal = player.transform.position;

            var newPath = AStarPathfinder.Instance.FindPath(start, goal, 1);

            if (newPath != null && newPath.Count > 0)
            {
                Vector2 currentTarget = currentPath != null && currentPathIndex < currentPath.Count
                    ? currentPath[currentPathIndex]
                    : (Vector2)transform.position;

                currentPath = newPath;

                int closestIndex = FindClosestPathIndex(newPath, transform.position);

                if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
                    currentPathIndex = closestIndex;
            }

            pathUpdateTimer = pathUpdateInterval;
        }

        if (currentPath == null || currentPathIndex >= currentPath.Count)
            return;

        Vector2 targetPos = currentPath[currentPathIndex];
        Vector2 moveDir = (targetPos - (Vector2)transform.position).normalized;

        transform.position = Vector2.MoveTowards(transform.position, targetPos, enemyMoveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.05f)
            currentPathIndex++;
    }

    private int FindClosestPathIndex(List<Vector2> path, Vector2 currentPos)
    {
        int closestIndex = 0;
        float closestDist = float.MaxValue;

        for (int i = 0; i < path.Count; i++)
        {
            float dist = Vector2.Distance(path[i], currentPos);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
    //---

    private void FlipEnemy()
    {
        //if (currentDirection.x != 0)
        //{
        //    Vector3 localScale = transform.localScale;
        //    localScale.x = Mathf.Sign(currentDirection.x) * Mathf.Abs(localScale.x);
        //    transform.localScale = localScale;
        //}
        if (currentDirection.x > 0.05f)
            transform.localScale = new Vector3(1, 1, 1); // hướng phải
        else if (currentDirection.x < -0.05f)
            transform.localScale = new Vector3(-1, 1, 1); // hướng trái
    }


    //protected void FlipEnemy()
    //{
    //    if (player != null)
    //    {
    //        transform.localScale = new Vector3(player.transform.position.x < transform.position.x ? -1 : 1, 1, 1); // lật hình ảnh enemy theo hướng di chuyển
    //    }
    //}

    [Obsolete]
    public virtual void TakeDamage(float damage)
    {
        if (!player.IsInvincible)
        {
            if (isDead) return; // Nếu enemy đã chết rồi thì không cần nhận sát thương nữa
            currentHP -= damage;
            currentHP = Mathf.Max(currentHP, 0); // giữ cho máu không nhỏ hơn 0
            UpdateHPBar(); // cập nhật thanh máu mỗi lần nhận sát thương
            if (currentHP <= 0)
            {
                Die();
            }
        }
            
    }

    [System.Obsolete]
    protected virtual void Die()
    {
        if (isDead) return; // Nếu enemy đã chết, không làm gì cả

        isDead = true; // Đánh dấu enemy đã chết
        GameManager gameManager = FindObjectOfType<GameManager>();
        GameManager2 gameManager2 = FindObjectOfType<GameManager2>();
        if (gameManager != null)
        {
            gameManager.AddEnergy();
            gameManager.AddScore(10);
            player.Heal(10f);
        }
        if (gameManager2 != null)
        {
            gameManager2.AddEnergy();
            //gameManager.AddScore(10);
            //player.Heal(10f);
        }

        Destroy(gameObject); // Xóa enemy khỏi scene
    }

    protected void UpdateHPBar()
    {
        if (HPBar != null)
        {
            HPBar.fillAmount = currentHP / maxHP; // cập nhật thanh máu
        }
    }
    public void Stun(float duration)
    {
        if (gameManager.IsRageActive())
        {
            isStunned = true;
            stunTimer = duration * player.GetRageStunDurationMultiplier();

            // Nếu đang knockback thì hủy luôn knockback
            isKnockedBack = false;
            knockbackTimer = 0f;

            // Option: Nếu muốn enemy đứng yên lập tức
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            isStunned = true;
            stunTimer = duration;

            // Nếu đang knockback thì hủy luôn knockback
            isKnockedBack = false;
            knockbackTimer = 0f;

            // Option: Nếu muốn enemy đứng yên lập tức
            rb.linearVelocity = Vector2.zero;
        }

    }
    public void KnockBack(Vector2 sourcePosition, float knockbackForce)
    {
        
        if (gameManager.IsRageActive())
        {
            // Nếu đang trong thời gian miễn knockback → không knockback nữa
            if (knockbackCooldownTimer > 0f) return;

            // Tính hướng knockback từ vị trí tấn công đến enemy
            Vector2 knockbackDirection = (transform.position - (Vector3)sourcePosition).normalized;

            // Thêm lực đẩy vào Rigidbody2D
            rb.AddForce(knockbackDirection * knockbackForce * player.GetRageKnockbackForceMultiplier(), ForceMode2D.Impulse);

            // Kích hoạt trạng thái knockback
            isKnockedBack = true;
            knockbackTimer = knockbackDuration;

            // Bắt đầu thời gian miễn knockback (cooldown)
            knockbackCooldownTimer = knockbackCooldown;
        }
        else
        {
            // Nếu đang trong thời gian miễn knockback → không knockback nữa
            if (knockbackCooldownTimer > 0f) return;

            // Tính hướng knockback từ vị trí tấn công đến enemy
            Vector2 knockbackDirection = (transform.position - (Vector3)sourcePosition).normalized;

            // Thêm lực đẩy vào Rigidbody2D
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

            // Kích hoạt trạng thái knockback
            isKnockedBack = true;
            knockbackTimer = knockbackDuration;

            // Bắt đầu thời gian miễn knockback (cooldown)
            knockbackCooldownTimer = knockbackCooldown;
        }
    }


}
//    Vector2 knockbackDirection = (transform.position - sourcePosition).normalized;:
//    Tính hướng từ nguồn gây knockback (ví dụ viên đạn) → đẩy enemy đi ra xa khỏi nguồn.
//    .normalized: đảm bảo độ dài vector là 1 để áp lực knockback chỉ phụ thuộc knockbackForce.
//    rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);:
//    Gán lực đẩy ngay lập tức (giống cú hích) lên Rigidbody2D.
//    ForceMode2D.Impulse: tạo lực tác động ngay tức thì (khác với ForceMode2D.Force là đẩy liên tục, chậm hơn).
//    isKnockedBack = true;:
//    Bật trạng thái knockback để ngắt hành vi di chuyển thông thường.
//    knockbackTimer = knockbackDuration;:
//    Reset thời gian knockback → hệ thống sẽ đếm ngược đến khi hết hiệu ứng.

//Giả sử bạn không tách knockbackTimer và knockbackDuration, và chỉ dùng mỗi knockbackTimer, thì bạn sẽ mất thông tin gốc về thời lượng knockback ban đầu.
//Ví dụ:
//public void KnockBack(Vector2 sourcePosition)
//{
//    ...
//    knockbackTimer = 0.2f; // hard-code giá trị ngay tại đây
//}
//➡ Điều này khiến việc chỉnh sửa thời gian knockback không còn linh hoạt nữa — bạn phải sửa ở nhiều chỗ nếu muốn thay đổi thời gian sau này.

// PriorityQueue đơn giản
public class PriorityQueue<T>
{
    private List<KeyValuePair<T, int>> elements = new List<KeyValuePair<T, int>>();

    public int Count => elements.Count;

    public void Enqueue(T item, int priority)
    {
        elements.Add(new KeyValuePair<T, int>(item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;
        for (int i = 1; i < elements.Count; i++)
        {
            if (elements[i].Value < elements[bestIndex].Value)
                bestIndex = i;
        }

        T bestItem = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }

    public bool Contains(T item)
    {
        return elements.Exists(e => EqualityComparer<T>.Default.Equals(e.Key, item));
    }
}
//[RequireComponent(typeof(Rigidbody2D))]
//public class EnemySmooth : MonoBehaviour
//{
//    public float moveSpeed = 3f;
//    public Transform player;
//    public LayerMask obstacleMask;
//    public float avoidDistance = 0.5f;

//    private Rigidbody2D rb;

//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//    }

//    void FixedUpdate()
//    {
//        if (!isKnockedBack && !isStunned)
//        {
//            if (player == null) return;

//            Vector2 direction = (player.position - transform.position).normalized;

//            // Kiểm tra có chướng ngại vật phía trước
//            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, avoidDistance, obstacleMask);
//            if (hit.collider != null)
//            {
//                // Có tường phía trước, thử tìm hướng khác
//                direction = FindAlternativeDirection(direction);
//            }

//            rb.linearVelocity = direction * moveSpeed;

//            FlipEnemy(direction);
//        }
        
//    }

//    void FlipEnemy(Vector2 dir)
//    {
//        if (dir.x > 0.1f)
//            transform.localScale = new Vector3(1, 1, 1);
//        else if (dir.x < -0.1f)
//            transform.localScale = new Vector3(-1, 1, 1);
//    }

//    Vector2 FindAlternativeDirection(Vector2 originalDir)
//    {
//        Vector2[] alternatives = {
//            Vector2.Perpendicular(originalDir).normalized,
//            -Vector2.Perpendicular(originalDir).normalized,
//            new Vector2(originalDir.y, -originalDir.x).normalized,
//            new Vector2(-originalDir.y, originalDir.x).normalized
//        };

//        foreach (var dir in alternatives)
//        {
//            if (!Physics2D.Raycast(transform.position, dir, avoidDistance, obstacleMask))
//                return dir;
//        }

//        return Vector2.zero; // không tìm được đường nào tránh
//    }
//}