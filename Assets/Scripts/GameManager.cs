using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.Cinemachine;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using UnityEditor.Rendering.LookDev;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int currentEnergy;
    [SerializeField] private int energyThreshHold = 10;
    [SerializeField] private GameObject boss;
    private bool bossCalled = false;
    [SerializeField] private GameObject enemySpawner;

    [SerializeField] private Image energyBar;
    [SerializeField] private GameObject GameUI;
    [SerializeField] private CinemachineCamera cam;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winMenu;

    [SerializeField] private AudioManager audioManager;
    //[SerializeField] private GameObject theScoreUI; 
    [SerializeField] private GameObject theScoreUI; // set active
    //[SerializeField] private ScoreManager scoreManager;

    public int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private float playerCurrentEnergyPoint;
    [SerializeField] private float playerEnergyPointThreshHold = 10;
    [SerializeField] private Image playerEnergyPoint;
    //[SerializeField] private GameObject playerEnergyUI;


    private bool isGameMenuActive;
    private bool isGameOverMenuActive;
    private bool isWinGameMenuActive;
    private bool isPauseGameMenuActive;

    [SerializeField] private Player thePlayer;
    [SerializeField] private float GM_HealValue = 20f;

    [SerializeField] private TextMeshProUGUI EnergyPointAmountText;

    [SerializeField] private float playerCurrentRagePoint;
    [SerializeField] private float playerRagePointThreshHold = 200f;
    [SerializeField] private Image playerRagePoint;
    //[SerializeField] private GameObject playerRageUI;
    [SerializeField] private TextMeshProUGUI playerRagePointAmountText;

    private Coroutine rageDecayCoroutine; // Coroutine để giảm nộ

    [SerializeField] private GameObject rageEffectPrefab;
    //[SerializeField] private Transform rageEffectSpawnPoint;
    private GameObject activeRageEffect; // Lưu hiệu ứng nộ đang chạy
    private bool isRageActive = false;
    private Coroutine rageDrainCoroutine; // Coroutine giảm nộ khi kích hoạt nộ

    [SerializeField] private Image playerSmallRagePointBar;
    [SerializeField] private Image playerSmallEnergyPointBar;

    [SerializeField] private float ragePointRequiredToUseRageMode = 50f;
    [SerializeField] private float rageModeMaxDuration = 10f;
    [SerializeField] private float ragePointDrainRate = 10f;
    //[SerializeField] private float rageModeCooldown = 10f;

    [SerializeField] private float rageCooldown = 10f; // 10s hồi chiêu
    private float rageCooldownTimer = 0f;
    private Coroutine rageEffectCoroutine; // Thêm biến để quản lý coroutine
    private bool isForceStopped = false;

    [SerializeField] private Image playerRageModeIcon;
    [SerializeField] private TextMeshProUGUI playerRageModeCoolDownText;
    [SerializeField] private GameObject skillsUI;
    [SerializeField] private GameObject HotBarUI;

    // ---
    private int playerGold = 0;
    public static event Action<int> OnGoldChanged; // Sự kiện thông báo khi gold thay đổi

    private MultiZoneGenerator2 multiZoneGenerator2;
    private GameObject bossAI2;

    [SerializeField] private GameUI gameUI_BestScore;
    private void Start()
    {
        //thePlayer = FindAnyObjectByType<Player>(); // tìm player trong scene để theo dõi vị trí của player 
        currentEnergy = 0;
        UpdateEnergyBar();
        playerCurrentEnergyPoint = 0;
        UpdatePlayerEnergyPointUI();
        UpdateEnergyPointText();
        //UpdatePlayerHPUI();
        playerCurrentRagePoint = 0;
        UpdateRagePointUI();
        UpdatePlayerRagePointText();


        boss.SetActive(false);

        cam.Lens.OrthographicSize = 10f;
        isGameMenuActive = false;
        isGameOverMenuActive = false;
        isWinGameMenuActive = false;
        isPauseGameMenuActive = false;

        MainMenu();
        //rageEffectSpwanPoint = thePlayer.transform.position;
        audioManager.StopAudioGame();
    }

    private void Update()
    {
        //if (isGameMenuActive) // Nếu đang ở Main Menu
        //{
        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        StartGame();
        //    }
        //    if (Input.GetKeyDown(KeyCode.Escape))
        //    {
        //        Application.Quit();
        //    }
        //}
        HandleActivateRageMode();
        HandleRageCooldown(); // Giảm cooldown mỗi frame
        // 🔁 Cập nhật UI mỗi frame
        if (isGameMenuActive == true || isGameOverMenuActive == true || isWinGameMenuActive == true)
        {
            skillsUI.SetActive(false); // Deactivate skills UI
            HotBarUI.SetActive(false); // Deactivate hotbar UI
        }
        else
        {
            skillsUI.SetActive(true); // Activate skills UI
            HotBarUI.SetActive(true); // Activate hotbar UI
            UpdateRageModeCoolDownUI(); // Cập nhật UI hồi chiêu nộ
        }

    }

    public bool IsGameMenuActive()
    {
        return isGameMenuActive;
    }

    public bool IsGameOverMenuActive()
    {
        return isGameOverMenuActive;
    }

    public bool IsWinGameMenuActive()
    {
        return isWinGameMenuActive;
    }
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }


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
    public void AddPlayerEnergyPoint()
    {
        if (playerCurrentEnergyPoint < playerEnergyPointThreshHold)
        {
            playerCurrentEnergyPoint += 1;
            UpdatePlayerEnergyPointUI();
            UpdateEnergyPointText();
        }
    }
    public void UpdateEnergyPointText()
    {
        if (EnergyPointAmountText != null)
        {
            EnergyPointAmountText.text = playerCurrentEnergyPoint + "/" + playerEnergyPointThreshHold;
        }
    }
    public float ReturnPlayerEnergyPointThreshold()
    {
        return playerEnergyPointThreshHold;
    }
    public float ReturnPlayerCurrentEnergyPoint()
    {
        return playerCurrentEnergyPoint;
    }

    public void AddRagePoint(float amount)
    {
        if (isRageActive) return; // Ngăn cộng nộ & dừng timer khi kích hoạt nộ

        playerCurrentRagePoint = Mathf.Clamp(playerCurrentRagePoint + amount, 0, playerRagePointThreshHold);
        UpdateRagePointUI();
        UpdatePlayerRagePointText();

        // Khi gây sát thương, chỉ reset timer nếu nộ chưa kích hoạt
        if (rageDecayCoroutine != null)
        {
            StopCoroutine(rageDecayCoroutine);
        }

        if (!isRageActive) // Chỉ chạy timer nếu nộ không kích hoạt
        {
            rageDecayCoroutine = StartCoroutine(RageDecayTimer());
        }
    }

    private IEnumerator RageDecayTimer()
    {
        yield return new WaitForSeconds(5f); // Chờ 5 giây  

        while (playerCurrentRagePoint > 0)
        {
            playerCurrentRagePoint = Mathf.Clamp(playerCurrentRagePoint - 10, 0, playerRagePointThreshHold);
            UpdateRagePointUI();
            UpdatePlayerRagePointText();

            // Nếu nộ đã về 0, dừng coroutine
            if (playerCurrentRagePoint == 0)
            {
                rageDecayCoroutine = null;
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }

        rageDecayCoroutine = null;
    }

    private void UpdateRagePointUI()
    {
        if (playerRagePoint != null)
        {
            float fillAmount = (float)playerCurrentRagePoint / (float)playerRagePointThreshHold;
            playerRagePoint.fillAmount = fillAmount;
        }
        if (playerSmallRagePointBar != null)
        {
            float fillAmount = (float)playerCurrentRagePoint / (float)playerRagePointThreshHold;
            playerSmallRagePointBar.fillAmount = fillAmount;
        }
    }
    private void UpdatePlayerRagePointText()
    {
        if (playerRagePointAmountText != null)
        {
            playerRagePointAmountText.text = playerCurrentRagePoint + "/" + playerRagePointThreshHold;
        }
    }

    public void HealPlayerByHealPickup()
    {
        if (thePlayer != null)
        {
            thePlayer.Heal(GM_HealValue);
        }
    }

    private void CallBoss()
    {
        bossCalled = true;
        boss.SetActive(true);
        BossAIEnemy bossAIEnemy = boss.GetComponent<BossAIEnemy>();

        if (bossAIEnemy != null)
        {
            StartCoroutine(TeleportBoss(bossAIEnemy));
        }
        GameUI.SetActive(false);

        cam.Lens.OrthographicSize = 12f;
        audioManager.PlayBossAudio();
    }

    private IEnumerator TeleportBoss(BossAIEnemy bossAIEnemy)
    {
        yield return new WaitForSeconds(0.1f); // Small delay to ensure the boss is fully activated
        bossAIEnemy.TeleportingAtStarting();
    }

    private void UpdateEnergyBar()
    {
        if (energyBar != null)
        {
            float fillAmount = (float)currentEnergy / (float)energyThreshHold;
            energyBar.fillAmount = fillAmount;
        }
    }
    private void UpdatePlayerEnergyPointUI()
    {
        if (energyBar != null)
        {
            float fillAmount = (float)playerCurrentEnergyPoint / (float)playerEnergyPointThreshHold;
            playerEnergyPoint.fillAmount = fillAmount;
        }
        if (playerSmallEnergyPointBar != null)
        {
            float fillAmount = (float)playerCurrentEnergyPoint / (float)playerEnergyPointThreshHold;
            playerSmallEnergyPointBar.fillAmount = fillAmount;
        }
    }
    public void UpdatePlayerEnergyPoint(float newEnergy)
    {
        // Đảm bảo năng lượng không vượt quá giới hạn (playerEnergyPointThreshHold) hoặc xuống dưới 0.
        playerCurrentEnergyPoint = Mathf.Clamp(newEnergy, 0, playerEnergyPointThreshHold);
        UpdatePlayerEnergyPointUI(); // Cập nhật thanh năng lượng UI
    }



    // có 6 màn hình giao diện: main menu, game over, pause, start, resume, win
    public void MainMenu()
    {
        //isGameMenuActive = true;
        isGameOverMenuActive = false;
        isWinGameMenuActive = false;
        isPauseGameMenuActive = false;

        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
        winMenu.SetActive(false);
        theScoreUI.SetActive(true); // Deactivate score UI
        HotBarUI.SetActive(true); // Deactivate hotbar UI

        Time.timeScale = 1f;
        audioManager.StopAudioGame();
    }

    public void GameOverMenu()
    {
        isGameOverMenuActive = true;
        isGameMenuActive = false;
        isWinGameMenuActive = false;
        isPauseGameMenuActive = false;

        gameOverMenu.SetActive(true);
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        winMenu.SetActive(false);
        skillsUI.SetActive(false); // Deactivate skills UI

        HotBarUI.SetActive(false);
        theScoreUI.SetActive(false);
        Time.timeScale = 0f;
    }
    //bool isPauseGameMenuActive = false;
    public void PauseGameMenu()
    {
        isPauseGameMenuActive = true; // Đánh dấu trạng thái Pause
        pauseMenu.SetActive(true);
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winMenu.SetActive(false);

        HotBarUI.SetActive(true);
        theScoreUI.SetActive(true);
        skillsUI.SetActive(true); 

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPauseGameMenuActive = false; // Đánh dấu trạng thái Resume
        pauseMenu.SetActive(false);
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winMenu.SetActive(false);

        HotBarUI.SetActive(true);
        theScoreUI.SetActive(true);
        skillsUI.SetActive(true); // Deactivate skills UI
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        isGameMenuActive = false;
        isGameOverMenuActive = false;
        isWinGameMenuActive = false;
        isPauseGameMenuActive = false;

        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winMenu.SetActive(false);
        theScoreUI.SetActive(true); // Activate score UI
        skillsUI.SetActive(true); // Activate skills UI

        HotBarUI.SetActive(true);
        //Time.timeScale = 1f; // Ensure the game time is running
        audioManager.PlayDefaultAudio();
    }

    public void WinGame()
    {
        isGameMenuActive = false;
        isGameOverMenuActive = false;
        isWinGameMenuActive = true;
        isPauseGameMenuActive = false;

        winMenu.SetActive(true);
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        theScoreUI.SetActive(false); // Deactivate score UI
        skillsUI.SetActive(false); // Deactivate skills UI

        HotBarUI.SetActive(false);

        //winMenu.GetComponentInChildren<TextMeshProUGUI>().text = "You Win! Your Score: " + score;
        //Time.timeScale = 0f;

        gameUI_BestScore.ShowWinScreen();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    public void ActivateRage()
    {
        if (playerCurrentRagePoint < ragePointRequiredToUseRageMode || isRageActive) return;

        isRageActive = true;
        isForceStopped = false; // Reset cờ

        if (rageDecayCoroutine != null)
        {
            StopCoroutine(rageDecayCoroutine);
            rageDecayCoroutine = null;
        }

        activeRageEffect = Instantiate(rageEffectPrefab, thePlayer.transform);
        activeRageEffect.transform.localPosition = Vector3.zero;

        rageEffectCoroutine = StartCoroutine(RageEffectDuration());
    }

    private IEnumerator RageEffectDuration()
    {
        //float duration = 5f; // Giới hạn 5 giây
        float elapsedTime = 0f;

        while (elapsedTime < rageModeMaxDuration)
        {
            if (isForceStopped)
            {
                yield break;
            }

            // ✅ Chỉ giảm nộ nếu KHÔNG bị tắt thủ công
            if (!isForceStopped && playerCurrentRagePoint > 0)
            {
                playerCurrentRagePoint = Mathf.Clamp(playerCurrentRagePoint - ragePointDrainRate, 0, playerRagePointThreshHold);
                UpdateRagePointUI();
                UpdatePlayerRagePointText();

                if (playerCurrentRagePoint == 0)
                {
                    ForceStopRage();
                    yield break;
                }
            }

            elapsedTime += 1f;
            yield return new WaitForSeconds(1f);
        }

        ForceStopRage();
    }
    public void DeactivateRage()
    {
        isRageActive = false;
        rageCooldownTimer = rageCooldown; // Bắt đầu hồi chiêu

        if (activeRageEffect != null)
        {
            Destroy(activeRageEffect);
            activeRageEffect = null;
        }

        if (rageDrainCoroutine != null)
        {
            StopCoroutine(rageDrainCoroutine);
            rageDrainCoroutine = null;
        }

        // Bắt đầu lại đếm ngược nộ sau khi hết Rage
        if (playerCurrentRagePoint > 0)
        {
            rageDecayCoroutine = StartCoroutine(RageDecayTimer());
        }
    }
    public void ForceStopRage()
    {
        if (!isRageActive) return;

        isRageActive = false;
        rageCooldownTimer = rageCooldown; // Bắt đầu hồi chiêu

        if (activeRageEffect != null)
        {
            Destroy(activeRageEffect);
            activeRageEffect = null;
        }

        if (rageDrainCoroutine != null)
        {
            StopCoroutine(rageDrainCoroutine);
            rageDrainCoroutine = null;
        }

        if (rageEffectCoroutine != null)
        {
            StopCoroutine(rageEffectCoroutine);
            rageEffectCoroutine = null;
        }

        if (playerCurrentRagePoint > 0)
        {
            rageDecayCoroutine = StartCoroutine(RageDecayTimer());
        }

        Debug.Log("Rage Mode đã bị tắt và bắt đầu hồi chiêu.");
    }
    private void HandleActivateRageMode()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isRageActive)
            {
                // Chỉ cho tắt khi thật sự đã vào Rage Mode
                isForceStopped = true;
                ForceStopRage();
                return;
            }

            if (rageCooldownTimer > 0f)
            {
                Debug.Log("Rage Mode đang hồi chiêu. Còn lại: " + rageCooldownTimer.ToString("F1") + "s");
                return;
            }

            ActivateRage();
        }
    }
    private void HandleRageCooldown()
    {
        if (rageCooldownTimer > 0f)
        {
            rageCooldownTimer -= Time.deltaTime; // Giảm cooldown
            if (rageCooldownTimer < 0f)
            {
                rageCooldownTimer = 0f; // Đảm bảo không nhỏ hơn 0
            }
        }
    }

    // Hàm IsRageActive() giúp Player biết nộ có đang hoạt động không.
    public bool IsRageActive()
    {
        return isRageActive;
    }

    private void UpdateRageModeCoolDownUI()
    {
        if (playerRageModeCoolDownText != null && playerRageModeIcon != null)
        {
            if (rageCooldownTimer > 0f)
            {
                playerRageModeCoolDownText.text = "Q - " + rageCooldownTimer.ToString("F1") + "s";

                // Đảm bảo icon không vượt quá 1 hoặc nhỏ hơn 0
                float fill = Mathf.Clamp01(1f - (rageCooldownTimer / rageCooldown));
                playerRageModeIcon.fillAmount = fill;

                // Làm tối icon (xám)
                playerRageModeIcon.color = new Color(0.4f, 0.4f, 0.4f, 1f);
            }
            else if (isRageActive)
            {
                playerRageModeCoolDownText.text = "Q - " + "Active!";
                playerRageModeIcon.color = new Color(0.4f, 0.4f, 0.4f, 1f);
            }
            else
            {
                // Kiểm tra nếu không đủ rage point
                if (playerCurrentRagePoint < ragePointRequiredToUseRageMode)
                {
                    playerRageModeCoolDownText.text = "Q - " + "Not enough rage!";
                    playerRageModeIcon.fillAmount = 1f;
                    playerRageModeIcon.color = new Color(0.4f, 0.4f, 0.4f, 1f);
                }
                else
                {
                    playerRageModeCoolDownText.text = "Q - " + "Ready!";
                    playerRageModeIcon.fillAmount = 1f;
                    playerRageModeIcon.color = Color.white;
                }
            }
        }
    }

    public int GetPlayerGold()
    {
        return playerGold;
    }

    public void AddGold(int amount)
    {
        // Mỗi lần Player nhặt Gold, GameManager sẽ cộng gold → gọi sự kiện OnGoldChanged → GameUI nhận và cập nhật GoldText.
        playerGold += amount; // Cộng vàng vào playerGold

        OnGoldChanged?.Invoke(playerGold); // Gọi sự kiện OnGoldChanged và truyền playerGold hiện tại
        Debug.Log("Gold: " + playerGold); // Ghi log số vàng hiện tại
    }

    //public void EnergyBarScene2()
    //{
    //    MultiZoneGenerator2 multiZoneGenerator2 = FindObjectOfType<MultiZoneGenerator2>();
    //    bossAI2 = GameObject.Find("BossAI2");
    //    if (multiZoneGenerator2 != null && multiZoneGenerator2.IsMapGenerated)
    //    {
    //        currentEnergy = 0;
    //        bossAI2.SetActive(false);

    //        cam.Lens.OrthographicSize = 10f;
    //        GameUI.SetActive(true);
    //    }

    //}

    //public void CallBossBarScene2()
    //{
    //    MultiZoneGenerator2 multiZoneGenerator2 = FindObjectOfType<MultiZoneGenerator2>();
    //    if (multiZoneGenerator2 != null && multiZoneGenerator2.IsMapGenerated)
    //    {
    //        boss.SetActive(true);
    //        BossAIEnemy bossAIEnemy = boss.GetComponent<BossAIEnemy>();
    //        if (bossAIEnemy != null)
    //        {
    //            StartCoroutine(TeleportBoss(bossAIEnemy));
    //        }
    //        GameUI.SetActive(false);
    //        cam.Lens.OrthographicSize = 12f;
    //        audioManager.PlayBossAudio();
    //    }
    //}
}
