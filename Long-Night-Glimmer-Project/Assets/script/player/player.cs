using System.Collections;
using System.Collections.Generic;
using System.Threading;
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


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();


    }

    private void Update()
    {

        PlayerInput();//移动输入
        SwitchAnimation();//切换动画
    }
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
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
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