using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public string speakerName;   // 说话者名字
    public string content;       // 对话内容
    public bool isLeft;          // true=左边人物说话，false=右边人物说话
}