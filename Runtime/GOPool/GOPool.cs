using System.Collections.Generic;
using DarkNaku.Foundation;
using UnityEngine;
using UnityEngine.Pool;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DarkNaku.GOPool
{
    public class GOPool : SingletonScriptable<GOPool>
    {
        [SerializeField] private List<GOPoolData> _items = new();

        private Dictionary<string, GOPoolData> _moldTable;

        public static void Register(GOPoolData data)
        {
            Instance._Register(data);
        }

        public static void Unregister(string key)
        {
            Instance._Unregister(key);
        }
        
        public static GameObject Get(string key, Transform parent = null)
        {
            return Instance._Get(key, parent).GO;
        }
        
        public static T Get<T>(string key, Transform parent = null) where T : IGOPoolItem
        {
            return Instance._Get<T>(key, parent);
        }
        
        public static void Release(GameObject item)
        {
            Instance._Release(item);
        }
        
        public static void Release(IGOPoolItem item)
        {
            Instance._Release(item);
        }
        
        protected override void OnInitialize()
        {
            _moldTable = new();

            for (int i = 0; i < _items.Count; i++)
            {
                _Register(_items[i]);
            }
        }

        private void _Register(GOPoolData data)
        {
            if (data.Prefab == null) return;

            var prefab = data.Prefab;

            data.Pool = new ObjectPool<IGOPoolItem>(
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

            if (string.IsNullOrEmpty(data.Key))
            {
                _moldTable.TryAdd(data.Prefab.name, data);
            }
            else
            {
                _moldTable.TryAdd(data.Key, data);
            }
        }

        private void _Unregister(string key)
        {
            if (_moldTable.ContainsKey(key))
            {
                Destroy(_moldTable[key].Prefab);

                _moldTable.Remove(key);
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
        
#if UNITY_EDITOR
        [MenuItem("Tools/GOPool")]
        private static void SelectGOPool()
        {
            Selection.activeObject = Instance;
        }
#endif
    }
}