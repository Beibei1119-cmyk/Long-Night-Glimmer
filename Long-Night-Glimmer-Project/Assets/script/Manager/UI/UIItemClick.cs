using UnityEngine;
using UnityEngine.EventSystems;

public class UIItemClick : MonoBehaviour, IPointerClickHandler
{
    [Header("物品信息")]
    public string itemName = "物品";

    [Header("拾取提示文字")]
    public string pickupHint = "";  // 拾取时显示的文字，留空则显示"获得 + 物品名"

    private void Start()
    {
        Debug.Log($"[UIItemClick] Start: {gameObject.name}, itemName={itemName}");

        var img = GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            img.raycastTarget = true;
            Debug.Log($"[UIItemClick] 设置 raycastTarget=true");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[UIItemClick] 被点击！{gameObject.name}, itemName={itemName}");
        InventoryManager.Instance.AddItem(itemName);

        // 显示提示：如果 pickupHint 有内容就用它，否则用默认的
        string hint = string.IsNullOrEmpty(pickupHint) ? $"获得 {itemName}" : pickupHint;
        UIManager.Instance.ShowHint(hint);

        gameObject.SetActive(false);
    }
}