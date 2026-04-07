using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("提示面板")]
    public GameObject hintPanel;
    public Text hintText;

    [Header("快捷栏")]
    public Image hotbarIcon;
    public Text hotbarName;
    public Button leftButton;
    public Button rightButton;

    public Sprite defaultIcon;

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

        if (leftButton != null)
            leftButton.onClick.AddListener(() => InventoryManager.Instance.SelectPrevious());

        if (rightButton != null)
            rightButton.onClick.AddListener(() => InventoryManager.Instance.SelectNext());

        RefreshHotbar();
    }

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

    public void RefreshHotbar()
    {
        if (hotbarIcon == null)
        {
            Debug.LogWarning("hotbarIcon 未赋值");
            return;
        }
        if (hotbarName == null)
        {
            Debug.LogWarning("hotbarName 未赋值");
            return;
        }

        string currentItem = InventoryManager.Instance.CurrentItem;

        if (string.IsNullOrEmpty(currentItem))
        {
            hotbarIcon.sprite = null;
            hotbarIcon.color = new Color(1, 1, 1, 0);
            hotbarName.text = "";
        }
        else
        {
            hotbarIcon.color = new Color(1, 1, 1, 1);
            hotbarName.text = currentItem;

            Sprite icon = Resources.Load<Sprite>($"Icons/{currentItem}");

            // ===== 只有这一行关键调试 =====
            Debug.Log($"加载图片: Icons/{currentItem} -> {(icon != null ? "成功" : "失败")}");
            // ==============================

            if (icon != null)
            {
                hotbarIcon.sprite = icon;
            }
            else
            {
                hotbarIcon.sprite = null;
                hotbarIcon.color = new Color(1, 0.5f, 0.5f, 1);
            }
        }
    }

    private void SetItemIcon(string itemName)
    {
        hotbarIcon.sprite = Resources.Load<Sprite>($"Icons/{itemName}");
    }
}