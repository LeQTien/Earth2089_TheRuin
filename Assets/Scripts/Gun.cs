using System.Collections;
using TMPro;
using UnityEngine;

public class Gun : AbstractGunClass
{
    [SerializeField] private Transform firePosition; // vị trí bắn

    private float shotDelay; // độ trễ giữa các lần bắn
    [SerializeField] private float reloadTime = 1f; // thời gian nạp đạn

    private float shotTime; // thời điểm được phép bắn (tính theo giây, dùng để kiểm soát tốc độ bắn)
    private float reloadEndTime; // thời điểm kết thúc nạp đạn

    [SerializeField] private int maxAmmo = 30; // số lượng viên đạn tối đa
    [SerializeField] private int currentAmmo; // số lượng viên đạn còn lại

    private bool isReloading = false; // kiểm tra xem đang nạp đạn hay không
    private bool isShooting = false; // kiểm tra xem đang bắn hay không

    [SerializeField] private TextMeshProUGUI ammoText; // hiển thị số lượng viên đạn còn lại

    private float burstShotTime;
    [SerializeField] private float burstShotDelay = 0.08f;
    private float eachBurstShotDelay;

    [SerializeField] private TextMeshProUGUI ammoText_UI;

    [SerializeField] private float baseGunFireRateMultiplier = 1f;
    [SerializeField] private float baseGunDelayTimeInBetween2BurstMultiplier = 2.4f;

    [SerializeField] private Sprite weaponIcon;
    public Sprite WeaponIcon => weaponIcon;

    protected override void Start()
    {
        base.Start();
        currentAmmo = maxAmmo;
        isReloading = false;
        isShooting = false;
        //gunGameObject.SetActive(true);
        UpdateFireRate();
        UpdateAmmoText();

    }
    public void UpdateFireRate()
    {
        if (gameManager.IsRageActive())
        {
            baseFireRate = baseFireRate_Static / player.GetRageFireRateDivider();
        }
        else
        {
            baseFireRate = baseFireRate_Static;
        }
        shotDelay = baseFireRate * baseGunFireRateMultiplier;
        eachBurstShotDelay = baseFireRate * baseGunDelayTimeInBetween2BurstMultiplier;
    }

    protected void Update()
    {


        RotateGun();
        HandleShooting();
        Reload();
        UpdateFireRate();


        if (isShooting == true && Time.time > shotTime)
        {
            isShooting = false;
        }
        if (isReloading == true && Time.time > reloadEndTime)
        {
            isReloading = false;
        }
    }
    
    void HandleShooting()
    {
        NormalShooting();
        BurstShooting();
        UpdateAmmoText();
    }

    void NormalShooting()
    {
        // kiểm tra xem người chơi có nhấn chuột trái,
        // số lượng viên đạn còn lại có lớn hơn 0,
        // thời điểm bắn phải lớn hơn thời điểm được phép bắn
        if (Input.GetMouseButton(0) && currentAmmo > 0 && Time.time > shotTime && Time.time > burstShotTime)
        {
            isShooting = true;
            audioManager.PlayShootSound();
            //gunGameObject.SetActive(true);
            //shotGunGameObject.SetActive(false);
            // tạo viên đạn
            Instantiate(bulletPrefab, firePosition.position, firePosition.rotation);
            // Cập nhật thời điểm được phép bắn tiếp theo
            shotTime = Time.time + shotDelay;
            // giảm số lượng viên đạn còn lại
            currentAmmo--;
        }
    }
    void BurstShooting()
    {
        if (Input.GetMouseButton(1) && currentAmmo > 0 && Time.time > burstShotTime && Time.time > shotTime)
        {
            //shotGunGameObject.SetActive(true);
            //gunGameObject.SetActive(false);

            //isShooting = true;
            StartCoroutine(BurstShoot());
            burstShotTime = Time.time + burstShotDelay * 3 + eachBurstShotDelay; // eachBurstShotDelay là thời gian đợi giữa các đợt bắn burst
        }

    }

    private IEnumerator BurstShoot()
    {
        int burstCount = Mathf.Min(3, currentAmmo); // số lượng viên đạn bắn ra trong một đợt burst
        for (int i = 0; i < burstCount; i++)
        {
            if (currentAmmo > 0)
            {
                isShooting = true;
                audioManager.PlayShootSound();
                Instantiate(bulletPrefab, firePosition.position, firePosition.rotation);
                currentAmmo--;
                UpdateAmmoText();
                yield return new WaitForSeconds(burstShotDelay);
            }
            if (currentAmmo == 0)
            {
                Reload();
            }
        }
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo || currentAmmo == 0)
        {
            isReloading = true;
            audioManager.PlayReLoadSound();
            // cập nhật thời điểm được phép bắn sau khi nạp đạn
            reloadEndTime = Time.time + reloadTime;
            shotTime = reloadEndTime;
            burstShotTime = reloadEndTime;
            currentAmmo = maxAmmo;
        }
        UpdateAmmoText();
    }

    public bool IsShooting()
    {
        return isShooting;
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    private void UpdateAmmoText()
    {
        //if (gameObject.activeSelf != true)
        //{
        //    ammoText.gameObject.SetActive(false);
        //    ammoText_UI.gameObject.SetActive(false);
        //}
        if (ammoText != null)
        {
            if (Time.time >= reloadEndTime)
            {
                ammoText.text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
                ammoText_UI.text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
            }
            else
            {
                ammoText.text = "Reloading...";
                ammoText_UI.text = "Reloading...";
            }
        }
    }

    //public float GetShotDelay()
    //{
    //    return shotDelay;
    //}
    //public float GetEachBurstShotDelay()
    //{
    //    return eachBurstShotDelay;
    //}
}
