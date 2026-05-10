using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        // 只需要加载 persistentscene
        // TransitionManager 会自动加载 startSceneName 指定的场景
        SceneManager.LoadScene("persistentscene");
    }

    void QuitGame()
    {
        Application.Quit();
    }
}