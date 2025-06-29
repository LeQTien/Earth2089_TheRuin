using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField] private TextMeshProUGUI HPAmountText;

    // các biến xử lý thanh máu của player
    [SerializeField] private float maxHP = 300f;
    [SerializeField] private float baseMoveSpeed = 11f;
    private float moveSpeed; // Tốc độ hiện tại

    [SerializeField] private float currentHP;
    [SerializeField] private Image HPBar;

    [SerializeField] private GameManager gameManager; 
    [SerializeField] private GameObject theGun;
    [SerializeField] private GameObject theshotGun;
    [SerializeField] private GameObject theElectroGun;

    [SerializeField] private GameObject[] weapons;
    //[SerializeField] private GameObject[] weapons2;

    private int currentWeaponIndex = 0;
    //private int currentWeaponIndex2 = 0; // Chỉ số vũ khí trong weapons2[]
    private bool isUsingWeapons1 = true; // Để kiểm tra xem đang dùng weapons[] hay weapons2[]

    private bool isUsingElectroGun = false;
    private int previousWeaponIndex = 0;

    //[SerializeField] private ScoreManager scoreManager;
    //[SerializeField] private float knockBackForce = 5f; // Lực đẩy lùi
    [SerializeField] private TextMeshProUGUI HPAmountText_UI;
    [SerializeField] private Image HPBar_UI;

    private AbstractPlayerBulletClass abstractPlayerBulletClass;
    private Enemy abstractEnemyClass;
    private AbstractGunClass abstractGunClass;

    [SerializeField] private float rageSpeed = 15f;

    [SerializeField] private float rageDamageMultiplier = 1.5f;
    [SerializeField] private float rageMaxDistanceMultiplier = 1.5f;

    [SerializeField] private float rageKnockbackForceMultiplier = 1.5f;
    [SerializeField] private float rageStunDurationMultiplier = 1.5f;

    [SerializeField] private float rageFireRateDivider = 1.5f;
    [SerializeField] private float rageBulletSpeedMultiplier = 1.5f;

    [SerializeField] private float dashForce = 5f;  // Tốc độ dash
    [SerializeField] private float dashDuration = 0.2f; // Thời gian dash
    [SerializeField] private float dashCooldown = 1f; // Thời gian cooldown giữa các lần dash
    private float dashTime = 0f; // Thời gian còn lại của dash
    private float dashCooldownTime = 0f; // Thời gian còn lại của cooldown
    private bool isDashing = false;

    private bool isKnockedBack = false;
    private float knockbackDuration = 0.3f; // Khoảng thời gian enemy sẽ bị ảnh hưởng bởi knockback. Sau khoảng này, enemy mới quay lại trạng thái bình thường.
    private float knockbackTimer = 0f; // Biến đếm ngược thời gian hiệu lực của knockback. Mỗi frame (Update()), giá trị này sẽ giảm dần về 0.

    private float knockbackCooldown = 0.3f; // khoảng thời gian miễn nhiễm knockback để người chơi chỉ có thể knockback 1 lần mỗi Burst
    private float knockbackCooldownTimer = 0f;

    private bool isStunned = false;
    private float stunDuration = 0f;
    private float stunTimer = 0f;

    public bool IsInvincible => isDashing; // Trong PlayerController
    [SerializeField] private LayerMask obstacleLayer; // Layer chứa TilemapCollider

    public InventoryManager inventory;
    [SerializeField] private HotbarUIManager hotbarUIManager; // Kéo vào từ Editor

    public bool GetIsElectroGunActive()
    {
        return isUsingElectroGun;
    }

    public float GetRageDamageMultiplier()
    {
        return rageDamageMultiplier;
    }
    public float GetRageMaxDistanceMultiplier()
    {
        return rageMaxDistanceMultiplier;
    }
    public float GetRageKnockbackForceMultiplier()
    {
        return rageKnockbackForceMultiplier;
    }
    public float GetRageStunDurationMultiplier()
    {
        return rageStunDurationMultiplier;
    }
    public float GetRageFireRateDivider()
    {
        return rageFireRateDivider;
    }
    public float GetRageBulletSpeedMultiplier()
    {
        return rageBulletSpeedMultiplier;
    }

    // lấy tham chiếu thành phần Rigidbody2d, spriteRenderer và những thành phần khác của Player
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        abstractPlayerBulletClass = GetComponent<AbstractPlayerBulletClass>();
        abstractEnemyClass = GetComponent<Enemy>();
        abstractGunClass = GetComponent<AbstractGunClass>();

        inventory = FindObjectOfType<InventoryManager>();
        DontDestroyOnLoad(gameObject);


    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        currentHP = maxHP;
        UpdateHPBar();
        UpdateHPAmountText();
        theGun.SetActive(true);
        theshotGun.SetActive(false);

        //for (int i = 0; i < weapons2.Length; i++)
        //{
        //    weapons2[i].SetActive(false);
        //}
        theElectroGun.SetActive(false);

        hotbarUIManager.UpdateHotbarUI(weapons);
        hotbarUIManager.HighlightSlot(currentWeaponIndex);
    }

    void Update()
    {
        if (!GameManager.Instance.gameStarted) return;
        // gọi hàm di chuyển player
        if (!isDashing)
        {
            MovePlayer();
        }
        
        HandleChangeGun();
        HandleQuitAndPauseGame();
        HandleOpenInventory();
        HandleDashing();
        HandleKnockbackWithStun();
    }

    private void HandleOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventory != null)
            {
                inventory.ToggleInventory();
            }
            else
            {
                Debug.LogError("InventoryManager chưa được gán!");
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (inventory != null)
            {
                inventory.selectedHotbarItem.Use(this);
            }
            else
            {
                Debug.LogError("InventoryManager chưa được gán!");
            }
        }
    }
    private void HandleQuitAndPauseGame()
    {
        if (gameManager.IsGameMenuActive() == true && Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (gameManager.IsGameMenuActive() == false && Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.PauseGameMenu();
        }
    }
    private void MovePlayer()
    {
        if (isKnockedBack || isStunned || isDashing)
        {
            // Nếu đang knockback / stun / dash → không xử lý input di chuyển
            return;
        }

        // Kiểm tra trạng thái nộ
        if (gameManager.IsRageActive()) // Kiểm tra trạng thái nộ
        {
            moveSpeed = rageSpeed;  // Tăng tốc khi nộ bật
        }
        else
        {
            moveSpeed = baseMoveSpeed; // Reset tốc độ khi nộ tắt
        }

        // Lấy giá trị di chuyển từ input
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Di chuyển player, đảm bảo giữ tốc độ không thay đổi khi di chuyển theo đường chéo
        rb.linearVelocity = playerInput.normalized * moveSpeed;

        // Xoay sprite theo hướng di chuyển
        if (playerInput.x < 0)
        {
            spriteRenderer.flipX = true; // nhìn sang trái
        }
        else if (playerInput.x > 0)
        {
            spriteRenderer.flipX = false; // nhìn sang phải
        }

        // Cập nhật trạng thái animation khi di chuyển
        if (playerInput != Vector2.zero)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }
    public bool GetPlayerFaceDirection() // true: trái, false: phải
    {
        return spriteRenderer.flipX;
    }

    // hàm xử lý player va chạm với enemy
    public void TakeDamage(float damage)
    {
        // xử lý khi player va chạm với enemy
        // Die();

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0); // giữ cho máu không nhỏ hơn 0
        UpdateHPBar(); // cập nhật thanh máu mỗi lần nhận sát thương
        
        if (currentHP <= 0)
        {
            Die();
        }
        UpdateHPAmountText(); // cập nhật số lượng máu mỗi lần nhận sát thương
    }

    // hàm xử lý hồi máu cho player
    public void Heal(float healValue)
    {
        if (currentHP < maxHP)
        {
            currentHP += healValue;
            currentHP = Mathf.Min(currentHP, maxHP); // giữ cho máu không lớn hơn maxHP
            UpdateHPBar(); // cập nhật thanh máu mỗi lần hồi máu
            UpdateHPAmountText(); // cập nhật số lượng máu mỗi lần hồi máu
            //Debug.Log("Heal Player");
        }
    }

    private void Die()
    {
        //Destroy(gameObject);
        gameManager.GameOverMenu();
    }

    public void UpdateHPBar()
    {
        HPBar.fillAmount = currentHP / maxHP;
        HPBar_UI.fillAmount = currentHP / maxHP;

    }

    public void UpdateHPAmountText()
    {
        Debug.Log("Updating HP Amount Text");
        if (HPAmountText != null)
        {
            HPAmountText.text = currentHP.ToString();
            HPAmountText_UI.text = currentHP.ToString();
        }
        else
        {
            HPAmountText.text = "Empty!";
            HPAmountText_UI.text = "Empty!";
        }
    }

    private void HandleChangeGun()
    {
        //ChangeGunByButton();
        HandleChangeWeaponSetByButton();
        HandleSwitchToElectroGun();

    }

    //private void HandleChangeWeaponSetByButton()
    //{
    //    // Nếu đang bật Electro Gun và người chơi nhấn 1 hoặc 2 → Tắt Electro Gun và quay lại vũ khí trước đó trong mảng tương ứng
    //    if (isUsingElectroGun)
    //    {
    //        if (Input.GetKeyDown(KeyCode.Alpha1))
    //        {
    //            isUsingElectroGun = false;
    //            theElectroGun.SetActive(false);
    //            isUsingWeapons1 = true;
    //            SwitchWeaponSet(weapons, weapons2, ref currentWeaponIndex);
    //        }
    //        else if (Input.GetKeyDown(KeyCode.Alpha2))
    //        {
    //            isUsingElectroGun = false;
    //            theElectroGun.SetActive(false);
    //            isUsingWeapons1 = false;
    //            SwitchWeaponSet(weapons2, weapons, ref currentWeaponIndex2);
    //        }
    //        return; // Ngăn cuộn chuột khi đang tắt Electro Gun
    //    }

    //    // Nếu không dùng Electro Gun, cho phép đổi giữa hai mảng vũ khí bình thường
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        isUsingWeapons1 = true;
    //        SwitchWeaponSet(weapons, weapons2, ref currentWeaponIndex);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha2))
    //    {
    //        isUsingWeapons1 = false;
    //        SwitchWeaponSet(weapons2, weapons, ref currentWeaponIndex2);
    //    }

    //    // Cuộn chuột để đổi vũ khí trong nhóm hiện tại (KHÔNG LOOP)
    //    float scroll = Input.GetAxis("Mouse ScrollWheel");
    //    if (scroll != 0f)
    //    {
    //        if (isUsingWeapons1)
    //        {
    //            ChangeWeaponByScroll(weapons, ref currentWeaponIndex, scroll);
    //        }
    //        else
    //        {
    //            ChangeWeaponByScroll(weapons2, ref currentWeaponIndex2, scroll);
    //        }
    //    }
    //}
    //// 🛠️ Chuyển đổi vũ khí bằng cuộn chuột (KHÔNG LOOP)
    //private void ChangeWeaponByScroll(GameObject[] weaponSet, ref int currentWeaponIndex, float scroll)
    //{
    //    if (weaponSet.Length < 2) return; // Nếu chỉ có 1 vũ khí thì không đổi

    //    // Cuộn lên: Chỉ đổi nếu chưa ở vũ khí đầu tiên
    //    if (scroll > 0f && currentWeaponIndex > 0)
    //    {
    //        currentWeaponIndex--;
    //    }
    //    // Cuộn xuống: Chỉ đổi nếu chưa ở vũ khí cuối cùng
    //    else if (scroll < 0f && currentWeaponIndex < weaponSet.Length - 1)
    //    {
    //        currentWeaponIndex++;
    //    }

    //    SwitchWeaponSet(weaponSet, null, ref currentWeaponIndex);
    //}

    //// 🛠️ Bật lại vũ khí trước đó của nhóm khi đổi nhóm
    //private void SwitchWeaponSet(GameObject[] weaponSetToEnable, GameObject[] weaponSetToDisable, ref int currentWeaponIndex)
    //{
    //    if (weaponSetToDisable != null)
    //    {
    //        foreach (var weapon in weaponSetToDisable)
    //        {
    //            weapon.SetActive(false); // Tắt toàn bộ vũ khí nhóm cũ
    //        }
    //    }

    //    for (int i = 0; i < weaponSetToEnable.Length; i++)
    //    {
    //        weaponSetToEnable[i].SetActive(i == currentWeaponIndex); // Bật lại vũ khí trước đó của nhóm
    //    }
    //}

    //private void ChangeGunByButton()
    //{
    //    // Nếu đang dùng Electro Gun, KHÔNG cho phép đổi vũ khí bằng Tab
    //    if (isUsingElectroGun) return;

    //    // Nhấn "Tab" để chuyển đổi giữa vũ khí trong nhóm hiện tại
    //    if (Input.GetKeyDown(KeyCode.Tab))
    //    {
    //        if (isUsingWeapons1)
    //        {
    //            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
    //            SwitchWeaponSet(weapons, null, ref currentWeaponIndex);
    //        }
    //        else
    //        {
    //            currentWeaponIndex2 = (currentWeaponIndex2 + 1) % weapons2.Length;
    //            SwitchWeaponSet(weapons2, null, ref currentWeaponIndex2);
    //        }
    //    }
    //}

    //private void HandleSwitchToElectroGun()
    //{
    //    // Nhấn "E" để bật/tắt Electro Gun
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        if (isUsingElectroGun)
    //        {
    //            // Nếu đang dùng Electro Gun, tắt nó và quay về vũ khí trước đó
    //            isUsingElectroGun = false;
    //            theElectroGun.SetActive(false);

    //            // Bật lại vũ khí trước đó của mảng hiện tại
    //            if (isUsingWeapons1)
    //            {
    //                SwitchWeaponSet(weapons, null, ref currentWeaponIndex);
    //            }
    //            else
    //            {
    //                SwitchWeaponSet(weapons2, null, ref currentWeaponIndex2);
    //            }
    //        }
    //        else
    //        {
    //            // Nếu chưa dùng Electro Gun, lưu lại vũ khí hiện tại và bật Electro Gun
    //            previousWeaponIndex = isUsingWeapons1 ? currentWeaponIndex : currentWeaponIndex2;
    //            isUsingElectroGun = true;

    //            // Tắt tất cả vũ khí trong mảng hiện tại
    //            if (isUsingWeapons1)
    //            {
    //                foreach (var weapon in weapons) weapon.SetActive(false);
    //            }
    //            else
    //            {
    //                foreach (var weapon in weapons2) weapon.SetActive(false);
    //            }

    //            // Bật Electro Gun
    //            theElectroGun.SetActive(true);
    //        }
    //    }
    //}
    private void HandleChangeWeaponSetByButton()
    {
        if (isUsingElectroGun)
        {
            for (int i = 0; i < hotbarUIManager.SlotCount; i++)
            {
                KeyCode key = KeyCode.Alpha1 + i;
                if (Input.GetKeyDown(key))
                {
                    SetWeaponFromHotbar(i);
                    return;
                }
            }
        }

        // Bình thường: chọn vũ khí bằng phím số
        for (int i = 0; i < hotbarUIManager.SlotCount; i++)
        {
            KeyCode key = KeyCode.Alpha1 + i;
            if (Input.GetKeyDown(key))
            {
                SetWeaponFromHotbar(i);
                return;
            }
        }

        // Cuộn chuột để chuyển vũ khí
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
            ChangeWeaponByScroll(scroll);
    }
    //private void SetWeaponFromHotbar(int index)
    //{
    //    if (index < 0 || index >= hotbarUIManager.SlotCount) return;
    //    if (index >= weapons.Length || weapons[index] == null) return;

    //    isUsingElectroGun = false;
    //    theElectroGun.SetActive(false);
    //    currentWeaponIndex = index;
    //    SwitchWeapon(currentWeaponIndex);
    //    hotbarUIManager.HighlightSlot(index); // Đảm bảo Selector cập nhật
    //}
    private void SetWeaponFromHotbar(int index)
    {
        if (index < 0 || index >= hotbarUIManager.SlotCount) return;

        currentWeaponIndex = index;
        isUsingElectroGun = false;
        theElectroGun.SetActive(false);

        if (index < weapons.Length && weapons[index] != null)
        {
            SwitchWeapon(index);
        }
        else
        {
            // Không có vũ khí -> disable tất cả
            foreach (var weapon in weapons)
            {
                if (weapon != null) weapon.SetActive(false);
            }
        }

        hotbarUIManager.HighlightSlot(index); // Dù có vũ khí hay không, vẫn highlight
    }


    //private void ChangeWeaponByScroll(float scroll)
    //{
    //    if (weapons.Length < 2) return;

    //    if (scroll > 0f && currentWeaponIndex > 0)
    //        currentWeaponIndex--;
    //    else if (scroll < 0f && currentWeaponIndex < weapons.Length - 1)
    //        currentWeaponIndex++;

    //    SwitchWeapon(currentWeaponIndex);
    //}
    //private void ChangeWeaponByScroll(float scroll)
    //{
    //    int slotCount = hotbarUIManager.SlotCount;

    //    if (slotCount < 2) return;

    //    if (scroll > 0f)
    //    {
    //        currentWeaponIndex--;
    //        if (currentWeaponIndex < 0)
    //            currentWeaponIndex = slotCount - 1;
    //    }
    //    else if (scroll < 0f)
    //    {
    //        currentWeaponIndex++;
    //        if (currentWeaponIndex >= slotCount)
    //            currentWeaponIndex = 0;
    //    }

    //    SetWeaponFromHotbar(currentWeaponIndex);
    //}
    private void ChangeWeaponByScroll(float scroll)
    {
        int maxIndex = hotbarUIManager.SlotCount - 1; // dùng số lượng slot thay vì weapons.Length

        int newIndex = currentWeaponIndex;

        if (scroll < 0f && currentWeaponIndex > 0)
        {
            newIndex = currentWeaponIndex - 1;
        }
        else if (scroll > 0f && currentWeaponIndex < maxIndex)
        {
            newIndex = currentWeaponIndex + 1;
        }

        SetWeaponFromHotbar(newIndex);
    }


    private void SwitchWeapon(int indexToEnable)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == indexToEnable);
        }
    }
    private void HandleSwitchToElectroGun()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isUsingElectroGun)
            {
                isUsingElectroGun = false;
                theElectroGun.SetActive(false);
                SwitchWeapon(currentWeaponIndex);
            }
            else
            {
                previousWeaponIndex = currentWeaponIndex;
                isUsingElectroGun = true;

                foreach (var weapon in weapons)
                    weapon.SetActive(false);

                theElectroGun.SetActive(true);
            }
        }
    }


    private void HandleDashing()
    {
        // Kiểm tra nếu nhấn phím Space và cooldown đã hết và chưa dashing
        if (Input.GetKey(KeyCode.Space) && !isDashing && dashCooldownTime <= 0)
        {
            Vector2 dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            if (dashDirection == Vector2.zero)
                dashDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;

            float dashDistance = moveSpeed * dashForce * dashDuration; // Khoảng cách dash cố định

            // Kiểm tra va chạm với tường (obstacleLayer là layer của tường)
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, dashDistance, obstacleLayer);

            // Nếu có va chạm, tính lại khoảng cách để không va chạm
            if (hit.collider != null)
                dashDistance = hit.distance - 0.1f; // Dừng lại trước khi va chạm với tường

            dashCooldownTime = dashCooldown; // Bắt đầu cooldown

            StartCoroutine(PerformDash(dashDirection, dashDistance));
        }

        // Giảm cooldown mỗi frame
        if (dashCooldownTime > 0)
            dashCooldownTime -= Time.deltaTime;
    }

    private IEnumerator PerformDash(Vector2 direction, float distance)
    {
        float dashSpeed = distance / dashDuration;
        float elapsed = 0f;

        isDashing = true;

        while (elapsed < dashDuration)
        {
            // Di chuyển player một khoảng cố định mỗi frame
            rb.MovePosition(rb.position + direction * dashSpeed * Time.fixedDeltaTime);
            elapsed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
    }

    public void Stun(float duration)
    {
        if (gameManager.IsRageActive())
        {
            isStunned = true;
            stunTimer = duration * GetRageStunDurationMultiplier();

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
            rb.AddForce(GetRageKnockbackForceMultiplier() * knockbackForce * knockbackDirection, ForceMode2D.Impulse);

            // Kích hoạt trạng thái knockback
            isKnockedBack = true;
            knockbackTimer = knockbackDuration;

            // Bắt đầu thời gian miễn knockback (cooldown)
            knockbackCooldownTimer = knockbackCooldown;

            Debug.Log("Player nhận knockback từ: " + sourcePosition + " → hướng: " + knockbackDirection);
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

            Debug.Log("Player nhận knockback từ: " + sourcePosition + " → hướng: " + knockbackDirection);
        }
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

}