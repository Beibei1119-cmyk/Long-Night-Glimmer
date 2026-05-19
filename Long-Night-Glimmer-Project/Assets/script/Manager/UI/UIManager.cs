using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    //[Header("拖拽图片")]
    //public Sprite keyDragSprite;  // 钥匙拖拽时的图片
    //public Sprite gemDragSprite;  // 发夹拖拽时的图片

    [Header("音效")]
    public AudioClip buttonClickSound;  // 按钮点击音效
    private AudioSource audioSource;    // 音效播放器

    [Header("内部面板")]
    public InsidePanel insidePanel;  // 改成 InsidePanel 类型

    [Header("提示面板")]
    public GameObject hintPanel;
    public Text hintText;

    [Header("详情面板")]
    public GameObject detailPanel;
    public Image detailImage;
    public Text detailText;

    [Header("快捷栏")]
    public Image hotbarIcon;
    public Text hotbarName;
    public Button leftButton;
    public Button rightButton;

    public Sprite defaultIcon;

    // ========== 密码面板 ==========
    [Header("密码面板")]
    public PasswordPanel passwordPanel;
    // =================================



    //[Header("组合锁面板")]
    //public GameObject comboLockPanel;  // 拖入 ComboLockPanel_BoxA
    [Header("组合锁面板")]
    public ComboLockPanel comboLockPanel;  // 改成 ComboLockPanel 类型，不是 GameObject
    public ComboLockPanel ComboLockPanel_BoxB;
    public ComboLockPanel comboLockPanel_BoxC;  
    public ComboLockPanel comboLockPanel_BoxD;  // 新增


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 初始化音效播放器
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.volume = 0.8f;
            }

            // ========== 强制设置这些属性 ==========
            audioSource.playOnAwake = false;
            audioSource.volume = 1f;
            audioSource.mute = false;      // 确保没被静音
            audioSource.enabled = true;     // 确保启用
           // ===================================

        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ========== 全局点击音效方法 ==========
    public void PlayButtonClick()
    {
        Debug.Log($"PlayButtonClick 被调用 - 音效文件: {buttonClickSound?.name}, AudioSource: {audioSource != null}");

        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
            Debug.Log("音效应该正在播放");
        }
        else
        {
            if (buttonClickSound == null) Debug.LogError("buttonClickSound 为空！请在 Inspector 中拖入音效文件");
            if (audioSource == null) Debug.LogError("audioSource 为空！");
        }
    }
    // ===================================


    private void Start()
    {
        if (hintPanel != null) hintPanel.SetActive(false);

        if (leftButton != null)
        {
            leftButton.onClick.AddListener(() => {
                PlayButtonClick();  // 添加这行
                InventoryManager.Instance.SelectPrevious();
            });
        }

        if (rightButton != null)
        {
            rightButton.onClick.AddListener(() => {
                PlayButtonClick();  // 添加这行
                InventoryManager.Instance.SelectNext();
            });
        }

        RefreshHotbar();
    }



    public void ShowHint(string message)
    {
        hintText.text = message;
        hintPanel.SetActive(true);
        CancelInvoke(nameof(HideHint));
        Invoke(nameof(HideHint), 2f);
    }

    public void ShowDetail(Sprite itemImage, string description)
    {
        detailImage.sprite = itemImage;
        detailText.text = description;
        detailPanel.SetActive(true);
    }

    public void HideDetail()
    {
        detailPanel.SetActive(false);
    }


    private void HideHint()
    {
        hintPanel.SetActive(false);
    }

    //============组合锁====================
    //public void ShowComboLockPanel(InteractableObject target)
    //{
    //    if (comboLockPanel != null)
    //    {
    //        comboLockPanel.SetActive(true);
    //        // 后续会把 target 传给面板
    //    }
    //}

    public void ShowComboLockPanel(InteractableObject target)
    {
        PlayButtonClick();  // 添加音效
        if (comboLockPanel != null)
        {
            comboLockPanel.Open(target);  // 调用 Open 方法传递箱子
        }
    }

    public void ShowComboLockPanel_BoxB(InteractableObject target)
    {
        PlayButtonClick();  // 添加音效
        if (ComboLockPanel_BoxB != null)
            ComboLockPanel_BoxB.Open(target);
    }
    //============================================================

    public void ShowComboLockPanel_BoxC(InteractableObject target)
    {
        PlayButtonClick();  // 添加音效
        if (comboLockPanel_BoxC != null)
            comboLockPanel_BoxC.Open(target);
    }


    public void ShowComboLockPanel_BoxD(InteractableObject target)
    {
        PlayButtonClick();  // 添加音效
        if (comboLockPanel_BoxD != null)
            comboLockPanel_BoxD.Open(target);
    }



    public void RefreshHotbar()
    {

        if (hotbarIcon == null || hotbarName == null)
        {
            Debug.LogWarning("[UIManager] hotbarIcon 或 hotbarName 未赋值");
            return;
        }

        string currentItem = InventoryManager.Instance.CurrentItem;
      

        if (string.IsNullOrEmpty(currentItem))
        {
          

            // ========== 清空显示 ==========
            hotbarIcon.sprite = null;
            hotbarIcon.color = new Color(1, 1, 1, 0);
            hotbarName.text = "";

            // 移除拖拽脚本
            DragableItem drag = hotbarIcon.GetComponent<DragableItem>();
            if (drag != null)
            {
            
                Destroy(drag);
            }
            // ============================
        }
        else
        {
          

            // ========== 显示物品 ==========
            hotbarIcon.color = new Color(1, 1, 1, 1);
            hotbarName.text = currentItem;

            // 加载图标
            Sprite icon = Resources.Load<Sprite>($"Icons/{currentItem}");
   

            if (icon != null)
            {
                hotbarIcon.sprite = icon;
            }
            else
            {
                hotbarIcon.sprite = null;
                hotbarIcon.color = new Color(1, 0.5f, 0.5f, 1);
            }

            // ========== 先移除旧的拖拽脚本，再添加新的 ==========
            DragableItem oldDrag = hotbarIcon.GetComponent<DragableItem>();
            if (oldDrag != null)
            {
               
                Destroy(oldDrag);
            }

            DragableItem newDrag = hotbarIcon.gameObject.AddComponent<DragableItem>();
            newDrag.itemName = currentItem;
  
            // ========================================================
        }

    }


    private void SetItemIcon(string itemName)
    {
        hotbarIcon.sprite = Resources.Load<Sprite>($"Icons/{itemName}");
    }


    // ========== 显示密码面板 ==========
    public void ShowPasswordPanel(InteractableObject target)
    {
        PlayButtonClick();  // 添加音效
        Debug.Log($"ShowPasswordPanel 被调用, target={target?.name}");
        if (passwordPanel != null)
        {
            passwordPanel.OpenPanel(target);
        }
        else
        {
            Debug.LogError("PasswordPanel 未赋值！");
        }
    }
    // =================================

    //public void ShowInsidePanel(Sprite bgImage, bool showKey, bool showClip)
    //{
    //    if (insidePanel != null)
    //        insidePanel.Show(bgImage, showKey, showClip);
    //}
    public void ShowInsidePanel(Sprite bgImage, bool showKey, bool showClip, bool showKey2, bool showGem1, bool showGem2, bool showstone, bool showBoard)
    {
        if (insidePanel != null)
            insidePanel.Show(bgImage, showKey, showClip, showKey2, showGem1, showGem2, showstone, showBoard);
    }

    public void HideInsidePanel()
    {
        if (insidePanel != null)
            insidePanel.Hide();
    }


    public void ReturnToMainMenu()
    {
        StartCoroutine(ReturnToMainMenuCoroutine());
    }

    private IEnumerator ReturnToMainMenuCoroutine()
    {
        // ========== 隐藏场景五的音乐物体 ==========
        GameObject audioManager = GameObject.Find("AudioManager");
        if (audioManager != null)
        {
            audioManager.SetActive(false);  // 隐藏
                                            // 或者 Destroy(audioManager); // 销毁
        }
        // ======================================

        yield return null;

        string currentScene = SceneManager.GetActiveScene().name;

        yield return SceneManager.UnloadSceneAsync(currentScene);

        SceneManager.LoadScene("MainMenu");

        Debug.Log($"卸载 {currentScene}，返回主菜单");

    }
}