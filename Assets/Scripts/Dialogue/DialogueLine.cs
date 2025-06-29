using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(2, 5)]
    public string sentence;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueAsset")]
public class DialogueData : ScriptableObject
{
    public List<DialogueLine> lines = new List<DialogueLine>();
}