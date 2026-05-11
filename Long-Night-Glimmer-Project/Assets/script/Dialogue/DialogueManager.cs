using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI组件")]
    public GameObject dialoguePanel;

    // 对话内容
    public Text contentText;

    // 左人物
    public GameObject faceLeft;
    public Text nameLeft;

    // 右人物
    public GameObject faceRight;
    public Text nameRight;

    // 按钮
    public Button leftButton;
    public Button rightButton;
    public Button closeButton;  // 新增：关闭按钮

    [Header("其他UI")]
    public GameObject mainCanvas;

    private List<DialogueData> currentDialogue = new List<DialogueData>();
    private int currentIndex = 0;
    private bool isActive = false;

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
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (leftButton != null)
            leftButton.onClick.AddListener(PreviousDialogue);

        if (rightButton != null)
            rightButton.onClick.AddListener(NextDialogue);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseDialogue);
    }

    public void StartDialogue(List<DialogueData> dialogue)
    {
        currentDialogue = dialogue;
        currentIndex = 0;
        isActive = true;
        ShowCurrentLine();
        dialoguePanel.SetActive(true);
        UpdateButtonState();

        if (mainCanvas != null)
            mainCanvas.SetActive(false);
    }

    private void ShowCurrentLine()
    {
        if (currentIndex >= 0 && currentIndex < currentDialogue.Count)
        {
            DialogueData line = currentDialogue[currentIndex];

            if (line.isLeft)
            {
                if (faceLeft != null) faceLeft.SetActive(true);
                if (faceRight != null) faceRight.SetActive(false);
                if (nameLeft != null) nameLeft.text = line.speakerName;
            }
            else
            {
                if (faceLeft != null) faceLeft.SetActive(false);
                if (faceRight != null) faceRight.SetActive(true);
                if (nameRight != null) nameRight.text = line.speakerName;
            }

            if (contentText != null)
                contentText.text = line.content;
        }
    }

    public void NextDialogue()
    {
        if (currentIndex < currentDialogue.Count - 1)
        {
            currentIndex++;
            ShowCurrentLine();
            UpdateButtonState();
        }
    }

    public void PreviousDialogue()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowCurrentLine();
            UpdateButtonState();
        }
    }

    public void CloseDialogue()
    {
        isActive = false;
        dialoguePanel.SetActive(false);

        if (mainCanvas != null)
            mainCanvas.SetActive(true);
    }

    private void UpdateButtonState()
    {
        if (leftButton != null)
            leftButton.interactable = currentIndex > 0;
        if (rightButton != null)
            rightButton.interactable = currentIndex < currentDialogue.Count - 1;
    }

    public bool IsActive()
    {
        return isActive;
    }
}