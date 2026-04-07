using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("提示面板")]
    public GameObject hintPanel;
    public Text hintText;

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
        if (hintPanel != null) hintPanel.SetActive(false);
    }

    /// <summary>
    /// 显示提示文字（2秒后自动消失）
    /// </summary>
    public void ShowHint(string message)
    {
        hintText.text = message;
        hintPanel.SetActive(true);
        CancelInvoke(nameof(HideHint));
        Invoke(nameof(HideHint), 2f);
    }

    private void HideHint()
    {
        hintPanel.SetActive(false);
    }
}