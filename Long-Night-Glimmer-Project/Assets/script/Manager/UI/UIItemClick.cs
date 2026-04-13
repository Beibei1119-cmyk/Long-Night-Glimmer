using UnityEngine;
using UnityEngine.EventSystems;

public class UIItemClick : MonoBehaviour, IPointerClickHandler
{
    public string itemName = "物品";

    private void Start()
    {
        Debug.Log($"[UIItemClick] Start: {gameObject.name}, itemName={itemName}");

        // 确保 Image 的 RaycastTarget 为 true
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
        UIManager.Instance.ShowHint($"获得 {itemName}");
        gameObject.SetActive(false);
    }
}