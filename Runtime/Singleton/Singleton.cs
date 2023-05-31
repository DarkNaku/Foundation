using UnityEngine;

namespace DarkNaku.Foundation
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static object _lock = new();
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                            (_instance as Singleton<T>).OnInstantiate();
                            Application.quitting += () => _instance = null;
                        }
                    }
                }

                return _instance;
            }
        }

        protected virtual void OnInstantiate()
        {
        }
    }
}