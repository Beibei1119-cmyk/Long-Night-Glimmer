using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;      // 场景名称
        public AudioClip musicClip;   // 对应的音乐
        [Range(0f, 1f)]
        public float volume = 0.6f;   // 音量
        public float fadeTime = 1f;   // 淡入淡出时间
    }

    public SceneMusic[] sceneMusicList;
    public float defaultFadeTime = 1f;

    private AudioSource audioSource;
    private string currentSceneName;
    private Coroutine currentFadeCoroutine;

    void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 设置 AudioSource
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.loop = true;
            audioSource.volume = 0;
            audioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 跳过 PersistentScene，不给它配音乐
        if (scene.name == "persistentscene")
        {
            Debug.Log("PersistentScene 加载，保持当前音乐");
            return;
        }

        // 查找场景对应的音乐配置
        foreach (var sceneMusic in sceneMusicList)
        {
            if (sceneMusic.sceneName == scene.name)
            {
                float fadeTime = sceneMusic.fadeTime > 0 ? sceneMusic.fadeTime : defaultFadeTime;
                StartCrossFade(sceneMusic.musicClip, fadeTime, sceneMusic.volume);
                currentSceneName = scene.name;
                return;
            }
        }

        // 没找到配置，静默处理（不报警告）
        Debug.Log($"场景 '{scene.name}' 未配置音乐");
    }

    public void StartCrossFade(AudioClip newClip, float fadeTime, float targetVolume)
    {
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);
        currentFadeCoroutine = StartCoroutine(CrossFadeRoutine(newClip, fadeTime, targetVolume));
    }

    private IEnumerator CrossFadeRoutine(AudioClip newClip, float fadeTime, float targetVolume)
    {
        // 淡出当前音乐
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        if (audioSource.isPlaying && audioSource.volume > 0)
        {
            while (elapsedTime < fadeTime / 2)
            {
                elapsedTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0, elapsedTime / (fadeTime / 2));
                yield return null;
            }
            audioSource.volume = 0;
        }

        // 切换音乐
        if (audioSource.clip != newClip)
        {
            audioSource.clip = newClip;
        }

        if (!audioSource.isPlaying && newClip != null)
        {
            audioSource.Play();
        }

        // 淡入新音乐
        elapsedTime = 0f;
        while (elapsedTime < fadeTime / 2)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, targetVolume, elapsedTime / (fadeTime / 2));
            yield return null;
        }

        audioSource.volume = targetVolume;
        currentFadeCoroutine = null;
    }
}