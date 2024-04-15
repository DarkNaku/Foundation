using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DarkNaku.Foundation;

namespace DarkNaku.Director
{
    public class Director : SingletonBehaviour<Director>
    {
        [SerializeField] private List<GameObject> _loadings;
        
        public static float MinLoadingTime
        {
            get => Instance._minLoadingTime;
            set => Instance._minLoadingTime = value;
        }

        private bool _isLoading;
        private float _minLoadingTime;
        private Dictionary<string, ISceneLoading> _loadingTable;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            var currentScene = SceneManager.GetActiveScene();
            var currentSceneHandler = Instance.FindComponent<SceneHandler>(currentScene);
            var currentSceneTransition = Instance.FindComponent<ISceneTransition>(currentScene);
            currentSceneHandler?.OnEnter();
            Instance.StartCoroutine(currentSceneTransition.CoTransitionIn(null));
        }
        
        public static void RegisterLoadingFromResource(string name, string path)
        {
            Instance._RegisterLoadingFromResource(name, path);
        }
        
        public static void RegisterLoading(string name, ISceneLoading loading)
        {
            Instance._RegisterLoading(name, loading);
        }
        
        public static void Change(string nextSceneName)
        {
            Instance.StartCoroutine(Instance.CoChange<SceneHandler>(nextSceneName, null));
        }
        
        public static void Change(string nextSceneName, string loadingName)
        {
            if (Instance._loadingTable.ContainsKey(loadingName))
            {
                Instance.StartCoroutine(Instance.CoChange<SceneHandler>(nextSceneName, loadingName, null));
            }
            else
            {
                Instance.StartCoroutine(Instance.CoChange<SceneHandler>(nextSceneName, null));
            }
        }
        
        public static void Change<T>(string nextSceneName, Action<T> onLoadScene = null) where T : SceneHandler
        {
            Instance.StartCoroutine(Instance.CoChange(nextSceneName, onLoadScene));
        }

        public static void Change<T>(string nextSceneName, string loadingName, Action<T> onLoadScene = null) where T : SceneHandler
        {
            if (Instance._loadingTable.ContainsKey(loadingName))
            {
                Instance.StartCoroutine(Instance.CoChange(nextSceneName, loadingName, onLoadScene));
            }
            else
            {
                Instance.StartCoroutine(Instance.CoChange(nextSceneName, onLoadScene));
            }
        }
        
        protected override void OnAwake()
        {
            _loadingTable = new Dictionary<string, ISceneLoading>();

            if (_loadings == null || _loadings.Count == 0) return;
            
            for (int i = 0; i < _loadings.Count; i++)
            {
                if (_loadings[i] == null) continue;
                
                var loading = _loadings[i].GetComponent<ISceneLoading>();
                
                if (loading != null)
                {
                    loading.Initialize();

                    _loadingTable.Add(_loadings[i].name, loading);
                }
            }
        }

        private void _RegisterLoadingFromResource(string name, string path)
        {
            if (_loadingTable.ContainsKey(name)) return;

            var resource = Resources.Load<GameObject>(path);
            var loadingInterface = resource?.GetComponent<ISceneLoading>();
                
            if (resource == null || loadingInterface == null)
            {
                Debug.LogWarningFormat("[Director] RegisterLoadingFromResource : Not Found Resource - {0}", name);
                return;
            }
            
            var go = Instantiate(resource);
                
            go.transform.SetParent(transform);
            
            var loading = go.GetComponent<ISceneLoading>();
            
            _RegisterLoading(name, loading);
        }
        
        private void _RegisterLoading(string name, ISceneLoading loading)
        {
            if (loading == null) return;

            if (_loadingTable.ContainsKey(name) || _loadingTable.Values.Contains(loading))
            {
                Debug.LogWarningFormat("[Director] RegisterLoading : Duplicated Loading - {0}", name);
                return;
            }
            
            loading.Initialize();

            _loadingTable.Add(name, loading);
        }

        private IEnumerator CoChange<T>(string nextSceneName, Action<T> onLoadScene) where T : SceneHandler
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

            while (progress < 1f)
            {
                progress = ao.progress / 0.9f;
                
                if (_minLoadingTime > 0f)
                {
                    progress = Mathf.Min((Time.time - loadingStart) / _minLoadingTime, progress);
                }
                
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

        private IEnumerator CoChange<T>(string nextSceneName, string loadingName, Action<T> onLoadScene) where T : SceneHandler
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
            
            if (currentSceneTransition != null)
            {
                yield return currentSceneTransition.CoTransitionOut(nextSceneName);
            }

            var loading = _loadingTable[loadingName];
            var loadingTransition = loading as ISceneTransition;
            var loadingProgress = loading as ILoadingProgress;
            
            if (loadingTransition != null)
            {
                yield return loadingTransition.CoTransitionIn(nextSceneName);
            }
            
            var ao = SceneManager.LoadSceneAsync(nextSceneName);

            ao.allowSceneActivation = false;

            var loadingStart = Time.time;
            var progress = 0f;

            while (progress < 1f)
            {
                progress = ao.progress / 0.9f;
                
                if (_minLoadingTime > 0f)
                {
                    progress = Mathf.Min((Time.time - loadingStart) / (_minLoadingTime * 0.5f), progress);
                }
                
                loadingProgress?.OnProgress(progress * 0.5f);
                
                yield return null;
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
            
            loadingStart = Time.time;
            progress = 0f;

            while (progress < 1f)
            {
                progress = Mathf.Min(1f, nextSceneHandler?.Progress ?? 1f);
                
                if (_minLoadingTime > 0f)
                {
                    progress = Mathf.Min((Time.time - loadingStart) / (_minLoadingTime * 0.5f), progress);
                }
                
                loadingProgress?.OnProgress((progress * 0.5f) + 0.5f);
                
                yield return null;
            }
            
            if (loadingTransition != null)
            {
                yield return loadingTransition.CoTransitionOut(currentSceneName);
            }

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