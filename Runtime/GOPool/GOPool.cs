using System.Collections;
using System.Collections.Generic;
using DarkNaku.Foundation;
using UnityEngine;
using UnityEngine.Pool;

namespace DarkNaku.GOPool
{
    public class GOPool : SingletonBehaviour<GOPool>
    {
        private Dictionary<string, GOPoolData> _moldTable = new Dictionary<string, GOPoolData>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            _instance = null;
        }

        public static void Register(string key, GameObject prefab)
        {
            Instance._Register(key, prefab);
        }

        public static void Unregister(string key)
        {
            Instance._Unregister(key);
        }
        
        public static GameObject Get(string key, Transform parent = null)
        {
            return Instance._Get(key, parent).GO;
        }
        
        public static U Get<U>(string key, Transform parent = null) where U : IGOPoolItem
        {
            return Instance._Get<U>(key, parent);
        }
        
        public static void Release(GameObject item)
        {
            Instance._Release(item);
        }
        
        public static void Release(IGOPoolItem item)
        {
            Instance._Release(item);
        }

        private void _RegisterBuiltIn(params string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                var prefab = Resources.Load<GameObject>(paths[i]);

                if (prefab == null)
                {
                    Debug.LogError($"[GOPool] RegisterBuiltIn : Prefab is null. Path = {paths[i]}");
                    continue;
                }

                //var key = (T) (object) paths[i];
                
                _Register(paths[i], prefab);
            }
        }

        private void _Register(string key, GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError($"[GOPool] Register : Prefab is null.");
                return;
            }

            var pool = new ObjectPool<IGOPoolItem>(
                () =>
                {
                    var go = Instantiate(prefab);
                    
                    var item = go.GetComponent<IGOPoolItem>();

                    if (item == null)
                    {
                        item = go.AddComponent<GOPoolItem>();
                    }

                    return item;
                },
                OnGetItem,
                OnReleaseItem,
                OnDestroyItem);

            var data = new GOPoolData(key, pool);

            _moldTable.TryAdd(key, data);
        }

        private void _Unregister(string key)
        {
            if (_moldTable.ContainsKey(key))
            {
                _moldTable.Remove(key);
                // 사용중인건? 모두 제거 해야함
            }
        }

        private T _Get<T>(string key, Transform parent) where T : IGOPoolItem
        {
            var item = _Get(key, parent);

            return (item != null) ? item.GO.GetComponent<T>() : default;
        }
        
        private IGOPoolItem _Get(string key, Transform parent)
        {
            if (_moldTable.TryGetValue(key, out var data))
            {
                var item = data.Pool.Get();
                item.Pool = data.Pool;
                item.GO.transform.SetParent(parent);
                return item;
            }

            return default;
        }
        
        private void _Release(GameObject item)
        {
            _Release(item.GetComponent<IGOPoolItem>());
        }

        private void _Release(IGOPoolItem item)
        {
            if (item != null)
            {
                item.Pool.Release(item);
            }
        }

        private void OnGetItem(IGOPoolItem item)
        {
            item.OnGetItem();
        }
        
        private void OnReleaseItem(IGOPoolItem item)
        {
            item.OnReleaseItem();
        }

        private void OnDestroyItem(IGOPoolItem item)
        {
            item.OnDestroyItem();
        }
    }
}