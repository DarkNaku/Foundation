using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DarkNaku.Foundation
{
    public abstract class Manager : MonoBehaviour
    {
        public bool Initialized { get; private set; }

        private bool _isInitializing;
        private HashSet<Manager> _managers;

        public virtual void Initialize(Manager parent = null)
        {
            if (Initialized) return;

            _isInitializing = true;
            
            FindAllManagerInChildren();

            foreach (var manager in _managers)
            {
                if (manager.Initialized == false)
                {
                    manager.Initialize(this);
                }
            }

            OnInitialize();

            _isInitializing = false;

            Initialized = true;

            foreach (var manager in _managers)
            {
                manager.OnStart();
            }

            if (parent == null)
            {
                OnStart();
            }
        }
        
        public virtual async Task InitializeAsync(Manager parent = null)
        {
            if (Initialized) return;

            _isInitializing = true;
            
            FindAllManagerInChildren();

            foreach (var manager in _managers)
            {
                if (manager.Initialized == false)
                {
                    await manager.InitializeAsync(this);
                }
            }

            await OnInitializeAsync();

            _isInitializing = false;

            Initialized = true;

            foreach (var manager in _managers)
            {
                await manager.OnStartAsync();
            }

            if (parent == null)
            {
                await OnStartAsync();
            }
        }

        public virtual void Uninitialize()
        {
            if (Initialized == false) return;

            foreach (var manager in _managers)
            {
                manager.Uninitialize();
            }

            OnUninitialize();

            Initialized = false;
        }

        public T Get<T>() where T : class
        {
            if (_isInitializing)
            {
                Debug.LogError("[Mangaer] Get : Do not refer to other managers during initialization.");

                return null;
            }

            if (this is T)
            {
                return this as T;
            }
            else
            {
                foreach (var manager in _managers)
                {
                    var found = manager.Get<T>();

                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            return null;
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected virtual async Task OnInitializeAsync()
        {
            await Task.Delay(0);
        }

        protected virtual async Task OnStartAsync()
        {
            await Task.Delay(0);
        }

        protected virtual void OnUninitialize()
        {
        }
        
        private void FindAllManagerInChildren()
        {
            _managers = new();
            
            transform.ForEachChild((child) =>
            {
                var manager = child.GetComponent<Manager>();

                if (manager == null) return;
                if (_managers.Contains(manager)) return;
                
                manager.FindAllManagerInChildren();
                
                _managers.Add(manager);
            });
        }
    }
    
    public abstract class Manager<T> : Manager where T : class
    {
        public static T Instance => _instance;
        
        private static T _instance;

        public sealed override void Initialize(Manager parent = null)
        {
            if (Initialized) return;

            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance.Equals(this) == false)
            {
                Destroy(gameObject);
            }
            
            base.Initialize(parent);
        }

        public sealed override async Task InitializeAsync(Manager parent = null)
        {
            if (Initialized) return;

            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance.Equals(this) == false)
            {
                Destroy(gameObject);
            }
            
            await base.InitializeAsync(parent);
        }

        public sealed override void Uninitialize()
        {
            base.Uninitialize();

            _instance = null;
        }
    }
}