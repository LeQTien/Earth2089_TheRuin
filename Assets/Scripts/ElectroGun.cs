using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElectroGun : AbstractGunClass
{
    [SerializeField] private Transform firePosition; // vị trí bắn
    [SerializeField] private float electroShootCooldown = 1f; // độ trễ giữa các lần bắn
    [SerializeField] private float unexplodableTime = 0.2f;

    private float nextShootTime;
    private ElectroBall activeElectroBall; // Lưu viên đạn đã bắn ra
    private bool canExplode = false; // Kiểm tra có thể kích nổ không

    //private GameManager gameManager;
    private float maxEnergyThreshold;
    private float currentEnergyPoint;
    [SerializeField] private float EnergyPointRequiredToShoot = 5f;

    //private GameObject electroShootingEffect;
    //private Animator electroShootingEffectAnimator;
    [SerializeField] private GameObject shootingEffectPrefab;
    [SerializeField] private float effectDuration = 0.3f; // Thời gian hiển thị hiệu ứng


    [Header("Electro Gun Settings")]
    [SerializeField] private float shootCooldown = 1f;
    [SerializeField] private float holdEnergyCostPerSecond = 10f;
    [SerializeField] private GameObject electroBeamPrefab;
    private GameObject activeBeam;
    private float holdTimer;
    private bool isBeamActive = false;

    [SerializeField] private Image playerElectroGunIcon;
    [SerializeField] private TextMeshProUGUI playerElectroGunCoolDownText;
    [SerializeField] private GameObject skillsUI;

    private float electroGunCooldownTimer;
    //private bool isElectroGunActive = false; // Trạng thái của ElectroGun
    //private float electroGunCooldown = 5f; // Ví dụ thời gian cooldown của ElectroGun

    override protected void Start()
    {
        base.Start();
        UpdateElectroGunCoolDownUI();
    }

    void Update()
    {
        RotateGun();

        // Cập nhật điểm năng lượng hiện tại
        currentEnergyPoint = gameManager.ReturnPlayerCurrentEnergyPoint();
        maxEnergyThreshold = gameManager.ReturnPlayerEnergyPointThreshold();

        HandleShooting();
        HandleExplosion();

        // Cập nhật cooldown UI cho ElectroGun
        UpdateElectroGunCoolDownUI();
    }

    private void HandleShooting()
    {
        //electroShootingEffectAnimator.SetTrigger("Shoot"); // Chạy animation bắn
        // Kiểm tra điều kiện trước khi bắn
        if (Input.GetMouseButtonDown(0) && Time.time > nextShootTime && currentEnergyPoint >= EnergyPointRequiredToShoot)
        {
            ////electroShootingEffectAnimator.SetTrigger("Shoot"); // Chạy animation bắn
            //shootingEffect.SetActive(true); // Bật hiệu ứng
            ////electroShootingEffectAnimator.Play("Shoot", 0, 0f); // Chạy animation từ đầu

            //// Tắt hiệu ứng sau effectDuration giây
            //Invoke(nameof(HideShootingEffect), effectDuration);

            // Tạo hiệu ứng bắn từ Prefab
            GameObject shootingEffect = Instantiate(shootingEffectPrefab, firePosition.position, firePosition.rotation);

            // Hủy hiệu ứng sau effectDuration giây
            Destroy(shootingEffect, effectDuration);

            // Trừ năng lượng khi bắn
            currentEnergyPoint -= EnergyPointRequiredToShoot;
            gameManager.UpdatePlayerEnergyPoint(currentEnergyPoint); // Cập nhật năng lượng vào GameManager
            gameManager.UpdateEnergyPointText(); // Cập nhật text UI
            // Bắn viên ElectroBall mới
            activeElectroBall = Instantiate(bulletPrefab, firePosition.position, firePosition.rotation).GetComponent<ElectroBall>();

            // Cài đặt thời gian có thể kích nổ
            canExplode = false;
            Invoke(nameof(EnableExplosion), unexplodableTime);

            // Cập nhật thời gian bắn tiếp theo
            nextShootTime = Time.time + electroShootCooldown;
        }

        // Giữ chuột phải: bắn beam liên tục
        if (Input.GetMouseButton(1) && currentEnergyPoint >= 10f)
        {
            // Nếu chưa có beam, tạo beam mới
            if (!isBeamActive)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;  // Đảm bảo beam chỉ di chuyển trong không gian 2D

                // Tính góc quay từ vị trí súng đến chuột
                Vector3 direction = mousePosition - firePosition.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;  // Xoay beam theo chuột (cộng 90 độ để hướng beam đúng)
                Quaternion beamRotation = Quaternion.Euler(0f, 0f, angle);  // Quay beam theo trục Z

                // Instantiate beam và xoay nó theo hướng chuột
                activeBeam = Instantiate(electroBeamPrefab, firePosition.position, beamRotation, transform);
                isBeamActive = true;  // Đánh dấu beam đã được tạo
            }

            // Tiêu hao năng lượng mỗi giây
            holdTimer += Time.deltaTime;
            if (holdTimer >= 1f)
            {
                currentEnergyPoint -= holdEnergyCostPerSecond;
                currentEnergyPoint = Mathf.Max(currentEnergyPoint, 0f);  // Không cho năng lượng dưới 0
                gameManager.UpdatePlayerEnergyPoint(currentEnergyPoint); // Cập nhật năng lượng vào GameManager
                gameManager.UpdateEnergyPointText(); // Cập nhật text UI
                holdTimer = 0f;
            }

            // Nếu hết năng lượng thì hủy beam
            if (currentEnergyPoint < holdEnergyCostPerSecond)
            {
                Destroy(activeBeam);
                isBeamActive = false;
            }
        }
        else if (Input.GetMouseButtonUp(1))  // Nếu thả chuột phải
        {
            // Hủy beam khi chuột phải được thả
            if (activeBeam != null)
            {
                Destroy(activeBeam);
                activeBeam = null;
                isBeamActive = false;
            }
        }

    }
    private void UpdateElectroGunCoolDownUI()
    {
        if (playerElectroGunCoolDownText != null && playerElectroGunIcon != null)
        {
            // Kiểm tra nếu không đủ năng lượng
            if (currentEnergyPoint < EnergyPointRequiredToShoot || currentEnergyPoint < holdEnergyCostPerSecond)
            {
                playerElectroGunCoolDownText.text = "E - " + "Not enough energy!";
                playerElectroGunIcon.fillAmount = 1f;
                playerElectroGunIcon.color = new Color(0.4f, 0.4f, 0.4f, 1f); // Làm tối icon
            }
            else
            {
                if (!player.GetIsElectroGunActive())
                {
                    playerElectroGunCoolDownText.text = "E - " + "Ready!";
                    playerElectroGunIcon.fillAmount = 1f;
                    playerElectroGunIcon.color = Color.white;
                }
                else
                {
                    playerElectroGunCoolDownText.text = "E - " + "Ready!";
                    playerElectroGunIcon.color = Color.white;
                }
            }
        }
    }


    private void HandleExplosion()
    {
        if (Input.GetMouseButtonDown(0) && canExplode && activeElectroBall != null)
        {
            activeElectroBall.CreateExplosion(); // Gọi hàm kích nổ của ElectroBall
            activeElectroBall = null; // Reset trạng thái
            canExplode = false;
        }
    }

    private void EnableExplosion()
    {
        canExplode = true;
    }
    
}
