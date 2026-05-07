using UnityEngine;

public class OpenBook : MonoBehaviour
{
    private GameObject bookObject;

    private void Start()
    {
        // 查找所有 BookPro 类型的物体（包括隐藏的）
        BookPro[] allBookPros = FindObjectsOfType<BookPro>(true);

        if (allBookPros.Length > 0)
        {
            bookObject = allBookPros[0].gameObject;
            Debug.Log($"找到 BookPro: {bookObject.name}, 是否激活: {bookObject.activeSelf}");
        }
        else
        {
            Debug.LogError("找不到 BookPro 组件！");
        }

        // 强制把背景置底
        Transform bg = transform.Find("Background");
        if (bg != null)
            bg.SetSiblingIndex(0);
    }

    private void OnMouseDown()
    {
        if (bookObject != null)
        {
            bookObject.SetActive(true);
            UIManager.Instance.ShowHint("打开了日历");
        }
    }



}