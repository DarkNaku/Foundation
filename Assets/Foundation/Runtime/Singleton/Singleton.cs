using System;

namespace DarkNaku.Foundation {
    public abstract class Singleton<T> : IDisposable where T : class, new() {
        private static object _lock = new();
        protected static T _instance;

        public static T Instance {
            get {
                if (_instance == null) {
                    lock (_lock) {
                        if (_instance == null) {
                            _instance = new();

                            (_instance as Singleton<T>).OnInstantiate();
                        }
                    }
                }

                return _instance;
            }
        }

        protected virtual void OnInstantiate() {
        }

        public void Dispose() {
            _instance = null;
        }
    }
}