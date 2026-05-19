using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ComboLockPanel : MonoBehaviour
{
    [Header("关联箱子")]
    public InteractableObject targetBox;

    [Header("凹槽列表")]
    public ComboLockSlot[] slots;

    [Header("结束界面")]
    public bool showEndingOnComplete = false;
    public GameObject endingPanel;
    public Text endingText;
    public Button nextButton;           // 下一句按钮
    public List<string> endingMessages;  // 多条结束文字

    private int currentLineIndex = 0;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Open(InteractableObject box)
    {
        targetBox = box;
        LoadAllSlots();
        gameObject.SetActive(true);
    }

    public void OnSlotFilled()
    {
        foreach (var slot in slots)
        {
            if (!slot.IsFilled()) return;
        }

        Debug.Log($"所有凹槽已满，准备解锁，targetBox={targetBox?.name}");
        if (targetBox != null)
        {
            Debug.Log("调用 targetBox.Unlock()");
            targetBox.Unlock();
        }

        if (showEndingOnComplete)
        {
            ShowEnding();
        }

        gameObject.SetActive(false);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void ShowEnding()
    {
        if (endingPanel != null)
        {
            currentLineIndex = 0;
            endingPanel.SetActive(true);
            UpdateEndingText();

            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(NextEndingLine);
            }
        }
    }

    private void UpdateEndingText()
    {
        if (endingText != null && endingMessages.Count > 0)
        {
            endingText.text = endingMessages[currentLineIndex];
        }
    }

    private void NextEndingLine()
    {
        currentLineIndex++;

        if (currentLineIndex < endingMessages.Count)
        {
            UpdateEndingText();
        }
        else
        {
            // 所有文字显示完毕，关闭面板
            endingPanel.SetActive(false);
            // 可选：返回主菜单
            // SceneManager.LoadScene("MainMenu");
        }
    }

    private void LoadAllSlots()
    {
        if (targetBox == null) return;

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string boxName = targetBox.name;

        foreach (var slot in slots)
        {
            if (slot == null) continue;

            string key = $"{boxName}_{slot.slotId}";
            bool isFilled = SceneStateManager.Instance.GetComboSlotState(sceneName, key);
            slot.LoadState(isFilled);
        }
    }
}