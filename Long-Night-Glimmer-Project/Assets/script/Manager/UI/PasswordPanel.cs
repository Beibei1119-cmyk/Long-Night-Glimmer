using UnityEngine;
using UnityEngine.UI;

public class PasswordPanel : MonoBehaviour
{
    [Header("布局")]
    public GameObject digitalLayout;

    [Header("UI组件")]
    public Text passwordDisplay;

    private string currentInput = "";        
    private int maxLength = 4;                     
    private InteractableObject currentTarget; 


    void Start()
    {
        //有这个代码则不需要手动关闭那个ui，没有的话则需要手动关闭哈。
        //Debug.Log($"PasswordPanel 启动: {gameObject.name}, 实例ID: {GetInstanceID()}");
        //gameObject.SetActive(false);
    }

    public void OpenPanel(InteractableObject target)
    {
        //Debug.Log($"OpenPanel 被调用, target={target?.name}");
        currentTarget = target;
        currentInput = "";
        UpdateDisplay();

        if (digitalLayout != null)
            digitalLayout.SetActive(true);

        gameObject.SetActive(true);

        // 加上这一行，强制UI立即准备好
        Canvas.ForceUpdateCanvases();
    }

    public void OnNumberClick(string number)
    {
        //Debug.Log($"OnNumberClick 被调用, number={number}, 当前currentInput={currentInput}");

        if (currentInput.Length < maxLength)
        {
            currentInput += number;
            UpdateDisplay();
            //Debug.Log($"添加后 currentInput={currentInput}");
        }
    }

    public void OnDeleteClick()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            UpdateDisplay();
        }
    }

    public void OnConfirmClick()
    {
      
        if (currentTarget == null)
        {
            Debug.LogError("currentTarget 是 null！请检查 OpenPanel 是否被正确调用");
            UIManager.Instance.ShowHint("出错了，请重新点击箱子");
            gameObject.SetActive(false);
            return;
        }

        if (currentTarget.CheckPassword(currentInput))
        {
            currentTarget.Unlock();
            gameObject.SetActive(false);
            UIManager.Instance.ShowHint("解锁成功！");
        }
        else
        {
            Debug.Log("密码错误！");
            currentInput = "";
            UpdateDisplay();
            UIManager.Instance.ShowHint("密码错误！");
        }
    }

    public void OnCloseClick()
    {
        gameObject.SetActive(false);
    }

    private void UpdateDisplay()
    {
        string display = "";
        for (int i = 0; i < currentInput.Length; i++)
        {
            display += "●";
        }
        while (display.Length < maxLength)
        {
            display += "○";
        }
        passwordDisplay.text = display;
    }
}