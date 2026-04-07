using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("锁定状态")]
    public bool isLocked = true;           // 是否锁着
    public string lockHint = "这个抽屉锁着呢";  // 锁着时的提示


    [Header("物品状态")]
    public bool isOpen = false;           // 当前是否打开

    [Header("外观")]
    public GameObject closedState;        // 关闭时的模型/图片
    public GameObject openState;          // 打开时的模型/图片

    [Header("音效（可选）")]
    public AudioClip openSound;           // 打开音效

    private void Start()
    {
        UpdateVisual();
    }


    private void OnMouseDown()
    {
        Debug.Log($"点击到了: {gameObject.name}");
        // 切换状态：TODO：当加入解密系统后，解密完成后才能点击开门哦~

        // ========== 新增：检查是否锁着 ==========
        if (isLocked)
        {
            UIManager.Instance.ShowHint(lockHint);
            return;  // 锁着，不能继续
        }
        // =====================================

        isOpen = !isOpen;
        UpdateVisual();
        // 播放音效（如果有）
        if (openSound != null)
        {
            AudioSource.PlayClipAtPoint(openSound, transform.position);
        }

        Debug.Log($"{gameObject.name} 已 {(isOpen ? "打开" : "关闭")}");
    }

    private void UpdateVisual()
    {
        if (closedState != null)
            closedState.SetActive(!isOpen);
        if (openState != null)
            openState.SetActive(isOpen);
    }


    // ========== 新增：解锁方法（被钥匙调用） ==========
    public void Unlock()
    {
        isLocked = false;
        UIManager.Instance.ShowHint($"{gameObject.name} 已解锁");
    }
    // ===============================================


}