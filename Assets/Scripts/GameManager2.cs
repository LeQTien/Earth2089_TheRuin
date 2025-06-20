using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class GameManager2 : MonoBehaviour
{
    private int currentEnergy;
    [SerializeField] private int energyThreshHold = 10;
    [SerializeField] private GameObject boss;
    private bool bossCalled = false;
    private AudioManager audioManager; // Assuming you have an AudioManager script to handle audio
    [SerializeField] private Image energyBar;
    [SerializeField] private GameObject GameUI;
    private CinemachineCamera cam;

    private void Start()
    {
        currentEnergy = 0;
        UpdateEnergyBar();

        boss.SetActive(false);

        cam = FindObjectOfType<CinemachineCamera>();
        cam.Lens.OrthographicSize = 10f;

        audioManager = FindObjectOfType<AudioManager>();
        
    }

    //public void Update()
    //{
        
    //}

    public void AddEnergy() // energy to call boss
    {
        if (bossCalled) return;
        currentEnergy += 1;
        UpdateEnergyBar();
        if (currentEnergy >= energyThreshHold && !bossCalled)
        {
            CallBoss();
        }
    }
    private void UpdateEnergyBar()
    {
        if (energyBar != null)
        {
            float fillAmount = (float)currentEnergy / (float)energyThreshHold;
            energyBar.fillAmount = fillAmount;
        }
    }
    private void CallBoss()
    {
        bossCalled = true;
        boss.SetActive(true);
        BossAIEnemy2 bossAIEnemy2 = boss.GetComponent<BossAIEnemy2>();

        if (bossAIEnemy2 != null)
        {
            StartCoroutine(TeleportBoss(bossAIEnemy2));
        }
        GameUI.SetActive(false);

        cam.Lens.OrthographicSize = 12f;
        audioManager.PlayBossAudio();
    }

    private IEnumerator TeleportBoss(BossAIEnemy2 bossAIEnemy2)
    {
        yield return new WaitForSeconds(0.1f); // Small delay to ensure the boss is fully activated
        bossAIEnemy2.TeleportingAtStarting();
    }

}
