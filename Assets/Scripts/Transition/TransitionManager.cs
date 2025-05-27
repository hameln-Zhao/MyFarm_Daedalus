using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyFarm.Transition
{
    public class Transition : Singleton<Transition>
    {
        [SceneName]
        public string StartSceneName = string.Empty;
        private CanvasGroup fadeCanvasGroup;
        private bool isFade;
        private IEnumerator Start()
        { 
            fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
            yield return  LoadSceneActive(StartSceneName);
            EventHandler.CallAfterSceneEvent();
        }

        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }

        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }
        

        private void OnTransitionEvent(string sceneName, Vector3 Pos)
        {
            if (!isFade)
            {
                StartCoroutine(TransitionScene(sceneName, Pos));
            }
            
        }
        /// <summary>
        /// 传送到目标场景的目标位置
        /// </summary>
        /// <param name="SceneName"></param>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        private IEnumerator TransitionScene(string SceneName, Vector3 targetPos)
        {
            EventHandler.CallBeforeSceneEvent();
            yield return Fade(1);
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            yield return LoadSceneActive(SceneName);
            EventHandler.CallMoveToPosition(targetPos); 
            EventHandler.CallAfterSceneEvent();
            yield return Fade(0);
        }

        /// <summary>
        /// 加载场景并激活
        /// </summary>
        /// <param name="SceneName"></param>
        /// <returns></returns>
        private IEnumerator LoadSceneActive(string SceneName)
        {
            yield return SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive); //在原有场景附加
            //直到上面的代码执行完 下面的才会执行
            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            SceneManager.SetActiveScene(newScene); //设置场景激活
        }
        /// <summary>
        /// 淡入淡出场景   1是往黑 0是往透明
        /// </summary>
        /// <param name="targetAlpha"></param>
        /// <returns></returns>
        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;
            fadeCanvasGroup.blocksRaycasts = true;//切换的时候遮挡点击
            float speed = Mathf.Abs( fadeCanvasGroup.alpha-targetAlpha)/Settings.fadeCanvasDuration;
            while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
            {
                fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed*Time.deltaTime);
                yield return null;
            }
            fadeCanvasGroup.blocksRaycasts = false;
            isFade = false;
        }
    }
}
