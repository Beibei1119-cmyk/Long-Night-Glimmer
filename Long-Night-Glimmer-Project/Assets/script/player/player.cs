using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class player : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    private float inputX;
    private float inputY;
    private Vector2 movementInput;

    private Animator[] animators;
    private bool isMoving;
    private bool inputDisable;

    // ========== 人物移动边界相关 ==========
    private PolygonCollider2D boundsCollider;  // 改用 PolygonCollider2D
    private bool hasBounds = false;
    // ===================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();


    }


    //人物初始状态朝前看--新加
    private void Start()
    {
        // 设置初始朝向（向前/向下）
        foreach (var anim in animators)
        {
            anim.SetFloat("InputX", 0);
            anim.SetFloat("InputY", -1);  // -1 = 向前（朝下）
        }
    }


    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;

    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;

    }

    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    private void OnAfterSceneLoadEvent()
    {
        inputDisable = false;

        // ========== 人物移动边界相关-场景加载后刷新边界 ==========
        UpdatePlayerBounds();
        // ===========================================
    }


    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true;
    }




    private void Update()
    {

        // ==========人物移动边界相关-每帧更新边界 ==========
        UpdatePlayerBounds();
        // =====================================


        if (inputDisable == false)
        {
            PlayerInput();//移动输入
        }
        SwitchAnimation();//切换动画


    }

    // ========== 人物移动边界相关-获取当前场景的人物边界 ==========
    private void UpdatePlayerBounds()
    {
        GameObject boundsObj = GameObject.FindGameObjectWithTag("PlayerBounds");

        if (boundsObj != null)
        {
            boundsCollider = boundsObj.GetComponent<PolygonCollider2D>();
            hasBounds = boundsCollider != null;
        }
        else
        {
            hasBounds = false;
        }
    }

    // ===============================================



    private void PlayerInput()//移动输入
    {
        //if(inputY == 0)
        inputX = Input.GetAxisRaw("Horizontal");
        //if(inputY == 0)
        inputY = Input.GetAxisRaw("Vertical");

        if (inputX != 0 && inputY != 0)
        {
            inputX = inputX * 0.6f;
            inputY = inputY * 0.6f;
        }

        movementInput = new Vector2(inputX, inputY);
        isMoving = movementInput != Vector2.zero;
    }


    private void FixedUpdate()
    {
        Movement();//玩家移动
    }


    private void Movement()//玩家移动
    {
        //rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
        Vector3 newPosition = rb.position + movementInput * speed * Time.deltaTime;

        if (hasBounds && boundsCollider != null)
        {
            // 获取脚底碰撞体
            Collider2D footCol = GetComponent<Collider2D>();
            if (footCol != null)
            {
                Vector2 footOffset = footCol.offset;

                // 计算脚底目标位置
                Vector2 targetFootPos = (Vector2)newPosition + footOffset;

                // 检查脚底是否在多边形内
                if (!boundsCollider.OverlapPoint(targetFootPos))
                {
                    // 超出边界，不移动
                    return;
                }
            }
            else
            {
                // 如果没有脚底碰撞体，检查中心点
                if (!boundsCollider.OverlapPoint(newPosition))
                {
                    return;
                }
            }
        }
        // =====================================
        rb.MovePosition(newPosition);
    }

    private void SwitchAnimation()//切换动画
    {
        foreach (var anim in animators)
        {
            anim.SetBool("isMoving", isMoving);
            //anim.SetFloat("mouseX", mouseX);
            //anim.SetFloat("mouseY", mouseY);

            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }



    }
    
    public bool IsMoving()
    {
        return isMoving;
    }
}