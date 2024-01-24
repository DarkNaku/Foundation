using UnityEngine;

namespace DarkNaku.Foundation
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private bool _isPersistent = true;

        private static readonly object _lock = new();
        private static T _instance;
        private static bool _isQuitting;

        public static T Instance
        {
            get
            {
                if (_isQuitting) return null;

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();

                        if (_instance == null)
                        {
                            _instance = new GameObject().AddComponent<T>();
                        }

                        _instance.name = string.Format("[Singleton {0}]", typeof(T).ToString());

                        (_instance as SingletonBehaviour<T>).OnInstantiate();
                    }

                    return _instance;
                }
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(this.gameObject);
            }

            if (_isPersistent)
            {
                DontDestroyOnLoad(gameObject);
            }

            OnAwake();
        }

        private void OnDestroy()
        {
            _isQuitting = true;
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        protected virtual void OnInstantiate()
        {
        }

        protected virtual void OnAwake()
        {
        }
    }
}