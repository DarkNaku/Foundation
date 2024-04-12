using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DarkNaku.Foundation;

namespace DarkNaku.Director
{
    public class Director : SingletonBehaviour<Director>
    {
        public float MinLoadingTime { get; set; } = 5f;

        private bool _isLoading = false;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            var currentScene = SceneManager.GetActiveScene();
            var currentSceneHandler = Instance.FindComponent<SceneHandler>(currentScene);
            currentSceneHandler?.OnEnter();
        }
        
        public static void Change(string nextSceneName)
        {
            Instance.StartCoroutine(Instance.CoChange<SceneHandler>(nextSceneName, null));
        }
        
        public static void Change(string nextSceneName, string loadingSceneName)
        {
            Instance.StartCoroutine(Instance.CoChange<SceneHandler>(nextSceneName, loadingSceneName, null));
        }
        
        public static void Change<T>(string nextSceneName, Action<T> onLoadScene = null) where T : SceneHandler
        {
            Instance.StartCoroutine(Instance.CoChange(nextSceneName, onLoadScene));
        }

        public static void Change<T>(string nextSceneName, string loadingSceneName, Action<T> onLoadScene = null) where T : SceneHandler
        {
            Instance.StartCoroutine(Instance.CoChange(nextSceneName, loadingSceneName, onLoadScene));
        }
        
        public IEnumerator CoChange<T>(string nextSceneName, Action<T> onLoadScene) where T : SceneHandler
        {
            if (_isLoading) yield break;

            _isLoading = true;

            var currentScene = SceneManager.GetActiveScene();
            var currentEventSystem = GetEventSystemInScene(currentScene);
            var currentSceneHandler = FindComponent<SceneHandler>(currentScene);
            var currentSceneTransition = FindComponent<ISceneTransition>(currentScene);
            var currentLoadingProgress = FindComponent<ILoadingProgress>(currentScene);
            var currentSceneName = currentScene.name;

            var ao = SceneManager.LoadSceneAsync(nextSceneName);

            ao.allowSceneActivation = false;

            if (currentEventSystem != null)
            {
                currentEventSystem.enabled = false;
            }

            var loadingStart = Time.time;
            var progress = 0f;

            while (progress < 0.9f)
            {
                var elapsed = Time.time - loadingStart;
                progress = MinLoadingTime > 0f ? Mathf.Min(elapsed / MinLoadingTime, ao.progress) : ao.progress;
                currentLoadingProgress?.OnProgress(progress);
                yield return null;
            }

            currentLoadingProgress?.OnProgress(1f);

            if (currentSceneTransition != null)
            {
                yield return currentSceneTransition.CoTransitionOut(nextSceneName);
            }

            currentSceneHandler?.OnExit();

            ao.allowSceneActivation = true;

            yield return new WaitUntil(() => ao.isDone);

            var nextScene = SceneManager.GetSceneByName(nextSceneName);
            var nextEventSystem = GetEventSystemInScene(nextScene);
            var nextSceneHandler = FindComponent<T>(nextScene);
            var nextSceneTransition = FindComponent<ISceneTransition>(nextScene);

            if (nextEventSystem != null)
            {
                nextEventSystem.enabled = false;
            }

            onLoadScene?.Invoke(nextSceneHandler);
            nextSceneHandler?.OnEnter();

            if (nextSceneTransition != null)
            {
                yield return nextSceneTransition.CoTransitionIn(currentSceneName);
            }

            if (nextEventSystem != null)
            {
                nextEventSystem.enabled = true;
            }

            _isLoading = false;
        }

        public IEnumerator CoChange<T>(string nextSceneName, string loadingSceneName, Action<T> onLoadScene) where T : SceneHandler
        {
            if (_isLoading) yield break;

            _isLoading = true;

            var currentScene = SceneManager.GetActiveScene();
            var currentEventSystem = GetEventSystemInScene(currentScene);
            var currentSceneHandler = FindComponent<SceneHandler>(currentScene);
            var currentSceneTransition = FindComponent<ISceneTransition>(currentScene);
            var currentSceneName = currentScene.name;

            if (currentEventSystem != null)
            {
                currentEventSystem.enabled = false;
            }

            var ao = SceneManager.LoadSceneAsync(loadingSceneName);

            ao.allowSceneActivation = false;

            yield return new WaitUntil(() => ao.progress >= 0.9f);

            if (currentSceneTransition != null)
            {
                yield return currentSceneTransition.CoTransitionOut(nextSceneName);
            }

            currentSceneHandler?.OnExit();

            ao.allowSceneActivation = true;

            yield return new WaitUntil(() => ao.isDone);

            var loadingScene = SceneManager.GetActiveScene();
            var loadingSceneHandler = FindComponent<SceneHandler>(loadingScene);
            var loadingSceneTransition = FindComponent<ISceneTransition>(loadingScene);
            var loadingProgress = FindComponent<ILoadingProgress>(loadingScene);

            loadingSceneHandler?.OnEnter();

            if (loadingSceneTransition != null)
            {
                yield return loadingSceneTransition.CoTransitionIn(currentSceneName);
            }

            ao = SceneManager.LoadSceneAsync(nextSceneName);

            ao.allowSceneActivation = false;

            var loadingStart = Time.time;
            var progress = 0f;

            while (progress < 0.9f)
            {
                var elapsed = Time.time - loadingStart;
                progress = MinLoadingTime > 0f ? Mathf.Min(elapsed / MinLoadingTime, ao.progress) : ao.progress;
                loadingProgress?.OnProgress(progress);
                yield return null;
            }

            loadingProgress?.OnProgress(1f);

            if (loadingSceneTransition != null)
            {
                yield return loadingSceneTransition.CoTransitionOut(nextSceneName);
            }

            loadingSceneHandler?.OnExit();

            ao.allowSceneActivation = true;

            yield return new WaitUntil(() => ao.isDone);

            var nextScene = SceneManager.GetSceneByName(nextSceneName);
            var nextEventSystem = GetEventSystemInScene(nextScene);
            var nextSceneHandler = FindComponent<T>(nextScene);
            var nextSceneTransition = FindComponent<ISceneTransition>(nextScene);

            if (nextEventSystem != null)
            {
                nextEventSystem.enabled = false;
            }

            onLoadScene?.Invoke(nextSceneHandler);
            nextSceneHandler?.OnEnter();

            if (nextSceneTransition != null)
            {
                yield return nextSceneTransition.CoTransitionIn(currentSceneName);
            }

            if (nextEventSystem != null)
            {
                nextEventSystem.enabled = true;
            }

            _isLoading = false;
        }
        
        private EventSystem GetEventSystemInScene(Scene scene)
        {
            EventSystem[] ess = FindObjectsOfType<EventSystem>();

            for (int i = 0; i < ess.Length; i++)
            {
                if (ess[i].gameObject.scene == scene) return ess[i];
            }

            return null;
        }

        private T FindComponent<T>(Scene scene) where T : class
        {
            GameObject[] goes = scene.GetRootGameObjects();

            for (int i = 0; i < goes.Length; i++)
            {
                var handler = goes[i].GetComponent<T>();
                
                if (handler != null) return handler;
            }

            return null;
        }
    }
}