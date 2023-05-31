using System;
using UnityEngine;

namespace DarkNaku.Foundation
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static object _lock = new object();

        private static T _instance = null;
        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();

                        if (_instance == null)
                        {
                            _instance = (new GameObject()).AddComponent<T>();
                        }

                        _instance.name = string.Format("[{0}]", typeof(T).ToString().ToUpper());

                        (_instance as SingletonBehaviour<T>).OnInstantiate();
                        Application.quitting += () => _instance = null;
                    }

                    return _instance;
                }
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        protected virtual void OnInstantiate()
        {
        }
    }
}