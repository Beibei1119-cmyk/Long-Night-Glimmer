using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string itemName = "铜钥匙";

    private void Start()
    {
        Debug.Log($"[PickupItem] Start 被调用, gameObject={gameObject.name}, itemName={itemName}");
    }

    private void OnMouseDown()
    {
        //Debug.Log($"点击到了: {gameObject.name}");

        Debug.Log($"[PickupItem] OnMouseDown 被点击！gameObject={gameObject.name}, itemName={itemName}");
        // 添加到背包
        InventoryManager.Instance.AddItem(itemName);

        // 显示提示
        UIManager.Instance.ShowHint($"获得 {itemName}");

        // 销毁物品
        Destroy(gameObject);
    }

   
}