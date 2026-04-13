using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    //[Header("拖拽图片")]
    //public Sprite keyDragSprite;  // 钥匙拖拽时的图片
    //public Sprite gemDragSprite;  // 发夹拖拽时的图片

    [Header("内部面板")]
    public InsidePanel insidePanel;  // 改成 InsidePanel 类型

    [Header("提示面板")]
    public GameObject hintPanel;
    public Text hintText;

    [Header("快捷栏")]
    public Image hotbarIcon;
    public Text hotbarName;
    public Button leftButton;
    public Button rightButton;

    public Sprite defaultIcon;

    // ========== 密码面板 ==========
    [Header("密码面板")]
    public PasswordPanel passwordPanel;
    // =================================


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

    //===========================================
   
  


    //============================================

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
        //Debug.Log($"[UIManager] RefreshHotbar 开始");

        if (hotbarIcon == null || hotbarName == null)
        {
            Debug.LogWarning("[UIManager] hotbarIcon 或 hotbarName 未赋值");
            return;
        }

        string currentItem = InventoryManager.Instance.CurrentItem;
        //Debug.Log($"[UIManager] currentItem = {currentItem ?? "null"}");

        if (string.IsNullOrEmpty(currentItem))
        {
            //Debug.Log("[UIManager] 清空快捷栏显示");

            // ========== 清空显示 ==========
            hotbarIcon.sprite = null;
            hotbarIcon.color = new Color(1, 1, 1, 0);
            hotbarName.text = "";

            // 移除拖拽脚本
            DragableItem drag = hotbarIcon.GetComponent<DragableItem>();
            if (drag != null)
            {
                //Debug.Log("[UIManager] 移除旧的 DragableItem 脚本");
                Destroy(drag);
            }
            // ============================
        }
        else
        {
            //Debug.Log($"[UIManager] 显示物品: {currentItem}");

            // ========== 显示物品 ==========
            hotbarIcon.color = new Color(1, 1, 1, 1);
            hotbarName.text = currentItem;

            // 加载图标
            Sprite icon = Resources.Load<Sprite>($"Icons/{currentItem}");
            //Debug.Log($"[UIManager] 加载图片：Icons/{currentItem} -> {(icon != null ? "成功" : "失败")}");

            if (icon != null)
            {
                hotbarIcon.sprite = icon;
            }
            else
            {
                hotbarIcon.sprite = null;
                hotbarIcon.color = new Color(1, 0.5f, 0.5f, 1);
            }

            // ========== 先移除旧的拖拽脚本，再添加新的 ==========
            DragableItem oldDrag = hotbarIcon.GetComponent<DragableItem>();
            if (oldDrag != null)
            {
               
                Destroy(oldDrag);
            }

            DragableItem newDrag = hotbarIcon.gameObject.AddComponent<DragableItem>();
            newDrag.itemName = currentItem;
            //Debug.Log($"[UIManager] 添加新的 DragableItem 脚本, itemName={currentItem}");
            // ========================================================
        }

        //Debug.Log("[UIManager] RefreshHotbar 完成");
    }


    private void SetItemIcon(string itemName)
    {
        hotbarIcon.sprite = Resources.Load<Sprite>($"Icons/{itemName}");
    }


    // ========== 显示密码面板 ==========
    public void ShowPasswordPanel(InteractableObject target)
    {
        Debug.Log($"ShowPasswordPanel 被调用, target={target?.name}");
        if (passwordPanel != null)
        {
            passwordPanel.OpenPanel(target);
        }
        else
        {
            Debug.LogError("PasswordPanel 未赋值！");
        }
    }
    // =================================

    public void ShowInsidePanel(Sprite bgImage, bool showKey, bool showClip)
    {
        if (insidePanel != null)
            insidePanel.Show(bgImage, showKey, showClip);
    }

    public void HideInsidePanel()
    {
        if (insidePanel != null)
            insidePanel.Hide();
    }

}