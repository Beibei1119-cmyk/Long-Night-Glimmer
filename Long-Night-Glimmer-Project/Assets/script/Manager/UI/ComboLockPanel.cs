using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  // 添加这行

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

    [Header("音效")]
    public AudioClip openSound;         // 打开面板音效
    public AudioClip slotFillSound;     // 凹槽填满音效
    public AudioClip unlockSound;       // 解锁成功音效
    public AudioClip closeSound;        // 关闭面板音效
    public AudioClip buttonClickSound;  // 按钮点击音效

    private int currentLineIndex = 0;
    private AudioSource audioSource;


    private void Start()
    {
        gameObject.SetActive(false);

        // 初始化 AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = 0.8f;
        }
        // ========== 强制启用 AudioSource ==========
        audioSource.playOnAwake = false;
        audioSource.volume = 0.8f;
        audioSource.enabled = true;  // ← 关键：确保启用
        // =========================================

    }

    // 统一播放音效的方法
    //private void PlaySound(AudioClip clip)
    //{
    //    if (clip != null && audioSource != null)
    //    {
    //        audioSource.PlayOneShot(clip);
    //    }
    //}

    public void Open(InteractableObject box)
    {
        //// 播放打开音效
        //PlaySound(openSound);

        targetBox = box;
        LoadAllSlots();
        gameObject.SetActive(true);
    }

    public void OnSlotFilled()
    {
        //// 播放凹槽填满音效
        //PlaySound(slotFillSound);

        foreach (var slot in slots)
        {
            if (!slot.IsFilled()) return;
        }

        Debug.Log($"所有凹槽已满，准备解锁，targetBox={targetBox?.name}");

        //// 播放解锁音效
        //PlaySound(unlockSound);

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
       //// 播放关闭音效
       // PlaySound(closeSound);

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
            endingPanel.SetActive(false);

            // 使用全局的 UIManager 来启动协程（UIManager 不会被销毁）
            UIManager.Instance.StartCoroutine(ReturnToMainMenuCoroutine());
        }
    }

    private IEnumerator ReturnToMainMenuCoroutine()
    {

        // ========== 停止当前场景的音乐 ==========
        AudioManager.Instance?.StopMusic();  // 如果有 AudioManager
        // ====================================
        yield return null;

        string currentScene = SceneManager.GetActiveScene().name;

        yield return SceneManager.UnloadSceneAsync(currentScene);

        SceneManager.LoadScene("MainMenu");

        Debug.Log($"卸载 {currentScene}，返回主菜单");
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