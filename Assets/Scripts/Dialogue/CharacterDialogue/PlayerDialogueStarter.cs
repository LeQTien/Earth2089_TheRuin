using System.Collections;
using UnityEngine;

public class PlayerDialogueStarter : MonoBehaviour
{
    public DialogueTrigger introDialogue;
    public bool isIntroFinished = false;

    void Start()
    {
        StartCoroutine(PlayIntroThenStartGame());
    }

    IEnumerator PlayIntroThenStartGame()
    {
        // 🔹 Đợi 1.5 giây trước khi bắt đầu thoại
        yield return new WaitForSecondsRealtime(1.5f);

        // 🔹 Tạm dừng game để ngăn gameplay hoạt động
        //Time.timeScale = 0f;

        // 🔹 Gắn callback khi thoại kết thúc
        DialogueManager.Instance.onDialogueEnd += OnIntroFinished;

        // 🔹 Bắt đầu thoại
        introDialogue.TriggerDialogue();

        // 🔹 Đợi đến khi thoại kết thúc
        while (!GameManager.Instance.gameStarted)
        {
            yield return null;
        }

        // ✅ Game sẽ tiếp tục sau khi onDialogueEnd gọi OnIntroFinished()
    }

    void OnIntroFinished()
    {
        DialogueManager.Instance.onDialogueEnd -= OnIntroFinished;
        StartCoroutine(DelayThenStartGameplay());
    }

    IEnumerator DelayThenStartGameplay()
    {
        // 🔹 Đợi 0.5 giây sau khi thoại kết thúc
        yield return new WaitForSecondsRealtime(0.5f);

        // 🔹 Cho phép game bắt đầu
        GameManager.Instance.gameStarted = true;
        isIntroFinished = true;
        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner != null)
        {
            enemySpawner.StartCoroutine(enemySpawner.SpawnEnemies());
        }
        //Time.timeScale = 1f;
    }
}
