using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DarkNaku.Foundation
{
    public abstract class Manager : MonoBehaviour
    {
        [SerializeField] private List<Manager> _managers;

        public bool Initialized { get; private set; }

        private bool _isInitializing;
        private Manager _parent;

        public void Initialize(Manager parent = null)
        {
            if (Initialized) return;

            _isInitializing = true;

            _parent = parent;

            for (int i = 0; i < _managers.Count; i++)
            {
                if (_managers[i].Initialized == false)
                {
                    _managers[i].Initialize(this);
                }
            }

            OnInitialize();

            _isInitializing = false;

            Initialized = true;

            for (int i = 0; i < _managers.Count; i++)
            {
                _managers[i].OnStart();
            }

            if (_parent == null)
            {
                OnStart();
            }
        }

        public async Task InitializeAsync(Manager parent = null)
        {
            if (Initialized) return;

            _isInitializing = true;

            _parent = parent;

            for (int i = 0; i < _managers.Count; i++)
            {
                if (_managers[i].Initialized == false)
                {
                    await _managers[i].InitializeAsync(this);
                }
            }

            await OnInitializeAsync();

            _isInitializing = false;

            Initialized = true;

            for (int i = 0; i < _managers.Count; i++)
            {
                await _managers[i].OnStartAsync();
            }

            if (_parent == null)
            {
                await OnStartAsync();
            }
        }

        public void Uninitialize()
        {
            if (Initialized == false) return;

            for (int i = 0; i < _managers.Count; i++)
            {
                _managers[i].Uninitialize();
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
                for (int i = 0; i < _managers.Count; i++)
                {
                    var manager = _managers[i].Get<T>();

                    if (manager != null)
                    {
                        return manager;
                    }
                }
            }

            return null;
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual async Task OnInitializeAsync()
        {
            await Task.Yield();
        }

        protected virtual void OnStart()
        {
        }

        protected virtual async Task OnStartAsync()
        {
            await Task.Yield();
        }

        protected virtual void OnUninitialize()
        {
        }
    }
}