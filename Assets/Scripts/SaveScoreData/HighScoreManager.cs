using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance { get; private set; }

    private const int MaxEntries = 5;
    private string _filePath;

    private HighScoreData _data = new HighScoreData();

    #region Unity lifecycle
    private void Awake()
    {
        // Singleton an toàn
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // GÁN ĐÚNG Ở ĐÂY
        _filePath = Path.Combine(Application.persistentDataPath, "highscores.json");

        LoadScores();
        Debug.Log("High Score file path: " + Application.persistentDataPath);
    }

    private void OnApplicationQuit() => SaveScores();
    #endregion

    #region API công khai
    public IReadOnlyList<HighScoreEntry> GetHighScores() => _data.entries;

    public void AddScore(string name, int score)
    {
        // 1. Thêm
        _data.entries.Add(new HighScoreEntry { playerName = name, score = score });

        // 2. Sắp xếp giảm dần
        _data.entries.Sort((a, b) => b.score.CompareTo(a.score));

        // 3. Giới hạn 10
        if (_data.entries.Count >= MaxEntries)
            _data.entries.RemoveRange(MaxEntries, _data.entries.Count - MaxEntries);

        // 4. Ghi xuống đĩa
        SaveScores();
    }
    #endregion

    #region Lưu / Nạp
    private void SaveScores()
    {
        var json = JsonUtility.ToJson(_data, true);
        File.WriteAllText(_filePath, json);
    }

    private void LoadScores()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            _data = JsonUtility.FromJson<HighScoreData>(json) ?? new HighScoreData();
        }
    }
    #endregion
    public void RemoveEntryAt(int index)
    {
        if (index >= 0 && index < _data.entries.Count)
        {
            _data.entries.RemoveAt(index);
            SaveScores();
        }
    }
}