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

        public async Task Initialize(Manager parent = null)
        {
            if (Initialized) return;

            _isInitializing = true;

            for (int i = 0; i < _managers.Count; i++)
            {
                if (_managers[i].Initialized == false)
                {
                    await _managers[i].Initialize(this);
                }
            }

            await OnInitialize();

            _isInitializing = false;

            Initialized = true;

            for (int i = 0; i < _managers.Count; i++)
            {
                await _managers[i].OnStart();
            }

            if (parent == null)
            {
                await OnStart();
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

        protected virtual async Task OnInitialize()
        {
        }

        protected virtual async Task OnStart()
        {
        }

        protected virtual void OnUninitialize()
        {
        }
    }
}