using UnityEngine;
using static UnityEditor.PlayerSettings.Switch;

public class InteractableObject : MonoBehaviour
{
    public enum ObjectType  //锁类型枚举
    {
        KeyLock,      // 钥匙锁
        PasswordLock, // 密码锁
        ShowDetail,  // 显示详情面板
        OnlyHint      // 只显示提示
    }

    [Header("锁类型")]
    public ObjectType lockType = ObjectType.KeyLock;

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

    // ========== 详情面板设置（ShowDetail类型用）==========
    [Header("详情面板设置")]
    public Sprite detailImage;
    [TextArea(3, 5)]
    public string detailDescription = "物品描述";
    public string detailHint = "查看物品";  // 新增：自定义提示文字


    [Header("内部面板")]
    public Sprite insideBackgroundImage;  // 这个箱子的内部背景图
    public bool hasKey = true;
    public bool hasClip = false;

    [Header("只显示提示框（OnlyHint）")]
    public string hintMessage = "这是一个物体";


    [Header("音效（可选）")]
    public AudioClip openSound;           // 打开音效

    private void Start()
    {
        UpdateVisual();
    }

    private void OnMouseDown()
    {
        // 切换状态：TODO：当加入解密系统后，解密完成后才能点击开门哦~

        // 检测是否点击在UI上
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {

            return;
        }

        // ========== 类型1：只显示提示 ==========
        if (lockType == ObjectType.OnlyHint)
        {
            UIManager.Instance.ShowHint(hintMessage);
            return;
        }

        // ========== 类型2：显示详情面板（需要先打开） ==========
        if (lockType == ObjectType.ShowDetail)
        {
            if (!isOpen)
            {
                // 第一次点击：打开物体
                isOpen = true;
                UpdateVisual();
                UIManager.Instance.ShowHint($"打开了{gameObject.name}");

                // 保存状态
                SceneStateManager.Instance?.SaveBoxState(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                    gameObject.name,
                    isOpen,
                    isLocked
                );
                return;
            }
            else
            {
                // 已打开：显示详情面板
                UIManager.Instance.ShowHint(detailHint);
                UIManager.Instance.ShowDetail(detailImage, detailDescription);
                return;
            }
        }

        // ========== 类型3：已打开且是 ShowDetail 类型：显示详情面板 ==========
        if (isOpen && lockType == ObjectType.ShowDetail)
        {
            UIManager.Instance.ShowHint(detailHint);
            UIManager.Instance.ShowDetail(detailImage, detailDescription);
            return;
        }

        // ========== 类型4：已打开，显示内部面板 ==========
        if (isOpen)
        {
            // 通过 InsidePanel 显示面板，按钮的显示/隐藏应该在 InsidePanel 内部处理
            UIManager.Instance.insidePanel.Show(insideBackgroundImage, hasKey, hasClip);
            return;
        }


        // ========== 类型5：钥匙锁 ==========
        if (lockType == ObjectType.KeyLock && isLocked)
        {
            UIManager.Instance.ShowHint(lockHint);
            return;  // 锁着，不能继续
        }


        // ========== 类型6：密码锁 ==========
        if (lockType == ObjectType.PasswordLock && isLocked)
        {
            // 先显示提示
            UIManager.Instance.ShowHint(lockHint);
            // 再弹出密码面板
            UIManager.Instance.ShowPasswordPanel(this);
            return;
        }

        // ================  解锁后的开关逻辑  =====================
        isOpen = !isOpen;
        UpdateVisual();
        //保存状态：
        SceneStateManager.Instance?.SaveBoxState(
        UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            gameObject.name,
            isOpen,
            isLocked
        ); 

      
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

 

    // ========== 解锁方法（被钥匙调用） ==========
    public void Unlock()
    {
        

        isLocked = false;
        isOpen = true;
        UpdateVisual();
        UIManager.Instance.ShowHint($"{gameObject.name} 已解锁并打开");
        // 注意：这里不要调用 RemoveItem , RemoveItem 会在 DragableItem.OnEndDrag 中调用
   

        // 播放音效
        if (openSound != null)
        {
            AudioSource.PlayClipAtPoint(openSound, transform.position);
        }

        // ========== 保存状态 ==========
        SceneStateManager.Instance?.SaveBoxState(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            gameObject.name,
            isOpen,
            isLocked
        );
    }



    public bool TryUnlockWithKey(string keyName)
    {

        //Debug.Log($"[InteractableObject] TryUnlockWithKey: keyName={keyName}, lockType={lockType}, isLocked={isLocked}");
        if (lockType != ObjectType.KeyLock)
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


    //============= 保存场景 ==============
    public void SetOpenState(bool open)
    {
        isOpen = open;
        UpdateVisual();
        Debug.Log($"[SetOpenState] {gameObject.name} isOpen={isOpen}");
    }

    public void SetUnlockedState(bool unlocked)
    {
        isLocked = !unlocked;
        Debug.Log($"[SetUnlockedState] {gameObject.name} isLocked={isLocked}, unlocked={unlocked}");
    }
}
