using System.Collections.Generic;
using UnityEngine;

public class ElectroBeam : MonoBehaviour
{
    [SerializeField] private GameObject bloodPrefabs;
    [SerializeField] private float enterDamage = 100f;
    [SerializeField] private float stayDamage = 10f;
    [SerializeField] private float stayDamageInterval = 0.5f;

    private GameManager gameManager;
    //private AbstractPlayerBulletClass playerBullet;

    private Dictionary<Enemy, float> enemyLastHitTime = new Dictionary<Enemy, float>();

    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        //playerBullet = FindAnyObjectByType<AbstractPlayerBulletClass>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(enterDamage);
                GameObject blood = Instantiate(bloodPrefabs, transform.position, Quaternion.identity);
                Destroy(blood, 1f);
                
                gameManager.AddRagePoint(1);
                enemyLastHitTime[enemy] = Time.time; // bắt đầu thời gian gây sát thương stay
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (!enemyLastHitTime.ContainsKey(enemy))
                {
                    enemyLastHitTime[enemy] = Time.time;
                }

                if (Time.time - enemyLastHitTime[enemy] >= stayDamageInterval)
                {
                    enemy.TakeDamage(stayDamage);
                    enemyLastHitTime[enemy] = Time.time;

                    GameObject blood = Instantiate(bloodPrefabs, transform.position, Quaternion.identity);
                    Destroy(blood, 1f);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null && enemyLastHitTime.ContainsKey(enemy))
        {
            enemyLastHitTime.Remove(enemy);
        }
    }
}
