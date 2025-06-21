using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HighScoreRowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button deleteButton;

    private int entryIndex;
    private HighScoreUI parentUI;

    public void Setup(string name, int score, int index, HighScoreUI ui)
    {
        nameText.text = $"{index + 1}. {name}";
        scoreText.text = score.ToString();
        entryIndex = index;
        parentUI = ui;

        deleteButton.onClick.AddListener(DeleteThis);
    }

    private void DeleteThis()
    {
        HighScoreManager.Instance.RemoveEntryAt(entryIndex);
        parentUI.Refresh();
    }
}
