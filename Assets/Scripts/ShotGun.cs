using System.Collections;
using TMPro;
using UnityEngine;

public class ShotGun : AbstractGunClass
{
    [SerializeField] private Transform shotGunFirePosition1; // vị trí bắn
    [SerializeField] private Transform shotGunFirePosition2; // vị trí bắn
    [SerializeField] private Transform shotGunFirePosition3; // vị trí bắn
    [SerializeField] private Transform shotGunFirePosition4; // vị trí bắn
    [SerializeField] private Transform shotGunFirePosition5; // vị trí bắn

    private float shotGunSingleShotDelay; // độ trễ giữa các lần bắn
    [SerializeField] private float shotGunReloadTime = 1f; // thời gian nạp đạn

    private float shotGunShotTime; // thời điểm được phép bắn (tính theo giây, dùng để kiểm soát tốc độ bắn)
    private float shotGunReloadEndTime; // thời điểm kết thúc nạp đạn

    [SerializeField] private int shotGunMaxAmmo = 30; // số lượng viên đạn tối đa
    [SerializeField] private int shotGunCurrentAmmo; // số lượng viên đạn còn lại

    private bool isReloading = false; // kiểm tra xem đang nạp đạn hay không
    private bool isShooting = false; // kiểm tra xem đang bắn hay không

    [SerializeField] private TextMeshProUGUI shotGunAmmoText; // hiển thị số lượng viên đạn còn lại

    private float shotGunBurstShotTime; // thời điểm bắn
    [SerializeField] private float shotGunBurstShotDelay = 0.1f; // đợi giữa 2 đợt bắn trong burst
    private float shotGunDelayTimeInBetween2Burst; // đợi giữa 2 lần bắn burst

    [SerializeField] private TextMeshProUGUI shotGunAmmoText_UI;

    [SerializeField] private float baseShotGunFireRateMultiplier = 5f;
    [SerializeField] private float baseShotGunDelayTimeInBetween2BurstMultiplier = 5f;

    [SerializeField] private Sprite weaponIcon;
    public Sprite WeaponIcon => weaponIcon;

    protected override void Start()
    {
        base.Start();
        shotGunCurrentAmmo = shotGunMaxAmmo;
        isReloading = false;
        isShooting = false;
        //shotGunGameObject.SetActive(true);
        UpdateFireRate();
        UpdateshotGunAmmoText();
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
        shotGunSingleShotDelay = baseFireRate * baseShotGunFireRateMultiplier;
        shotGunDelayTimeInBetween2Burst = baseFireRate * baseShotGunDelayTimeInBetween2BurstMultiplier;
    }

    protected void Update()
    {


        RotateGun();
        HandleShooting();
        Reload();
        UpdateFireRate();


        if (isShooting == true && Time.time > shotGunShotTime)
        {
            isShooting = false;
        }
        if (isReloading == true && Time.time > shotGunReloadEndTime)
        {
            isReloading = false;
        }

    }

    void HandleShooting()
    {
        NormalShooting();
        BurstShooting();
        UpdateshotGunAmmoText();
    }

