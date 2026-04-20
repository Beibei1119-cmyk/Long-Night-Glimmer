using UnityEngine;
using UnityEngine.UI;

public class ComboLockSlot : MonoBehaviour
{
    public string requiredItemName;   // 需要的物品，如"红色碎片"
    public Sprite filledSprite;        // 放入后的图片
    public Sprite emptySprite;         // 空状态图片（重置时用）

    private Image slotImage;
    private bool isFilled = false;
    private ComboLockPanel parentPanel;  // 添加父面板引用

    void Start()
    {
        slotImage = GetComponent<Image>();
        parentPanel = GetComponentInParent<ComboLockPanel>();  // 自动查找父面板
    }

    public bool TryPlaceItem(string itemName)
    {
        if (isFilled) return false;
        if (itemName != requiredItemName) return false;

        isFilled = true;
        if (filledSprite != null) slotImage.sprite = filledSprite;

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
}