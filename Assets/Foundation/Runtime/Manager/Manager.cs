using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DarkNaku.Foundation {
    public abstract class Manager : MonoBehaviour, IDisposable {
        public bool Initialized { get; private set; }

        private bool _isInitializing;
        private Manager _parent;
        private HashSet<Manager> _managers = new();

        public async Task Initialize(Manager parent = null) {
            if (Initialized) return;
            
            _isInitializing = true;
            
            _parent = parent;

            if (_parent == null) FindManagers();

            foreach (var manager in _managers) {
                if (manager.Initialized == false) {
                    await manager.Initialize(this);
                }
            }

            await OnInitialize();

            _isInitializing = false;

            Initialized = true;

            foreach (var manager in _managers) {
                await manager.OnStart();
            }

            if (parent == null) {
                await OnStart();
            }
        }

        public void Dispose() {
            if (Initialized == false) return;

            foreach (var manager in _managers) {
                manager.Dispose();
            }

            OnDispose();

            Initialized = false;
        }

        public T Get<T>() where T : Manager {
            if (_isInitializing) {
                Debug.LogError("[Mangaer] Get : Do not refer to other managers during initialization.");

                return null;
            }

            if (this is T) {
                return this as T;
            } else {
                if (_parent == null) {
                    foreach (var manager in _managers) {
                        var found = manager.Get<T>();

                        if (found != null) {
                            return found;
                        }
                    }
                } else {
                    return _parent.Get<T>();
                }
            }

            return null;
        }
        
        protected virtual Task OnInitialize() {
            return Task.CompletedTask;
        }

        protected virtual Task OnStart() {
            return Task.CompletedTask;
        }

        protected virtual void OnDispose() {
        }

        protected virtual void Awake() {
        }
        
        protected virtual void OnDestroy() {
        }

        private void FindManagers() {
            transform.ForEachChildIncludeSelf((tf) => {
                var managers = tf.GetComponents<Manager>();

                foreach (var manager in managers) {
                    if (manager == null) continue;
                    if (manager == this) continue;
                    if (_managers.Contains(manager)) continue;

                    _managers.Add(manager);
                }
            }, true);
        }
    }
    
    // 주의 : 도메인 리로드 꺼놓은 경우 _instance 변수 초기화가 되지 않기 때문에 상속 받은 클래스에서 아래 이벤트로 초기화를 해주어야 합니다.
    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public abstract class Manager<T> : Manager where T : Manager<T> {
        public static T Instance => _instance;
        
        protected static T _instance;

        protected sealed override void Awake() {
            if (_instance == null) {
                _instance = this as T;
            } else if (_instance.Equals(this) == false) {
                Destroy(this);
                Debug.LogErrorFormat("[Mangaer] Initialize : {0} has been registered multiple times.", GetType());
            }
            
            OnAwake();
        }

        protected sealed override void OnDestroy() {
            _instance = null;
            
            OnFireDestroy();
        }

        protected virtual void OnAwake() {
        }
        
        protected virtual void OnFireDestroy() {
        }
    }
}