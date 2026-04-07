using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string itemName = "铜钥匙";

    private void OnMouseDown()
    {
        Debug.Log($"点击到了: {gameObject.name}");

        // 添加到背包
        InventoryManager.Instance.AddItem(itemName);

        // 显示提示
        UIManager.Instance.ShowHint($"获得 {itemName}");

        // 销毁物品
        Destroy(gameObject);
    }
}