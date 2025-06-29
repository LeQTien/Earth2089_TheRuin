using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueData dialogueData;

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
    }
}
