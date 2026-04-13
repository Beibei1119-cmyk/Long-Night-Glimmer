using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<string> items = new List<string>();  // 物品列表
    public int selectedIndex = 0;  // 当前选中的物品索引

    public string CurrentItem => items.Count > 0 ? items[selectedIndex] : null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 左右键切换物品
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Q))
        {
            SelectPrevious();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.E))
        {
            SelectNext();
        }
    }

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log($"获得: {itemName}");

        // 每次捡起新物品，都自动选中它（最后一个）
        selectedIndex = items.Count - 1;

        RefreshUI();
    }

    public void RemoveItem(string itemName)
    {
        //Debug.Log($"[InventoryManager] RemoveItem 开始: itemName={itemName}");
        //Debug.Log($"[InventoryManager] 移除前背包: [{string.Join(", ", items)}], selectedIndex={selectedIndex}");

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == itemName)
            {
                //Debug.Log($"[InventoryManager] 找到物品在索引: {i}");
                items.RemoveAt(i);

                // 如果移除的是当前选中的物品
                if (selectedIndex >= items.Count)
                {
                    int oldIndex = selectedIndex;
                    selectedIndex = items.Count - 1;
                    //Debug.Log($"[InventoryManager] 调整 selectedIndex: {oldIndex} -> {selectedIndex}");
                }

                //Debug.Log($"[InventoryManager] 移除后背包: [{string.Join(", ", items)}], selectedIndex={selectedIndex}");
                //Debug.Log($"[InventoryManager] 当前选中物品: {CurrentItem}");

                // ========== 刷新 UI ==========
                //Debug.Log($"[InventoryManager] 调用 RefreshHotbar");
                UIManager.Instance.RefreshHotbar();
                // ============================

                return;
            }
        }
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public void SelectNext()
    {
        if (items.Count == 0) return;
        selectedIndex = (selectedIndex + 1) % items.Count;
        RefreshUI();
        Debug.Log($"选中: {CurrentItem}");
    }

    public void SelectPrevious()
    {
        if (items.Count == 0) return;
        selectedIndex = (selectedIndex - 1 + items.Count) % items.Count;
        RefreshUI();
        Debug.Log($"选中: {CurrentItem}");
    }

    private void RefreshUI()
    {
        UIManager.Instance?.RefreshHotbar();
    }
}