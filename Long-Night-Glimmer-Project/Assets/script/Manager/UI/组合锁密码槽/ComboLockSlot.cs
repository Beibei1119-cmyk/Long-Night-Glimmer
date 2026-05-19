using UnityEngine;
using UnityEngine.UI;

public class ComboLockSlot : MonoBehaviour
{
    public string requiredItemName;   // 需要的物品，如"红色碎片"
    public Sprite filledSprite;        // 放入后的图片
    public Sprite emptySprite;         // 空状态图片（重置时用）
          
    //================================
    public string slotId = "";  // 新增：唯一标识，如 "Slot0"
    //================================

    private Image slotImage;
    private bool isFilled = false;
    private ComboLockPanel parentPanel;  // 添加父面板引用

    [Header("音效")]
    public AudioClip placeSound;  // 放入物品音效


    void Start()
    {
        slotImage = GetComponent<Image>();
        parentPanel = GetComponentInParent<ComboLockPanel>();  // 自动查找父面板

        //================================
        // 如果没有设置 slotId，用物体名字
        if (string.IsNullOrEmpty(slotId))
            slotId = gameObject.name;
        //===================================
    }

    public bool TryPlaceItem(string itemName)
    {
        if (isFilled) return false;
        if (itemName != requiredItemName) return false;

        isFilled = true;
        if (filledSprite != null) slotImage.sprite = filledSprite;

        //======================================
        // 保存状态
        SaveSlotState();
        //=======================================


        // 通知父面板
        if (parentPanel != null) parentPanel.OnSlotFilled();
        Debug.Log($"凹槽 {requiredItemName} 已放入！");
        return true;
    }
    public void ResetSlot()
    {
        isFilled = false;
        if (slotImage != null && emptySprite != null)
        {
            slotImage.sprite = emptySprite;
        }
    }
    public bool IsFilled()
    {
        return isFilled;
    }


    //=============================================================
    // 新增：加载状态（从外部调用）
    public void LoadState(bool filled)
    {
        // 确保组件存在
        if (slotImage == null)
            slotImage = GetComponent<Image>();

        if (slotImage == null) return;

        isFilled = filled;
        if (filled && filledSprite != null)
            slotImage.sprite = filledSprite;
        else if (!filled && emptySprite != null)
            slotImage.sprite = emptySprite;
    }


    // 新增：保存状态到 SceneStateManager
    private void SaveSlotState()
    {
        if (parentPanel == null || parentPanel.targetBox == null) return;

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string boxName = parentPanel.targetBox.name;
        string key = $"{boxName}_{slotId}";

        // 存储到 SceneStateManager（运行时，不写文件）
        SceneStateManager.Instance.SetComboSlotState(sceneName, key, isFilled);
    }

    //=============================================================
}