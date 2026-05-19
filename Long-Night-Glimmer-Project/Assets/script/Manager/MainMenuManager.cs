using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;


    [Header("音乐设置")]
    public AudioClip backgroundMusic;  // 背景音乐
    public AudioClip buttonClickSound; // 按钮点击音效

    private AudioSource musicSource;   // 背景音乐播放器
    private AudioSource sfxSource;     // 音效播放器


    void Start()
    {
        // 初始化音频播放器
        SetupAudioSources();

        // 播放背景音乐
        PlayBackgroundMusic();

        // 绑定按钮事件
        if (startButton != null)
        {
            startButton.onClick.AddListener(() => PlayButtonClick(StartGame));
        }
        else
        {
            Debug.LogWarning("startButton 未绑定！");
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(() => PlayButtonClick(QuitGame));
        }
        else
        {
            Debug.LogWarning("quitButton 未绑定！");
        }
    }

    void SetupAudioSources()
    {
        // 创建背景音乐播放器
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = 0.6f;
        musicSource.playOnAwake = false;

        // 创建音效播放器
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = 0.8f;
        sfxSource.playOnAwake = false;
    }

    void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
            Debug.Log("主菜单背景音乐已开始播放");
        }
        else
        {
            Debug.LogWarning("未设置背景音乐或 AudioSource 不存在");
        }
    }

    void PlayButtonClick(System.Action action)
    {
        // 播放点击音效
        if (buttonClickSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(buttonClickSound);
            // 等待音效播放一小段时间后再执行动作（可选）
            Invoke(nameof(ExecuteAction), 0.1f);
            // 保存要执行的动作
            pendingAction = action;
        }
        else
        {
            // 没有音效直接执行
            action?.Invoke();
        }
    }

    private System.Action pendingAction;

    void ExecuteAction()
    {
        pendingAction?.Invoke();
        pendingAction = null;
    }

    void StartGame()
    {
        Debug.Log("开始游戏，加载 PersistentScene...");
        // 加载 persistent scene
        SceneManager.LoadScene("persistentscene");
    }

    void QuitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}