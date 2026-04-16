using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateManager : MonoBehaviour
{
    public static SceneStateManager Instance;

    // 存储每个场景中箱子的状态
    // key: 场景名 + 箱子ID, value: 是否已打开
    private Dictionary<string, bool> openedBoxes = new Dictionary<string, bool>();
    private Dictionary<string, bool> unlockedBoxes = new Dictionary<string, bool>();

    // ========== 新增：UI物品状态 ==========
    private Dictionary<string, bool> pickedUpItems = new Dictionary<string, bool>();
    
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

    private void Start()
    {
        // 监听场景加载完成事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[OnSceneLoaded] 场景加载完成: {scene.name}");
        StartCoroutine(RestoreAllBoxesDelayed(scene.name));
        // UI物品使用固定场景名 "Persistent"
        StartCoroutine(RestoreUIItemsDelayed("PersistentScene"));
    }

    private System.Collections.IEnumerator RestoreAllBoxesDelayed(string sceneName)
    {
        yield return null; // 等待一帧
        RestoreAllBoxes(sceneName);
    }

    // ========== 箱子状态保存/恢复 ==========

    public void SaveBoxState(string sceneName, string boxId, bool isOpen, bool isLocked)
    {
        string key = $"{sceneName}_{boxId}";
        openedBoxes[key] = isOpen;
        unlockedBoxes[key] = !isLocked;

    }

    // 获取箱子状态
    public bool IsBoxOpen(string sceneName, string boxId)
    {
        string key = $"{sceneName}_{boxId}";
        return openedBoxes.ContainsKey(key) && openedBoxes[key];
    }

    public bool IsBoxUnlocked(string sceneName, string boxId)
    {
        string key = $"{sceneName}_{boxId}";
        return unlockedBoxes.ContainsKey(key) && unlockedBoxes[key];
    }

    // 恢复所有箱子


    private void RestoreAllBoxes(string sceneName)
    {
        InteractableObject[] allBoxes = FindObjectsOfType<InteractableObject>();

        foreach (var box in allBoxes)
        {
            string boxId = box.gameObject.name;

            // 使用传入的场景名，而不是 SceneManager.GetActiveScene().name
            if (IsBoxOpen(sceneName, boxId))
            {
                box.SetOpenState(true);
            }

            if (IsBoxUnlocked(sceneName, boxId))
            {
                box.SetUnlockedState(true);
            }
        }
    }

    // ========== UI物品状态保存/恢复 ==========
    public void SaveUIItemState(string sceneName, string itemId, bool isPickedUp)
    {
        string key = $"{sceneName}_{itemId}";
        pickedUpItems[key] = isPickedUp;
    }

    public bool IsUIItemPickedUp(string sceneName, string itemId)
    {
        string key = $"{sceneName}_{itemId}";
        return pickedUpItems.ContainsKey(key) && pickedUpItems[key];
    }

    private IEnumerator RestoreUIItemsDelayed(string sceneName)
    {
        yield return null;
        RestoreUIItems(sceneName);
    }

    private void RestoreUIItems(string sceneName)
    {
        // 只打印日志，不做实际隐藏/显示
        // 实际显示/隐藏由 InsidePanel.Show() 控制
        Debug.Log($"[RestoreUIItems] 场景: {sceneName}, 已保存 {pickedUpItems.Count} 条记录");
    }
}