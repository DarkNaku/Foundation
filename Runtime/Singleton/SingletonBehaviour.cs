using UnityEngine;

namespace DarkNaku.Foundation
{
    public abstract class SingletonBehaviour : MonoBehaviour
    {
        protected virtual bool IsDontDestroyOnLoad { get; set; } = true;

        protected static bool IsQuitting { get; private set; }

        protected virtual void Awake()
        {
            if (IsDontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            OnAwake();
        }

        protected virtual void OnDestroy()
        {
            IsQuitting = true;
            
            OnFireDestroy();
        }

        protected virtual void OnApplicationQuit()
        {
            IsQuitting = true;
            
            OnFireApplicationQuit();
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnFireDestroy()
        {
        }

        protected virtual void OnFireApplicationQuit()
        {
        }
    }
    
    public abstract class SingletonBehaviour<T> : SingletonBehaviour where T : MonoBehaviour
    {
        private static readonly object _lock = new();
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (IsQuitting) return null;

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        var instances = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);

                        if (instances.Length > 0)
                        {
                            _instance = instances[0];

                            for (int i = 1; i < instances.Length; i++)
                            {
                                Debug.LogWarningFormat("[SingletonBehaviour] Instance Duplicated - {0}", instances[i].name);
                                Destroy(instances[i]);
                            }
                        }
                        else
                        {
                            _instance = new GameObject($"[Singleton - {typeof(T)}]").AddComponent<T>();
                        }
                    }

                    return _instance;
                }
            }
        }

        protected sealed override void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Debug.LogWarningFormat("[SingletonBehaviour] Duplicated - {0}", typeof(T).Name);
                Destroy(gameObject);
                return;
            }

            base.Awake();
        }
        
        protected sealed override void OnDestroy()
        {
            if (_instance == this)
            {
                base.OnDestroy();
            }
        }

        protected sealed override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
        }
    }
}