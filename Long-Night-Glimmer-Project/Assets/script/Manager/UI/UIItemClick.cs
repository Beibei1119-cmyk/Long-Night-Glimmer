using UnityEngine;
using UnityEngine.EventSystems;

public class UIItemClick : MonoBehaviour, IPointerClickHandler
{
    [Header("物品信息")]
    public string itemName = "物品";
    public AudioClip pickupSound;  // 添加这行


    [Header("拾取提示文字")]
    public string pickupHint = "";  // 拾取时显示的文字，留空则显示"获得 + 物品名"

    private void Start()
    {
     

        var img = GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            img.raycastTarget = true;
          
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 播放拾取音效
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, Camera.main.transform.position);
        }

        InventoryManager.Instance.AddItem(itemName);

        string hint = string.IsNullOrEmpty(pickupHint) ? $"获得 {itemName}" : pickupHint;
        UIManager.Instance.ShowHint(hint);

        // 保存状态
        string sceneName = "PersistentScene";
        SceneStateManager.Instance.SaveUIItemState(sceneName, gameObject.name, true);

        gameObject.SetActive(false);
    }
}
