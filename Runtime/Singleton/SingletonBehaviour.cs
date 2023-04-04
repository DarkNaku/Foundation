using UnityEngine;

namespace DarkNaku.Foundation
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static object _lock = new object();
        private static bool _isDestroyed = false;

        private static T _instance = null;
        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_isDestroyed) return null;

                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();

                        if (_instance == null)
                        {
                            _instance = (new GameObject()).AddComponent<T>();
                        }

                        _instance.name = string.Format("[{0}]", typeof(T).ToString().ToUpper());

                        (_instance as SingletonBehaviour<T>).OnInstantiate();
                    }

                    return _instance;
                }
            }
        }

        protected void OnDestroy()
        {
            _isDestroyed = true;
        }

        protected virtual void OnInstantiate()
        {
        }
    }
}