using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [Header("淡入淡出设置")]
    public Image fadeImage;
    public float fadeDuration = 0.5f;

    [Header("过渡文字")]
    public Text transitionText;
    public float textDisplayTime = 0.8f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.raycastTarget = false;
        }

        if (transitionText != null)
            transitionText.gameObject.SetActive(false);
    }

    public void FadeOut(System.Action onComplete = null)
    {
        StartCoroutine(Fade(1f, onComplete));
    }

    public void FadeIn(System.Action onComplete = null)
    {
        StartCoroutine(Fade(0f, onComplete));
    }

    public void FadeOutWithText(string text, System.Action onComplete = null)
    {
        StartCoroutine(FadeOutWithTextCoroutine(text, onComplete));
    }

    public void FadeInWithText(System.Action onComplete = null)
    {
        StartCoroutine(FadeInWithTextCoroutine(onComplete));
    }

    private IEnumerator Fade(float targetAlpha, System.Action onComplete)
    {
        if (fadeImage == null) yield break;

        fadeImage.raycastTarget = true;
        float startAlpha = fadeImage.color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, targetAlpha);

        if (targetAlpha == 0f)
            fadeImage.raycastTarget = false;

        onComplete?.Invoke();
    }

    // 改成 public
    public IEnumerator FadeOutWithTextCoroutine(string text, System.Action onComplete = null)
    {
        yield return StartCoroutine(Fade(1f, null));

        if (transitionText != null)
        {
            transitionText.text = text;
            transitionText.gameObject.SetActive(true);
        }


        // ========== 新增：等待一段时间 ==========
        yield return new WaitForSeconds(textDisplayTime);
        // =====================================


        onComplete?.Invoke();
    }

    // 改成 public
    public IEnumerator FadeInWithTextCoroutine(System.Action onComplete = null)
    {
        // 注意：不要在这里隐藏文字，因为已经在 FadeOutWithTextCoroutine 中等待过了
        // 直接淡入，淡入完成后文字会自然被覆盖或隐藏

        yield return StartCoroutine(Fade(0f, null));

        // 淡入完成后隐藏文字
        if (transitionText != null)
            transitionText.gameObject.SetActive(false);

        onComplete?.Invoke();
    }

    public IEnumerator FadeOutCoroutine()
    {
        yield return StartCoroutine(Fade(1f, null));
    }

    public IEnumerator FadeInCoroutine()
    {
        yield return StartCoroutine(Fade(0f, null));
    }
}