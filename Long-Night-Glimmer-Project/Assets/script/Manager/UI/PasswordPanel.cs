using UnityEngine;
using UnityEngine.UI;

public class PasswordPanel : MonoBehaviour
{
    [Header("布局")]
    public GameObject digitalLayout;

    [Header("UI组件")]
    public Text passwordDisplay;

    [Header("音效")]
    public AudioClip numberClickSound;     // 数字按钮点击音效
    public AudioClip deleteSound;          // 删除按钮音效
    public AudioClip confirmSound;         // 确认按钮音效
    public AudioClip closeSound;           // 关闭按钮音效
    public AudioClip errorSound;           // 密码错误音效
    public AudioClip successSound;         // 解锁成功音效


    private string currentInput = "";        
    private int maxLength = 4;                     
    private InteractableObject currentTarget;
    private AudioSource audioSource;  // ← 添加这一行

    void Start()
    {
        //有这个代码则不需要手动关闭那个ui，没有的话则需要手动关闭哈。
        //Debug.Log($"PasswordPanel 启动: {gameObject.name}, 实例ID: {GetInstanceID()}");
        //gameObject.SetActive(false);
        // 初始化 AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = 0.8f;
        }
    }
    // 统一播放音效的方法
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    public void OpenPanel(InteractableObject target)
    {
        //Debug.Log($"OpenPanel 被调用, target={target?.name}");
        currentTarget = target;
        currentInput = "";
        UpdateDisplay();

        if (digitalLayout != null)
            digitalLayout.SetActive(true);

        gameObject.SetActive(true);

        // 加上这一行，强制UI立即准备好
        Canvas.ForceUpdateCanvases();
    }

    public void OnNumberClick(string number)
    {
        //Debug.Log($"OnNumberClick 被调用, number={number}, 当前currentInput={currentInput}");
        // 播放数字点击音效
        PlaySound(numberClickSound);

        if (currentInput.Length < maxLength)
        {
            currentInput += number;
            UpdateDisplay();
            //Debug.Log($"添加后 currentInput={currentInput}");
        }
    }

    public void OnDeleteClick()
    {
        // 播放删除音效
        PlaySound(deleteSound);

        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            UpdateDisplay();
        }
    }

    public void OnConfirmClick()
    {
      
        if (currentTarget == null)
        {
            Debug.LogError("currentTarget 是 null！请检查 OpenPanel 是否被正确调用");
            UIManager.Instance.ShowHint("出错了，请重新点击箱子");
            gameObject.SetActive(false);
            return;
        }

        if (currentTarget.CheckPassword(currentInput))
        {
            // 播放成功音效
            PlaySound(successSound);

            currentTarget.Unlock();
            gameObject.SetActive(false);
            UIManager.Instance.ShowHint("解锁成功！");
        }
        else
        {
            // 播放错误音效
            PlaySound(errorSound);

            Debug.Log("密码错误！");
            currentInput = "";
            UpdateDisplay();
            UIManager.Instance.ShowHint("密码错误！");
        }
    }

    public void OnCloseClick()
    {
        // 播放关闭音效
        PlaySound(closeSound);

        gameObject.SetActive(false);
    }

    private void UpdateDisplay()
    {
        string display = "";
        for (int i = 0; i < currentInput.Length; i++)
        {
            display += "●";
        }
        while (display.Length < maxLength)
        {
            display += "○";
        }
        passwordDisplay.text = display;
    }
}