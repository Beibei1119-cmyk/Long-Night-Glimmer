using UnityEngine;
using static UnityEditor.PlayerSettings.Switch;

public class InteractableObject : MonoBehaviour
{
    [Header("锁类型")]
    public LockType lockType = LockType.KeyLock;

    [Header("锁定状态")]
    public bool isLocked = true;           // 是否锁着
    public string lockHint = "这个抽屉锁着呢";  // 锁着时的提示

    [Header("钥匙锁设置")]
    public string requiredKey = "铜钥匙";

    [Header("物品状态")]
    public bool isOpen = false;           // 当前是否打开

    [Header("外观")]
    public GameObject closedState;        // 关闭时的模型/图片
    public GameObject openState;          // 打开时的模型/图片

    [Header("密码锁设置")]
    public string correctPassword = "1234";

    


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

        // 检测是否点击在UI上
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("点击在UI上，忽略物体点击");
            return;
        }

        Debug.Log($"点击到了: {gameObject.name}");

        // ========== 钥匙锁：检查是否锁着 ==========
        if (lockType == LockType.KeyLock && isLocked)
        {
            UIManager.Instance.ShowHint(lockHint);
            return;  // 锁着，不能继续
        }
        // =====================================

        // ========== 密码锁：弹出密码面板 ==========
        if (lockType == LockType.PasswordLock && isLocked)
        {
            // 先显示提示
            UIManager.Instance.ShowHint(lockHint);
            // 再弹出密码面板
            UIManager.Instance.ShowPasswordPanel(this);
            return;
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


    // ========== 新增：密码验证方法 ==========
    public bool CheckPassword(string input)
    {
        Debug.Log($"CheckPassword: 输入={input}, 正确密码={correctPassword}, 相等={input == correctPassword}");
        return input == correctPassword;
    }

    // =====================================


    // ========== 解锁方法（被钥匙调用） ==========
    public void Unlock()
    {
        //Debug.Log($"[InteractableObject] Unlock 开始, 物品: {gameObject.name}");

        isLocked = false;
        isOpen = true;
        UpdateVisual();

        //Debug.Log($"[InteractableObject] 状态已更新: isLocked={isLocked}, isOpen={isOpen}");


        UIManager.Instance.ShowHint($"{gameObject.name} 已解锁并打开");
        // ========== 注意：这里不要调用 RemoveItem ==========
        // RemoveItem 会在 DragableItem.OnEndDrag 中调用
        // ==================================================

        //Debug.Log($"[InteractableObject] Unlock 完成，注意：未调用 RemoveItem");


        // 播放音效
        if (openSound != null)
        {
            AudioSource.PlayClipAtPoint(openSound, transform.position);
        }
    }

    // ===============================================

    // ========== 新增：锁类型枚举 ==========
    public enum LockType
    {
        KeyLock,      // 钥匙锁
        PasswordLock  // 密码锁
    }
    // =================================

    public bool TryUnlockWithKey(string keyName)
    {

        //Debug.Log($"[InteractableObject] TryUnlockWithKey: keyName={keyName}, lockType={lockType}, isLocked={isLocked}");
        if (lockType != LockType.KeyLock)
        {
            Debug.Log($"[InteractableObject] 不是钥匙锁，返回 false");
            return false;
        }

        if (!isLocked)
        {
            Debug.Log($"[InteractableObject] 已经解锁了，返回 false");
            return false;
        }

        if (keyName == requiredKey)
        {
            //Debug.Log($"[InteractableObject] 钥匙匹配！调用 Unlock()");
            Unlock();
            return true;
        }

        Debug.Log($"[InteractableObject] 钥匙不匹配，需要: {requiredKey}");
        UIManager.Instance.ShowHint($"这个锁需要 {requiredKey}");
        return false;
    }

}