using UnityEngine;
using System.Collections.Generic;

public class NPC : MonoBehaviour
{
    [Header("对话内容")]
    public List<DialogueData> dialogueList;

    private void OnMouseDown()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager 不存在！");
            return;
        }

        if (DialogueManager.Instance.IsActive()) return;

        if (dialogueList != null && dialogueList.Count > 0)
        {
            DialogueManager.Instance.StartDialogue(dialogueList);
        }
    }
}