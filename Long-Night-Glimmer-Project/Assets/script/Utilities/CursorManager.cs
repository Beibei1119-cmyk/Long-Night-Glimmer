using UnityEngine;
using System.Collections.Generic;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("鼠标图片")]
    public Texture2D normalCursor;
    public Texture2D hoverCursor;

    [Header("鼠标热点（点击位置）")]
    public Vector2 hotSpot = Vector2.zero;

    private bool isHovering = false;

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
        SetNormalCursor();
    }

    private void Update()
    {
        // 从鼠标位置发射射线，检测可交互物体
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero);

        bool canInteract = false;

        if (hit.collider != null)
        {
            // 检查物体是否有可交互组件
            if (hit.collider.GetComponent<InteractableObject>() != null ||
                hit.collider.GetComponent<NPC>() != null ||
                hit.collider.GetComponent<PickupItem>() != null ||
                hit.collider.GetComponent<DragableItem>() != null)
            {
                canInteract = true;
            }
        }

        // 更新鼠标样式
        if (canInteract && !isHovering)
        {
            SetHoverCursor();
            isHovering = true;
        }
        else if (!canInteract && isHovering)
        {
            SetNormalCursor();
            isHovering = false;
        }
    }

    public void SetNormalCursor()
    {
        if (normalCursor != null)
            Cursor.SetCursor(normalCursor, hotSpot, CursorMode.Auto);
    }

    public void SetHoverCursor()
    {
        if (hoverCursor != null)
            Cursor.SetCursor(hoverCursor, hotSpot, CursorMode.Auto);
    }
}