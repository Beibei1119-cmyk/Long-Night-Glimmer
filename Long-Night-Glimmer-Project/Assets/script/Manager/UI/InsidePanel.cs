using UnityEngine;
using UnityEngine.UI;

public class InsidePanel : MonoBehaviour
{
    [Header("物品")]
    public GameObject keyItem;      // 钥匙物品（普通图片）
    public GameObject clipItem;     // 发夹物品（普通图片）
    public GameObject key2Item;     // 银钥匙（新增）
    public GameObject gem1Item;     // 宝石1（新增）
    public GameObject gem2Item;     // 宝石2（新增）



    public Button closeButton;      // 关闭按钮（还是按钮）

    [Header("背景图")]
    public Image backgroundImage;  // 拖入 BackgroundImage


    private void Start()
    {
        //有这个代码则不需要手动关闭那个ui，没有的话则需要手动关闭哈。
        //gameObject.SetActive(false);
        // 给物品添加点击拾取功能
       

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

    }

    private void AddPickupToItem(GameObject item, string itemName)
    {
        Debug.Log($"[AddPickupToItem] 开始, item={item.name}, itemName={itemName}");

        // 确保有 Image 组件
        Image img = item.GetComponent<Image>();
        if (img == null)
        {
            img = item.AddComponent<Image>();
            Debug.Log($"[AddPickupToItem] 添加 Image 组件");
        }
        img.raycastTarget = true;

        // 移除可能存在的 PickupItem（避免冲突）
        PickupItem oldPickup = item.GetComponent<PickupItem>();
        if (oldPickup != null)
            Destroy(oldPickup);

        // 移除可能存在的 Collider（UI 不需要）
        BoxCollider2D col = item.GetComponent<BoxCollider2D>();
        if (col != null)
            Destroy(col);

        // 添加 UI 点击脚本
        UIItemClick click = item.GetComponent<UIItemClick>();
        if (click == null)
            click = item.AddComponent<UIItemClick>();
        click.itemName = itemName;

    }



    public void Show(Sprite bgImage, bool showKey, bool showClip, bool showKey2, bool showGem1, bool showGem2)
    {
        // 设置背景图
        if (backgroundImage != null && bgImage != null)
            backgroundImage.sprite = bgImage;

        // 获取当前场景名
        string sceneName = "PersistentScene";

        // 根据保存的状态决定是否显示物品
        bool keyShouldShow = showKey && !SceneStateManager.Instance.IsUIItemPickedUp(sceneName, "KeyItem");
        bool clipShouldShow = showClip && !SceneStateManager.Instance.IsUIItemPickedUp(sceneName, "ClipItem");
        bool key2ShouldShow = showKey2 && !SceneStateManager.Instance.IsUIItemPickedUp(sceneName, "Key2Item");
        bool gem1ShouldShow = showGem1 && !SceneStateManager.Instance.IsUIItemPickedUp(sceneName, "Gem1Item");
        bool gem2ShouldShow = showGem2 && !SceneStateManager.Instance.IsUIItemPickedUp(sceneName, "Gem2Item");

        if (keyItem != null) keyItem.SetActive(keyShouldShow);
        if (clipItem != null) clipItem.SetActive(clipShouldShow);
        if (key2Item != null) key2Item.SetActive(key2ShouldShow);
        if (gem1Item != null) gem1Item.SetActive(gem1ShouldShow);
        if (gem2Item != null) gem2Item.SetActive(gem2ShouldShow);
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }


  
}