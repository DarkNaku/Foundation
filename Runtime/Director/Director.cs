using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace DarkNaku.Foundation
{
    public class Director : SingletonBehaviour<Director>
    {
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
        
        public static void Change<T>(string nextSceneName, Action<T> onLoadScene = null) where T : SceneHandler
        {
            Instance.StartCoroutine(Instance.CoChange(nextSceneName, onLoadScene));
        }
        
        public IEnumerator CoChange<T>(string nextSceneName, Action<T> onLoadScene) where T : SceneHandler
        {
            if (_isLoading) yield break;

            _isLoading = true;

            var currentScene = SceneManager.GetActiveScene();
            var currentEventSystem = GetEventSystemInScene(currentScene);
            var currentSceneHandler = FindComponent<SceneHandler>(currentScene);
            var currentSceneTransition = FindComponent<ISceneTransition>(currentScene);
            var currentSceneName = currentScene.name;

            var ao = SceneManager.LoadSceneAsync(nextSceneName);

            ao.allowSceneActivation = false;

            if (currentEventSystem != null)
            {
                currentEventSystem.enabled = false;
            }

            yield return new WaitUntil(() => ao.progress >= 0.9f);

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