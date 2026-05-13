using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using System.Collections;

public class OpeningManager : MonoBehaviour
{
    public static OpeningManager Instance;

    [Header("Timeline")]
    public PlayableDirector openingTimeline;

    [Header("相机")]
    public CinemachineVirtualCamera normalCamera;  // CM vcam1

    [Header("玩家")]
    public GameObject player;
    public Animator playerAnimator;

    [Header("UI")]
    public GameObject mainCanvas;  // 新增：主界面 Canvas
    public GameObject skipButton;  // 跳过按钮（可选）


    [Header("相机设置")]
    public float finalOrthographicSize = 5.13f;  // 最终相机大小，在 Inspector 中设置


    private bool hasPlayed = false;
    private bool isPlaying = false;

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
        //===========================
        PlayerPrefs.DeleteAll();  // 清除所有保存数据
        //=============================

        // 检查是否已经播放过开场动画
        if (PlayerPrefs.GetInt("OpeningPlayed", 0) == 1)
        {
            hasPlayed = true;
            StartGameNormally();
        }
        else
        {
            StartCoroutine(WaitForSceneLoad());
        }
    }

    private IEnumerator WaitForSceneLoad()
    {
        yield return new WaitForSeconds(0.5f);

        if (!hasPlayed)
        {
            PlayOpening();
        }
    }

    //public void PlayOpening()
    //{
    //    if (isPlaying) return;
    //    isPlaying = true;
    //    hasPlayed = true;

    //    Debug.Log("=== 开始播放开场动画 ===");

    //    // 禁用玩家控制
    //    DisablePlayerControl();

    //    // 播放 Timeline
    //    if (openingTimeline != null)
    //    {
    //        openingTimeline.stopped += OnTimelineFinished;
    //        openingTimeline.Play();
    //        Debug.Log("Timeline 开始播放");
    //    }
    //    else
    //    {
    //        Debug.LogError("OpeningTimeline 未设置！");
    //        EndOpening();
    //    }

    //    // 记录已播放
    //    PlayerPrefs.SetInt("OpeningPlayed", 1);
    //    PlayerPrefs.Save();
    //}

    //====================================
    public void PlayOpening()
    {
        // 显示跳过按钮
        if (skipButton != null)
            skipButton.SetActive(true);

        if (isPlaying) return;
        isPlaying = true;
        hasPlayed = true;

        Debug.Log("=== 开始播放开场动画 ===");

        // ========== 隐藏主界面 ==========
        if (mainCanvas != null)
            mainCanvas.SetActive(false);



        DisablePlayerControl();

        if (openingTimeline != null)
        {
            Debug.Log($"Timeline 初始状态: time={openingTimeline.time}, duration={openingTimeline.duration}, state={openingTimeline.state}");

            openingTimeline.stopped += OnTimelineFinished;
            openingTimeline.Play();

            Debug.Log($"Timeline Play() 后: time={openingTimeline.time}, state={openingTimeline.state}");
        }
        else
        {
            Debug.LogError("OpeningTimeline 未设置！");
            EndOpening();
        }

        PlayerPrefs.SetInt("OpeningPlayed", 1);
        PlayerPrefs.Save();
    }

    private void Update()
    {
        // 按 ESC 键跳过
        if (isPlaying && Input.GetKeyDown(KeyCode.Escape))
        {
            SkipOpening();
        }
    }
    public void SkipOpening()
    {
        if (!isPlaying) return;

        Debug.Log("=== 跳过开场动画 ===");


        // 先关对话
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive())
        {
            DialogueManager.Instance.CloseDialogue();
        }


        if (openingTimeline != null)
        {
            openingTimeline.Stop();
            openingTimeline.time = openingTimeline.duration;
        }

        if (openingTimeline != null)
        {
            openingTimeline.Stop();
            openingTimeline.time = openingTimeline.duration;
        }

        EndOpening();
    }

    //===============================
    private void OnTimelineFinished(PlayableDirector director)
    {
         Debug.Log($"Timeline 结束事件触发，time={director.time}, state={director.state}");

        openingTimeline.stopped -= OnTimelineFinished;



        Debug.Log("Timeline 播放完成");
        EndOpening();
    }

    private void EndOpening()
    {
        Debug.Log("=== 开场动画结束 ===");
        // 保险：强制关闭对话面板
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive())
        {
            DialogueManager.Instance.CloseDialogue();
        }


        // ========== 恢复相机到指定大小 ==========
        if (normalCamera != null)
        {
            var lens = normalCamera.m_Lens;
            lens.OrthographicSize = finalOrthographicSize;
            normalCamera.m_Lens = lens;
        }
        // =================================


        // 隐藏跳过按钮
        if (skipButton != null)
            skipButton.SetActive(false);

        // ========== 恢复显示主界面 ==========
        if (mainCanvas != null)
            mainCanvas.SetActive(true);



        // 重置相机优先级
        if (normalCamera != null)
            normalCamera.Priority = 10;

        // 重置边界
        StartCoroutine(ResetBounds());

        // 恢复玩家控制
        EnablePlayerControl();

 // ========== 新增：重置拖拽控制器状态 ==========
    CameraDragController dragController = FindObjectOfType<CameraDragController>();
    if (dragController != null)
    {
        dragController.ResetDragState();
        Debug.Log("拖拽控制器已重置");
    }
    // ===========================================


        isPlaying = false;
    }

    private IEnumerator ResetBounds()
    {
        yield return null;

        // 重置 SwitchBounds
        SwitchBounds switchBounds = FindObjectOfType<SwitchBounds>();
        if (switchBounds != null)
        {
            Debug.Log("重置 SwitchBounds 边界");
            switchBounds.SwitchConfinerShape();
        }

        // 刷新拖拽边界
        CameraDragController drag = FindObjectOfType<CameraDragController>();
        if (drag != null)
        {
            Debug.Log("刷新 CameraDragController 边界");
            drag.RefreshBounds();
        }

        yield return null;
    }

    private void DisablePlayerControl()
    {
        if (player != null)
        {
            player.GetComponent<player>().enabled = false;
            //if (playerAnimator != null)
            //    playerAnimator.SetBool("isGettingUp", true);
            Debug.Log("玩家控制已禁用");
        }
    }

    private void EnablePlayerControl()
    {
        if (player != null)
        {
            player.GetComponent<player>().enabled = true;
            if (playerAnimator != null)
            {
                //playerAnimator.SetBool("isGettingUp", false);
                playerAnimator.SetFloat("InputX", 0);
                playerAnimator.SetFloat("InputY", -1);
            }
            Debug.Log("玩家控制已恢复");
        }
    }

    private void StartGameNormally()
    {
        Debug.Log("开场动画已播放过，直接进入游戏");
        if (normalCamera != null)
            normalCamera.Priority = 10;
        if (player != null)
            player.GetComponent<player>().enabled = true;
    }

    // 供 Signal 调用的方法（可选，用于 Timeline 中触发对话）
    public void OnStartDialogue()
    {
        Debug.Log("Timeline 触发对话");
        // 这里可以启动对话
    }
}