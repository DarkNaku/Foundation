using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace DarkNaku.Foundation
{
    public class Director : SingletonBehaviour<Director>
    {
        private bool _isChanging = false;

        public static void Change(string nextSceneName)
        {
            Instance.StartCoroutine(Instance._Change(nextSceneName, null, null));
        }

        public static void Change(string nextSceneName, string loadingSceneName)
        {
            Instance.StartCoroutine(Instance._Change(nextSceneName, loadingSceneName, null));
        }

        public static void Change(string nextSceneName, string loadingSceneName, object param)
        {
            Instance.StartCoroutine(Instance._Change(nextSceneName, loadingSceneName, param));
        }

        protected override void OnInstantiate()
        {
            DontDestroyOnLoad(gameObject);
        }

        private IEnumerator _Change(string nextSceneName, string loadingSceneName, object param)
        {
            if (_isChanging) yield break;

            _isChanging = true;

            if (loadingSceneName == null)
            {
                yield return ChangeWithOutLoading(nextSceneName, param);
            }
            else
            {
                yield return ChangeWithLoading(nextSceneName, loadingSceneName, param);
            }

            _isChanging = false;
        }

        private IEnumerator ChangeWithLoading(string nextSceneName, string loadingSceneName, object param)
        {
            var currentScene = SceneManager.GetActiveScene();
            var currentEventSystem = GetEventSystemInScene(currentScene);
            var currentSceneHandler = FindComponent<SceneHandler>(currentScene);
            var currentSceneTransition = FindComponent<ITransition>(currentScene);
            var currentSceneName = currentScene.name;

            if (currentEventSystem != null) currentEventSystem.enabled = false;

            var ao = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Single);
            ao.allowSceneActivation = false;

            yield return new WaitUntil(() => ao.progress >= 0.9f);

            currentSceneHandler.OnWillLeave();

            yield return currentSceneTransition?.CoTransitionOut(nextSceneName);

            currentSceneHandler.OnDidLeave();

            ao.allowSceneActivation = true;

            yield return new WaitUntil(() => ao.isDone);

            var loadingScene = SceneManager.GetSceneByName(loadingSceneName);
            var loadingEventSystem = GetEventSystemInScene(loadingScene);
            var loadingSceneHandler = FindComponent<SceneHandler>(loadingScene);
            var loadingSceneTransition = FindComponent<ITransition>(loadingScene);

            if (loadingEventSystem != null) loadingEventSystem.enabled = false;

            yield return loadingSceneHandler.CoInitialize(param);

            loadingSceneHandler.OnWillEnter(param);

            yield return loadingSceneTransition?.CoTransitionIn(currentSceneName);

            loadingSceneHandler.OnDidEnter(param);

            if (loadingEventSystem != null) loadingEventSystem.enabled = true;

            ao = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
            ao.allowSceneActivation = false;

            while (ao.progress >= 0.9f)
            {
                loadingSceneHandler.OnProgress(ao.progress);
                yield return null;
            }

            loadingSceneHandler.OnProgress(1f);

            loadingSceneHandler.OnWillLeave();

            yield return loadingSceneTransition?.CoTransitionOut(nextSceneName);

            loadingSceneHandler.OnDidLeave();

            ao.allowSceneActivation = true;

            yield return new WaitUntil(() => ao.isDone);

            var nextScene = SceneManager.GetSceneByName(nextSceneName);
            var nextEventSystem = GetEventSystemInScene(nextScene);
            var nextSceneHandler = FindComponent<SceneHandler>(nextScene);
            var nextSceneTransition = FindComponent<ITransition>(nextScene);

            if (nextEventSystem != null) nextEventSystem.enabled = false;

            yield return nextSceneHandler.CoInitialize(param);

            nextSceneHandler.OnWillEnter(param);

            yield return nextSceneTransition?.CoTransitionIn(currentSceneName);

            nextSceneHandler.OnDidEnter(param);

            if (nextEventSystem != null) nextEventSystem.enabled = true;
        }

        private IEnumerator ChangeWithOutLoading(string nextSceneName, object param)
        {
            var currentScene = SceneManager.GetActiveScene();
            var currentEventSystem = GetEventSystemInScene(currentScene);
            var currentSceneHandler = FindComponent<SceneHandler>(currentScene);
            var currentSceneTransition = FindComponent<ITransition>(currentScene);
            var currentSceneName = currentScene.name;

            var ao = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
            ao.allowSceneActivation = false;

            if (currentEventSystem != null) currentEventSystem.enabled = false;

            while (ao.progress < 0.9f)
            {
                currentSceneHandler.OnProgress(ao.progress);
                yield return null;
            }

            currentSceneHandler.OnProgress(1f);
            currentSceneHandler.OnWillLeave();

            yield return currentSceneTransition?.CoTransitionOut(nextSceneName);

            currentSceneHandler.OnDidLeave();

            ao.allowSceneActivation = true;

            yield return new WaitUntil(() => ao.isDone);

            var nextScene = SceneManager.GetSceneByName(nextSceneName);
            var nextEventSystem = GetEventSystemInScene(nextScene);
            var nextSceneHandler = FindComponent<SceneHandler>(nextScene);
            var nextSceneTransition = FindComponent<ITransition>(nextScene);

            if (nextEventSystem != null) nextEventSystem.enabled = false;

            yield return nextSceneHandler.CoInitialize(param);

            nextSceneHandler.OnWillEnter(param);

            yield return nextSceneTransition?.CoTransitionIn(currentSceneName);

            nextSceneHandler.OnDidEnter(param);

            if (nextEventSystem != null) nextEventSystem.enabled = true;
        }

        private EventSystem GetEventSystemInScene(Scene scene)
        {
            EventSystem[] ess = EventSystem.FindObjectsOfType<EventSystem>();

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
                T handler = goes[i].GetComponent(typeof(T)) as T;
                if (handler != null) return handler;
            }

            return null;
        }
    }
}