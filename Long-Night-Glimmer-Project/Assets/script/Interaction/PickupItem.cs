using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string itemName = "铜钥匙";
    public string saveId = "";  // 新增：唯一标识，如 "RedShard"

    private void Start()
    {
        // 检查是否已经被拾取过
        if (!string.IsNullOrEmpty(saveId))
        {
            bool isPickedUp = SceneStateManager.Instance.IsPickupItemPickedUp(saveId);
            if (isPickedUp)
            {
                gameObject.SetActive(false);
                return;
            }
        }

        Debug.Log($"[PickupItem] Start 被调用, gameObject={gameObject.name}, itemName={itemName}");
    }

    private void OnMouseDown()
    {
        Debug.Log($"[PickupItem] OnMouseDown 被点击！gameObject={gameObject.name}, itemName={itemName}");

        // 添加到背包
        InventoryManager.Instance.AddItem(itemName);
        UIManager.Instance.ShowHint($"获得 {itemName}");

        // 保存状态
        if (!string.IsNullOrEmpty(saveId))
        {
            SceneStateManager.Instance.SavePickupItemState(saveId, true);
        }

        // 销毁物品
        Destroy(gameObject);

    }


}