    void NormalShooting()
    {
        // kiểm tra xem người chơi có nhấn chuột trái,
        // số lượng viên đạn còn lại có lớn hơn 0,
        // thời điểm bắn phải lớn hơn thời điểm được phép bắn

        if (Input.GetMouseButton(0) && shotGunCurrentAmmo > 0 && Time.time > shotGunShotTime && Time.time > shotGunBurstShotTime)
        {
            isShooting = true;
            audioManager.PlayShootSound();
            //shotGunGameObject.SetActive(true);
            //gunGameObject.SetActive(false);
            // tạo viên đạn
            Instantiate(bulletPrefab, shotGunFirePosition1.position, shotGunFirePosition1.rotation);
            Instantiate(bulletPrefab, shotGunFirePosition1.position, shotGunFirePosition2.rotation);
            Instantiate(bulletPrefab, shotGunFirePosition1.position, shotGunFirePosition3.rotation);
            Instantiate(bulletPrefab, shotGunFirePosition1.position, shotGunFirePosition4.rotation);
            Instantiate(bulletPrefab, shotGunFirePosition1.position, shotGunFirePosition5.rotation);

            // Cập nhật thời điểm được phép bắn tiếp theo
            shotGunShotTime = Time.time + shotGunSingleShotDelay;
            // giảm số lượng viên đạn còn lại
            shotGunCurrentAmmo = shotGunCurrentAmmo - 5;
        }
        //if (Input.GetMouseButton(0)) {
        //    gunGameObject.SetActive(true);
        //    shotGunGameObject.SetActive(false);
        //}
    }
    //private bool isBurstShooting = false;
    void BurstShooting()
    {
        if (Time.time > shotGunShotTime)
        {
            if (Input.GetMouseButton(1) && shotGunCurrentAmmo > 0 && Time.time > shotGunBurstShotTime)
            {
                //isShooting = true;
                //isBurstShooting = true;
                StartCoroutine(BurstShoot());
                shotGunBurstShotTime = Time.time + shotGunBurstShotDelay * 2 + shotGunDelayTimeInBetween2Burst;
                //shotGunShotTime = shotGunBurstShotTime;
                //isBurstShooting = false;
            }
        }
        
        
    }
    private IEnumerator BurstShoot()
    {
        int burstCount = Mathf.Min(2, shotGunCurrentAmmo / 5); // số lượng viên đạn bắn ra trong một đợt burst
        for (int i = 0; i < burstCount; i++)
        {
            if (shotGunCurrentAmmo > 0)
            {
                isShooting = true;
                audioManager.PlayShootSound();
                Instantiate(bulletPrefab, shotGunFirePosition1.position, shotGunFirePosition1.rotation);
                Instantiate(bulletPrefab, shotGunFirePosition1.position, shotGunFirePosition2.rotation);
                Instantiate(bulletPrefab, shotGunFirePosition1.position, shotGunFirePosition3.rotation);
                Instantiate(bulletPrefab, shotGunFirePosition1.position, shotGunFirePosition4.rotation);
                Instantiate(bulletPrefab, shotGunFirePosition1.position, shotGunFirePosition5.rotation);
                shotGunCurrentAmmo = shotGunCurrentAmmo - 5;
                UpdateshotGunAmmoText();
                yield return new WaitForSeconds(shotGunBurstShotDelay);
            }
            if (shotGunCurrentAmmo == 0)
            {
                Reload();
            }
        }
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && shotGunCurrentAmmo < shotGunMaxAmmo || shotGunCurrentAmmo == 0)
        {
            isReloading = true;
            audioManager.PlayReLoadSound();
            // cập nhật thời điểm được phép bắn sau khi nạp đạn
            shotGunReloadEndTime = Time.time + shotGunReloadTime;
            shotGunShotTime = shotGunReloadEndTime;
            shotGunBurstShotTime = shotGunReloadEndTime;
            shotGunCurrentAmmo = shotGunMaxAmmo;

        }
        UpdateshotGunAmmoText();
    }

    public bool IsShooting()
    {
        return isShooting;
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    private void UpdateshotGunAmmoText()
    {
        if (shotGunAmmoText != null)
        {
            //if (gameObject.activeSelf != true)
            //{
            //    shotGunAmmoText.gameObject.SetActive(false);
            //    shotGunAmmoText_UI.gameObject.SetActive(false);
            //}
            if (Time.time >= shotGunReloadEndTime)
            {
                shotGunAmmoText.text = shotGunCurrentAmmo.ToString() + "/" + shotGunMaxAmmo.ToString();
                shotGunAmmoText_UI.text = shotGunCurrentAmmo.ToString() + "/" + shotGunMaxAmmo.ToString();
            }
            else
            {
                shotGunAmmoText.text = "Reloading...";
                shotGunAmmoText_UI.text = "Reloading...";
            }
        }
    }
    //public float GetShotGunSingleShotDelay()
    //{
    //    return shotGunSingleShotDelay;
    //}
    //public float GetShotGunDelayTimeInBetween2Burst()
    //{
    //    return shotGunDelayTimeInBetween2Burst;
    //}
}
