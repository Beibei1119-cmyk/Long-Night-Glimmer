using UnityEngine;
using Cinemachine;

public class CameraDragController : MonoBehaviour
{
    [Header("摄像机设置")]
    public CinemachineVirtualCamera virtualCamera;
    public Transform player;
    public Camera mainCamera;

    [Header("拖拽设置")]
    public float dragSensitivity = 0.5f;

    [Header("边界设置")]
    public string backgroundTag = "Background";

    private CinemachineFramingTransposer transposer;
    private Vector3 dragOrigin;
    private Vector3 targetCameraOffset;
    private bool isDragging = false;
    private bool lastIsMoving = false;

    // 记录摄像机相对于玩家的偏移
    private Vector3 currentOffset = Vector3.zero;

    void Start()
    {
        if (virtualCamera == null)
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (mainCamera == null)
            mainCamera = Camera.main;

        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        // 保存初始偏移
        if (transposer != null)
        {
            targetCameraOffset = transposer.m_TrackedObjectOffset;
            currentOffset = targetCameraOffset;
        }
    }

    void Update()
    {
        player playerScript = player.GetComponent<player>();
        bool isMoving = playerScript != null && playerScript.IsMoving();

        // 人物开始移动 → 重置偏移
        if (isMoving && !lastIsMoving)
        {
            ResetCameraOffset();
        }

        lastIsMoving = isMoving;

        // 只有人物静止时才处理拖拽
        if (!isMoving)
        {
            HandleDrag();
        }

        // 平滑应用偏移
        if (transposer != null)
        {
            currentOffset = Vector3.Lerp(currentOffset, targetCameraOffset, Time.deltaTime * 10f);
            transposer.m_TrackedObjectOffset = currentOffset;
        }
    }

    void HandleDrag()
    {
        // 鼠标右键按下
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
        }

        // 鼠标右键拖拽中
        if (Input.GetMouseButton(1) && isDragging)
        {
            Vector3 delta = Input.mousePosition - dragOrigin;

            // 将屏幕坐标的移动转换为世界坐标偏移
            float dragX = delta.x * dragSensitivity * 0.01f;
            float dragY = delta.y * dragSensitivity * 0.01f;

            // 累加偏移
            Vector3 newOffset = targetCameraOffset;
            newOffset.x -= dragX;  // 注意方向：鼠标右移，相机左移
            newOffset.y -= dragY;

            // 应用边界限制
            newOffset = ClampOffsetWithBackground(newOffset);

            targetCameraOffset = newOffset;
            dragOrigin = Input.mousePosition;
        }

        // 鼠标右键松开
        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }
    }

    void ResetCameraOffset()
    {
        targetCameraOffset = Vector3.zero;
        currentOffset = Vector3.zero;
        if (transposer != null)
            transposer.m_TrackedObjectOffset = Vector3.zero;
    }

    Vector3 ClampOffsetWithBackground(Vector3 offset)
    {
        // 获取当前背景边界
        GameObject bgObj = GameObject.FindGameObjectWithTag(backgroundTag);
        if (bgObj == null)
            bgObj = GameObject.Find("背景2");

        if (bgObj == null || player == null || mainCamera == null)
            return offset;

        SpriteRenderer sr = bgObj.GetComponent<SpriteRenderer>();
        if (sr == null)
            return offset;

        Bounds bgBounds = sr.bounds;

        // 摄像机可视范围
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        // 玩家位置
        Vector3 playerPos = player.position;

        // 计算摄像机在玩家偏移后的实际位置
        Vector3 cameraPos = playerPos + new Vector3(offset.x, offset.y, 0);

        // 计算允许的摄像机范围
        float minX = bgBounds.min.x + camWidth;
        float maxX = bgBounds.max.x - camWidth;
        float minY = bgBounds.min.y + camHeight;
        float maxY = bgBounds.max.y - camHeight;

        // 如果背景比摄像机小，居中显示
        if (minX > maxX)
        {
            float centerX = (bgBounds.min.x + bgBounds.max.x) / 2f;
            offset.x = centerX - playerPos.x;
        }
        else
        {
            // 限制摄像机位置
            float clampedCamX = Mathf.Clamp(cameraPos.x, minX, maxX);
            offset.x = clampedCamX - playerPos.x;
        }

        if (minY > maxY)
        {
            float centerY = (bgBounds.min.y + bgBounds.max.y) / 2f;
            offset.y = centerY - playerPos.y;
        }
        else
        {
            float clampedCamY = Mathf.Clamp(cameraPos.y, minY, maxY);
            offset.y = clampedCamY - playerPos.y;
        }

        return offset;
    }

    // 供外部调用，场景切换时刷新
    public void RefreshBounds()
    {
        ResetCameraOffset();
    }
}