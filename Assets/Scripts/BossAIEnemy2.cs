using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class BossAIEnemy2 : Enemy
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [SerializeField] private float normalBulletSpeed = 30f;
    [SerializeField] private float inCircleBulletSpeed = 10f;

    [SerializeField] private float hpValue = 100f;
    [SerializeField] private GameObject miniEnemy;

    [SerializeField] private float skillCooldown = 1f;
    private float nextSkillTimer = 0f;

    [SerializeField] private GameObject usbPrefab;

    [SerializeField] private GameObject enemySpawner;
    //[SerializeField] private GameObject portalPrefab;
    private MultiZoneGenerator2 multiZoneGenerator2;

    protected override void Update()
    {
        base.Update();
        if (Time.time >= nextSkillTimer)
        {
            UseSkill();
        }
        
    }

    protected override void Die()
    {
        base.Die();
        enemySpawner.SetActive(false);
        if (usbPrefab != null)
        {
            Instantiate(usbPrefab, transform.position, Quaternion.identity);
        }
        //if (portalPrefab != null)
        //{
        //    Instantiate(portalPrefab, transform.position, Quaternion.identity);
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player != null)
            {
                player.TakeDamage(enterDamage + 10);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player != null)
            {
                player.TakeDamage(stayDamage);
            }
        }
    }

    private void NormalShooting()
    {
        if (player != null)
        {
            Vector3 directionToPlayer = player.transform.position - firePoint.position;
            directionToPlayer.Normalize();
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            EnemyBullet enemyBullet = bullet.AddComponent<EnemyBullet>();
            enemyBullet.SetMovementDirection(directionToPlayer * normalBulletSpeed);
        }
    }

    private void InCircleShooting()
    {
        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.Normalize();
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        const int bulletCount = 12;
        float angleStep = 360f / bulletCount;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = baseAngle + i * angleStep;
            Vector3 bulletDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            EnemyBullet enemyBullet = bullet.AddComponent<EnemyBullet>();
            enemyBullet.SetMovementDirection(bulletDirection * inCircleBulletSpeed);
        }
    }

    private void BossHealing(float hpAmount)
    {
        currentHP = Mathf.Min(currentHP + hpAmount, maxHP);
        UpdateHPBar();
    }

    private void summonMiniEnemy()
    {
        Instantiate(miniEnemy, transform.position, Quaternion.identity);
    }
    
    private void Teleporting()
    {
        if (player != null)
        {
            // transform.position = player.transform.position;
            Vector3 playerPos = player.transform.position;
            Vector3 newBossPos;
            if (player.GetPlayerFaceDirection()) // có nhìn sang trái không
            {
                newBossPos = new Vector3(playerPos.x + 5f, playerPos.y, playerPos.z);
            }
            else
            {
                newBossPos = new Vector3(playerPos.x - 5f, playerPos.y, playerPos.z);
            }

            // Gán vị trí mới cho boss
            transform.position = newBossPos;
        }
    }

    private void ChooseRandomSkill()
    {
        int randomSkill = Random.Range(0, 16);
        switch (randomSkill)
        {
            case 0:
                NormalShooting();
                break;
            case 1:
                NormalShooting();
                break;
            case 2:
                NormalShooting();
                break;
            case 3:
                NormalShooting();
                break;
            case 4:
                InCircleShooting();
                break;
            case 5:
                InCircleShooting();
                break;
            case 6:
                InCircleShooting();
                break;
            case 7:
                BossHealing(hpValue);
                break;
            case 8:
                BossHealing(hpValue);
                break;
            case 9:
                summonMiniEnemy();
                break;
            case 10:
                summonMiniEnemy();
                break;
            case 11:
                Teleporting();
                break;
            case 12:
                Teleporting();
                break;
            case 13:
                Teleporting();
                break;
            case 14:
                InCircleShooting();
                break;
            case 15:
                InCircleShooting();
                break;
        }
    }

    private void UseSkill()
    {
        nextSkillTimer = Time.time + skillCooldown;
        ChooseRandomSkill();
    }

    public void TeleportingAtStarting()
    {
        if (player != null)
        {
            // transform.position = player.transform.position;
            Vector3 playerPos = player.transform.position;
            Vector3 newBossPos;
            if (player.GetPlayerFaceDirection()) // có nhìn sang trái không
            {
                newBossPos = new Vector3(playerPos.x + 15f, playerPos.y, playerPos.z);
            }
            else
            {
                newBossPos = new Vector3(playerPos.x - 15f, playerPos.y, playerPos.z);
            }
            // Gán vị trí mới cho boss
            transform.position = newBossPos;
            Debug.Log("Boss teleported to: " + newBossPos);
        }
        else
        {
            Debug.LogWarning("Player not found for teleporting boss at starting.");
        }
    }
}