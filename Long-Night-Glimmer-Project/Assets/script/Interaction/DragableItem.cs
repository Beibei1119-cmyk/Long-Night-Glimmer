using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("物品信息")]
    public string itemName = "铜钥匙";

    // ========== 删除这行，不再需要 ==========
    // public Sprite dragSprite;
    // =====================================

    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;
    private Canvas parentCanvas;
    private GameObject dragImage;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        if (parentCanvas != null)
            transform.SetParent(parentCanvas.transform);

        CreateDragImage();
    }

    private void CreateDragImage()
    {
        if (dragImage != null) return;

        dragImage = new GameObject("DragImage");
        dragImage.transform.SetParent(parentCanvas.transform);
        dragImage.transform.SetAsLastSibling();

        Image img = dragImage.AddComponent<Image>();
        img.raycastTarget = false;

        // ========== 从 Resources/DragIcons/ 加载拖拽图片 ==========
        Sprite dragSprite = Resources.Load<Sprite>($"DragIcons/{itemName}");

        if (dragSprite != null)
        {
            img.sprite = dragSprite;
            
        }
        else
        {
            // 如果没有专用的拖拽图片，就用快捷栏的图片
            Image originalImage = GetComponent<Image>();
            img.sprite = originalImage != null ? originalImage.sprite : null;
            Debug.Log($"加载拖拽图片：DragIcons/{itemName} -> 失败，使用默认图标");
        }
        // ========================================================

        RectTransform rect = dragImage.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(80, 80);
        dragImage.transform.position = Input.mousePosition;
    }

    //// ========== 新增方法：根据物品类型获取拖拽图片 ==========
    //private Sprite GetDragSpriteFromUIManager()
    //{
    //    if (UIManager.Instance == null) return null;

    //    // 根据物品名称判断类型
    //    if (itemName.Contains("钥匙"))
    //    {
    //        return UIManager.Instance.keyDragSprite;
    //    }
    //    else if (itemName.Contains("发夹"))
    //    {
    //        return UIManager.Instance.gemDragSprite;
    //    }

    //    return null;
    //}
    //// ==================================================

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        if (dragImage != null)
        {
            dragImage.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log($"[DragableItem] OnEndDrag 开始, itemName={itemName}");
        // 先销毁拖拽图片
        if (dragImage != null)
        {
            Destroy(dragImage);
            dragImage = null;
            //Debug.Log($"[DragableItem] 拖拽图片已销毁");
        }

        // 射线检测
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        bool unlockSuccess = false;

        if (hit.collider != null)
        {
            InteractableObject obj = hit.collider.GetComponent<InteractableObject>();
            //Debug.Log($"[DragableItem] 点击到的物体: {hit.collider.name}, InteractableObject={obj != null}");

            if (obj != null )
            {
                unlockSuccess = obj.TryUnlockWithKey(itemName);
                //Debug.Log($"[DragableItem] TryUnlockWithKey 返回: {unlockSuccess}");
            }
        }


        // ========== 无论成功与否，都要恢复原物品的位置和显示！ ==========
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        //Debug.Log($"[DragableItem] 恢复原物品显示");
        // 回到原位
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        //Debug.Log($"[DragableItem] 恢复原物品显示，回到原位: {originalParent?.name}");

        if (unlockSuccess)
        {
            //Debug.Log($"[DragableItem] 解锁成功！准备从背包移除: {itemName}");
            InventoryManager.Instance.RemoveItem(itemName);
            //Debug.Log($"[DragableItem] RemoveItem 调用完成");
        }
        else
        {
            Debug.Log($"[DragableItem] 解锁失败");
        }
    }
}