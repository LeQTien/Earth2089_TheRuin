using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text bestScoreText;

    [SerializeField] private GameObject saveScorePopup;     // Panel chứa input + nút Save
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private GameObject toastText;

    [Header("Main‑menu panels")]
    [SerializeField] private GameObject highScorePanel;

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
        if (gameManager != null)
            gameManager.StartGame();
        else
            Debug.LogWarning("GameManager not assigned in GameUI.");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {
        if (gameManager != null)
            gameManager.ResumeGame();
        else
            Debug.LogWarning("GameManager not assigned in GameUI.");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowSavePopup()
    {
        if (saveScorePopup != null)
            saveScorePopup.SetActive(true);
        else
            Debug.LogWarning("Save Score Popup not assigned.");
    }

    public void CloseSavePopup()
    {
        if (saveScorePopup != null)
            saveScorePopup.SetActive(false);
    }

    public void SaveHighScore()
    {
        string player = string.IsNullOrWhiteSpace(playerNameInput?.text)
                        ? "Player" : playerNameInput.text.Trim();

        int score = 0;

        if (gameManager != null)
        {
            score = gameManager.score; // hoặc gameManager.GetScore()
        }
        else
        {
            Debug.LogWarning("GameManager is not assigned!");
        }

        if (HighScoreManager.Instance != null)
        {
            HighScoreManager.Instance.AddScore(player, score);
        }
        else
        {
            Debug.LogError("HighScoreManager.Instance is null! Make sure it exists in the scene.");
        }

        StartCoroutine(CloseAfterToast());
        //CloseSavePopup();
        //toastText.SetActive(true);
    }

    private IEnumerator CloseAfterToast()
    {
        CloseSavePopup();
        if (toastText != null)
        {
            Debug.Log("Toast bắt đầu hiển thị");
            toastText.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            Debug.Log("ToastText đã được tắt");
            toastText.SetActive(false);
        }
    }


    // HIGH SCORE
    public void ShowHighScorePanel()
    {
        if (highScorePanel != null)
            highScorePanel.SetActive(true);
    }

    public void HideHighScorePanel()
    {
        if (highScorePanel != null)
            highScorePanel.SetActive(false);
    }
    public void ShowWinScreen()
    {
        int currentScore = gameManager.score;  // hoặc gameManager.GetScore()
        bestScoreText.text = $"Your Best Score: {currentScore}";

        // Hiện panel win game ở đây nếu có
        // winPanel.SetActive(true);
    }
}
