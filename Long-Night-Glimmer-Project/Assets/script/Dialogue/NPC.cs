using UnityEngine;
using System.Collections.Generic;

public class NPC : MonoBehaviour
{
    [Header("对话内容")]
    public List<DialogueData> dialogueList;


    void Start()
    {
        if (NPCDialogueManager.Instance == null)
        {
            Debug.LogError("NPCDialogueManager.Instance 为空！请确保场景中有 NPCDialogueManager 物体");
        }
    }
    private void OnMouseDown()
    {
        if (NPCDialogueManager.Instance == null)
        {
            Debug.LogError("NPCDialogueManager 不存在！");
            return;
        }

        if (NPCDialogueManager.Instance.IsActive()) return;

        if (dialogueList != null && dialogueList.Count > 0)
        {
            NPCDialogueManager.Instance.StartDialogue(dialogueList);
        }
    }
}