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
    public GameObject stoneItem;  // 铁钥匙 
    public GameObject boardItem;    // 木板 ← 新增

    public Button closeButton;      // 关闭按钮（还是按钮）

    [Header("背景图")]
    public Image backgroundImage;  // 拖入 BackgroundImage

    [Header("音效")]
    public AudioClip openSound;     // 打开面板音效
    public AudioClip closeSound;    // 关闭面板音效
    public AudioClip itemPickupSound; // 拾取物品音效


    private AudioSource audioSource;

    private void Awake()
    {
        // 在 Awake 中初始化，确保最早执行
        InitAudioSource();
    }
    private void InitAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.volume = 0.8f;
        audioSource.enabled = true;
    }
    private void Start()
    {
        //有这个代码则不需要手动关闭那个ui，没有的话则需要手动关闭哈。
        //gameObject.SetActive(false);
        // 给物品添加点击拾取功能
        // 初始化 AudioSource
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = 0.8f;
        }

        // ========== 强制启用 AudioSource ==========
        audioSource.playOnAwake = false;
        audioSource.volume = 0.8f;
        audioSource.enabled = true;  // ← 关键：确保启用
        // =========================================


        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

    }

    //// 统一播放音效的方法
    //private void PlaySound(AudioClip clip)
    //{
    //    if (clip != null && audioSource != null)
    //    {
    //        audioSource.PlayOneShot(clip);
    //    }
    //}

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

        // 添加 UI 点击脚本，并设置拾取音效
        UIItemClick click = item.GetComponent<UIItemClick>();
        if (click == null)
            click = item.AddComponent<UIItemClick>();
        click.itemName = itemName;
        click.pickupSound = itemPickupSound;  // 传递音效引用
    }



    public void Show(Sprite bgImage, bool showKey, bool showClip, bool showKey2, bool showGem1, bool showGem2, bool showstone, bool showBoard)
    {
        //// 播放打开音效
        //PlaySound(openSound);
        // 设置背景图
        if (backgroundImage != null && bgImage != null)
            backgroundImage.sprite = bgImage;

        // 获取当前场景名
        string sceneName = "PersistentScene";


        // ========== 添加调试日志 ==========
        Debug.Log($"=== InsidePanel.Show 调试 ===");
        Debug.Log($"showKey={showKey}, showClip={showClip}, showKey2={showKey2}, showGem1={showGem1}, showGem2={showGem2}");

        // 获取拾取状态
        bool isKeyPickedUp = SceneStateManager.Instance.IsUIItemPickedUp(sceneName, "KeyItem");
        bool isClipPickedUp = SceneStateManager.Instance.IsUIItemPickedUp(sceneName, "ClipItem");
        bool isKey2PickedUp = SceneStateManager.Instance.IsUIItemPickedUp(sceneName, "Key2Item");
        bool isGem1PickedUp = SceneStateManager.Instance.IsUIItemPickedUp(sceneName, "Gem1Item");
        bool isGem2PickedUp = SceneStateManager.Instance.IsUIItemPickedUp(sceneName, "Gem2Item");
        bool isstonePickedUp = SceneStateManager.Instance.IsUIItemPickedUp(sceneName, "stoneItem"); 
        bool isBoardPickedUp = SceneStateManager.Instance.IsUIItemPickedUp(sceneName, "BoardItem"); // 新增

        Debug.Log($"KeyItem 是否已拾取: {isKeyPickedUp}");
        Debug.Log($"ClipItem 是否已拾取: {isClipPickedUp}");
        Debug.Log($"Key2Item 是否已拾取: {isKey2PickedUp}");
        Debug.Log($"Gem1Item 是否已拾取: {isGem1PickedUp}");
        Debug.Log($"Gem2Item 是否已拾取: {isGem2PickedUp}");

        // =================================

        // 根据保存的状态决定是否显示物品
        bool keyShouldShow = showKey && !isKeyPickedUp;
        bool clipShouldShow = showClip && !isClipPickedUp;
        bool key2ShouldShow = showKey2 && !isKey2PickedUp;
        bool gem1ShouldShow = showGem1 && !isGem1PickedUp;
        bool gem2ShouldShow = showGem2 && !isGem2PickedUp;
        bool stoneShouldShow = showstone && !isstonePickedUp; 
        bool boardShouldShow = showBoard && !isBoardPickedUp; // 新增

        Debug.Log($"最终显示: key={keyShouldShow}, clip={clipShouldShow}, key2={key2ShouldShow}, gem1={gem1ShouldShow}, gem2={gem2ShouldShow}");

        if (keyItem != null) keyItem.SetActive(keyShouldShow);
        if (clipItem != null) clipItem.SetActive(clipShouldShow);
        if (key2Item != null) key2Item.SetActive(key2ShouldShow);
        if (gem1Item != null) gem1Item.SetActive(gem1ShouldShow);
        if (gem2Item != null) gem2Item.SetActive(gem2ShouldShow);
        if (stoneItem != null) stoneItem.SetActive(stoneShouldShow); 
        if (boardItem != null) boardItem.SetActive(boardShouldShow); // 新增

        gameObject.SetActive(true);
    }
    public void Hide()
    {
        //// 播放关闭音效
        //PlaySound(closeSound);
        gameObject.SetActive(false);
    }


  
}