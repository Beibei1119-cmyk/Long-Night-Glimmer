using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


namespace Transition
{

    public class TransitionManager : MonoBehaviour
    {
        public string startSceneName = string.Empty;


        [Header("场景过渡文字")]
        public List<SceneTransitionText> sceneTransitionTexts = new List<SceneTransitionText>();



        private void OnEnable()
        {
            // 监听事件
            EventHandler.TransitionEvent += OnTransitionEvent;
        }

        private void OnDisable()
        {
            // 取消监听
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }

        private void Start()
        {
            StartCoroutine(LoadSceneSetActive(startSceneName));
        }

        // 接收到事件时调用

        private void OnTransitionEvent(string sceneToGo, Vector3 positionToGo)
        {
                StartCoroutine(Transition(sceneToGo, positionToGo));
        }


        /// <summary>
        /// 场景切换
        /// </summary>
        /// <param name="sceneName">目标位置</param>
        /// <param name="targetPosition">目标场景</param>
        /// <returns></returns>
        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {

            // 改成
            string displayText = GetTransitionText(sceneName);

            // 淡出并显示文字
            yield return StartCoroutine(FadeManager.Instance.FadeOutWithTextCoroutine(displayText));

            EventHandler.CallBeforeSceneUnloadEvent();
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            yield return LoadSceneSetActive(sceneName);
            EventHandler.CallMoveToPosition(targetPosition);
            EventHandler.CallAfterSceneLoadEvent();

            // 隐藏文字并淡入
            yield return StartCoroutine(FadeManager.Instance.FadeInWithTextCoroutine());


        }


        /// <summary>
        /// 加载场景并设置为激活
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            SceneManager.SetActiveScene(newScene);


            // 第一个场景激活后也获取到边界
            yield return null;
            SwitchBounds switchBounds = FindObjectOfType<SwitchBounds>();
            if (switchBounds != null) switchBounds.SwitchConfinerShape();
        }


        private string GetTransitionText(string sceneName)
        {
            foreach (var item in sceneTransitionTexts)
            {
                if (item.sceneName == sceneName)
                    return item.transitionText;
            }
            return $"进入 {sceneName}";
        }

        [System.Serializable]
        public class SceneTransitionText
        {
            public string sceneName;
            public string transitionText;
        }

    }

}