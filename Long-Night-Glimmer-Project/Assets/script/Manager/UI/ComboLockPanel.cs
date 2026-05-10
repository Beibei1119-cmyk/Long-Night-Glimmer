using UnityEngine;

public class ComboLockPanel : MonoBehaviour
{
    [Header("关联箱子")]
    public InteractableObject targetBox;

    [Header("凹槽列表")]
    public ComboLockSlot[] slots;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Open(InteractableObject box)
    {
        targetBox = box;
        //// 重置所有凹槽状态
        //foreach (var slot in slots)
        //{
        //    slot.ResetSlot();
        //}

//===========================================
        // 改为：加载保存的状态，而不是重置
        LoadAllSlots();
//==============================================

        gameObject.SetActive(true);
    }

    public void OnSlotFilled()
    {
        foreach (var slot in slots)
        {
            if (!slot.IsFilled()) return;
        }

        // 所有凹槽都满了，解锁箱子
        Debug.Log($"所有凹槽已满，准备解锁，targetBox={targetBox?.name}");
        if (targetBox != null)
        {
            Debug.Log("调用 targetBox.Unlock()");
            targetBox.Unlock();
        }
        // 关闭面板
        gameObject.SetActive(false);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }


    //======================================================
    // 新增：加载所有凹槽的保存状态
    private void LoadAllSlots()
    {
        if (targetBox == null) return;

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string boxName = targetBox.name;

        foreach (var slot in slots)
        {
            if (slot == null) continue;

            string key = $"{boxName}_{slot.slotId}";
            bool isFilled = SceneStateManager.Instance.GetComboSlotState(sceneName, key);
            slot.LoadState(isFilled);
        }
    }
    //======================================================

}