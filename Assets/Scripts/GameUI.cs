using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class GameUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
        gameManager.StartGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {
        gameManager.ResumeGame();
    }

    public void MainMenu()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //SceneManager.LoadScene("SampleScene");
        SceneManager.LoadScene("MainMenu");
    }
}
