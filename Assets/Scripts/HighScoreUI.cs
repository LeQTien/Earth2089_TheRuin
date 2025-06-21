using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreUI : MonoBehaviour
{
    [SerializeField] private Transform contentRoot; // nơi sẽ nhét các Row
    [SerializeField] private GameObject rowPrefab;

    private void Start()
    {
        Populate();
        Refresh();
    }

    private void Populate()
    {
        IReadOnlyList<HighScoreEntry> scores = HighScoreManager.Instance.GetHighScores();

        for (int i = 0; i < scores.Count; i++)
        {
            HighScoreEntry e = scores[i];
            GameObject row = Instantiate(rowPrefab, contentRoot);

            // Giả sử rowPrefab có 2 TMP_Text con
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
            texts[0].text = $"{i + 1}. {e.playerName}";
            texts[1].text = e.score.ToString();
        }
    }
    public void Refresh()
    {
        // Xóa các dòng cũ
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }

        IReadOnlyList<HighScoreEntry> scores = HighScoreManager.Instance.GetHighScores();

        for (int i = 0; i < scores.Count; i++)
        {
            var entry = scores[i];
            var rowObj = Instantiate(rowPrefab, contentRoot);
            var rowUI = rowObj.GetComponent<HighScoreRowUI>();

            rowUI.Setup(entry.playerName, entry.score, i, this);
        }
    }
}