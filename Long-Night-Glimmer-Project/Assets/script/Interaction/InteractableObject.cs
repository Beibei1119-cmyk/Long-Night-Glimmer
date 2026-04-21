using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public enum ObjectType
    {
        KeyLock,        // 钥匙锁（箱子、抽屉）/
        PasswordLock,   // 密码锁/
        DetailOnly,     // 只显示详情面板（画、书）
        DetailWithOpen, // 先切换形态，再显示详情面板（日历，窗户）/
        OnlyHint,        // 只显示提示（石头、花）
        ComboLock,      //组合锁/
        OpenThenInside      // 新增：先打开 → 内部面板（窗户里有东西）
    }

    [Header("门设置")]
    public bool disableClickAfterUnlock = false;  // 解锁后禁用点击

    [Header("交互类型")]
    public ObjectType objectType = ObjectType.KeyLock;

    // ========== 锁相关（KeyLock / PasswordLock 用）==========
    [Header("锁设置")]
    public bool isLocked = true;
    public string lockHint = "这个锁着呢";
    public string requiredKey = "铜钥匙";
    public string correctPassword = "1234";

    // ========== 物品状态 ==========
    [Header("物品状态")]
    public bool isOpen = false;
    public GameObject closedState;
    public GameObject openState;

    // ========== 详情面板设置（DetailOnly / DetailWithOpen 用）==========
    [Header("详情面板设置")]
    public Sprite detailImage;
    [TextArea(3, 5)]
    public string detailDescription = "物品描述";
    public string detailHint = "查看物品";



    // ========== 内部面板设置（箱子类用）==========
    [Header("内部面板")]
    public Sprite insideBackgroundImage;
    public bool hasKey = false;    //铜钥匙
    public bool hasClip = false;  //发夹
    public bool hasKey2 = false;   //银钥匙
    public bool hasGem1 = false;      // 宝石1
    public bool hasGem2 = false;      // 宝石2



    // ========== 提示框设置（OnlyHint 用）==========
    [Header("提示框设置")]
    public string hintMessage = "这是一个物体";

    [Header("音效")]
    public AudioClip openSound;

    private void Start()
    {
        UpdateVisual();
    }

    private void OnMouseDown()
    {
        // 检测是否点击在UI上
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }



        //单独只给门设置的：
        // 如果解锁后禁用点击，且已经打开，则不响应
        if (disableClickAfterUnlock && isOpen)
        {
            return;
        }


        // ========== 优先：已打开，显示内部面板 ==========
        if (isOpen)
        {
            // 箱子类：显示内部面板（拿物品）
            if (objectType == ObjectType.KeyLock ||
                objectType == ObjectType.PasswordLock ||
                objectType == ObjectType.ComboLock||
                objectType == ObjectType.OpenThenInside)
            {
                UIManager.Instance.insidePanel.Show(insideBackgroundImage, hasKey, hasClip, hasKey2, hasGem1, hasGem2);
                return;
            }
            // 窗户类：显示详情面板（放大图）
            if (objectType == ObjectType.DetailWithOpen)
                
            {
                UIManager.Instance.ShowHint(detailHint);
                UIManager.Instance.ShowDetail(detailImage, detailDescription);
                return;
            }

            // 默认
            return;
        }
        // ========== 类型1：只显示提示 ==========
        if (objectType == ObjectType.OnlyHint)
        {
            UIManager.Instance.ShowHint(hintMessage);
            return;
        }

        // ========== 类型2：只显示详情面板（不切换形态）==========
        if (objectType == ObjectType.DetailOnly)
        {
            UIManager.Instance.ShowHint(detailHint);
            UIManager.Instance.ShowDetail(detailImage, detailDescription);
            return;
        }

        // ========== 类型3：先切换形态，再显示详情面板 ==========
        if (objectType == ObjectType.DetailWithOpen)
        {
            if (!isOpen)
            {
                // 第一次点击：打开物体
                isOpen = true;
                UpdateVisual();
                UIManager.Instance.ShowHint($"打开了{gameObject.name}");
                SaveState();
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

        // ========== 类型：先打开，再显示内部面板（窗户里有物品）==========
        if (objectType == ObjectType.OpenThenInside)
        {
            if (!isOpen)
            {
                // 第一次点击：打开物体
                isOpen = true;
                UpdateVisual();
                UIManager.Instance.ShowHint($"打开了{gameObject.name}");
                SaveState();
                return;
            }
            else
            {
                // 已打开：显示内部面板（里面有可拾取物品）
                UIManager.Instance.insidePanel.Show(insideBackgroundImage, hasKey, hasClip, hasKey2, hasGem1, hasGem2);
                return;
            }
        }

        // ========== 类型：组合锁 ==========
        if (objectType == ObjectType.ComboLock)
        {
            if (!isOpen)
            {
                UIManager.Instance.ShowComboLockPanel(this);
                UIManager.Instance.ShowHint(hintMessage);
                return;
            }
            else
            {
                // 已打开：显示内部面板（里面有可拾取物品）
                UIManager.Instance.insidePanel.Show(insideBackgroundImage, hasKey, hasClip, hasKey2, hasGem1, hasGem2);
                return;
            }
        }

        // ========== 类型5：钥匙锁 ==========
        if (objectType == ObjectType.KeyLock && isLocked)
        {
            if (!isOpen)
            {
                UIManager.Instance.ShowHint(lockHint);
                return;
            }
            else
            {
                // 已打开：显示内部面板（里面有可拾取物品）
                UIManager.Instance.insidePanel.Show(insideBackgroundImage, hasKey, hasClip, hasKey2, hasGem1, hasGem2);
                return;
            }
        }

        // ========== 类型6：密码锁 ==========
        if (objectType == ObjectType.PasswordLock && isLocked)
        {
            if (!isOpen)
            {
                UIManager.Instance.ShowHint(lockHint);
                UIManager.Instance.ShowPasswordPanel(this);
                return;
            }
            else
            {
                // 已打开：显示内部面板（里面有可拾取物品）
                UIManager.Instance.insidePanel.Show(insideBackgroundImage, hasKey, hasClip, hasKey2, hasGem1, hasGem2);
                return;
            }
        }

        // ========== 解锁后的开关逻辑 ==========
        isOpen = !isOpen;
        UpdateVisual();
        SaveState();

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

    private void SaveState()
    {
        SceneStateManager.Instance?.SaveBoxState(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            gameObject.name,
            isOpen,
            isLocked
        );
    }

    public bool CheckPassword(string input)
    {
        return input == correctPassword;
    }

    public void Unlock()
    {
        Debug.Log($"Unlock 被调用，{gameObject.name}");
        isLocked = false;
        isOpen = true;
        UpdateVisual();
        UIManager.Instance.ShowHint($"{gameObject.name} 已解锁并打开");
        SaveState();

        if (openSound != null)
        {
            AudioSource.PlayClipAtPoint(openSound, transform.position);
        }
    }

    public bool TryUnlockWithKey(string keyName)
    {
        if (objectType != ObjectType.KeyLock) return false;
        if (!isLocked) return false;

        if (keyName == requiredKey)
        {
            Unlock();
            return true;
        }

        UIManager.Instance.ShowHint($"这个锁需要 {requiredKey}");
        return false;
    }

    public void SetOpenState(bool open)
    {
        isOpen = open;
        UpdateVisual();
    }

    public void SetUnlockedState(bool unlocked)
    {
        isLocked = !unlocked;
    }
}