using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialogueUI;
    public TMP_Text nameText;
    public TMP_Text sentenceText;

    private Queue<DialogueLine> dialogueLines;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        dialogueLines = new Queue<DialogueLine>();
    }

    public void StartDialogue(DialogueData dialogueData)
    {
        dialogueUI.SetActive(true);
        dialogueLines.Clear();

        foreach (var line in dialogueData.lines)
        {
            dialogueLines.Enqueue(line);
        }

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (dialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueLines.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(line));
    }

    IEnumerator TypeSentence(DialogueLine line)
    {
        nameText.text = line.speakerName;
        sentenceText.text = "";

        foreach (char letter in line.sentence.ToCharArray())
        {
            sentenceText.text += letter;
            yield return new WaitForSecondsRealtime(0.02f); // thời gian gõ mỗi ký tự
        }
    }
    public event System.Action onDialogueEnd; // ← đặt ở đầu class, khai báo sự kiện

    private void EndDialogue()
    {
        dialogueUI.SetActive(false);
        onDialogueEnd?.Invoke(); // ← báo cho các bên khác rằng thoại đã xong
    }

    void Update()
    {
        if (dialogueUI.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextLine();
        }
    }

}
