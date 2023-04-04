using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Foundation
{
    public class GOPool : SingletonBehaviour<GOPool>
    {
        private class PoolData
        {
            public GameObject Mold { get; }
            public List<GOItem> Pool { get; }

            public PoolData(GameObject mold)
            {
                Mold = mold;
                Pool = new List<GOItem>();
            }
        }

        private readonly Dictionary<string, PoolData> _pools = new();
        private readonly Queue<GOItem> _trash = new();
        private bool _isRecycleWorking = false;

        public static bool Contains(string key) => Instance._pools.ContainsKey(key);
        public static void Register(string key, GameObject mold) => Instance._Register(key, mold);
        public static void Unregister(string key) => Instance._Unregister(key);
        public static T Get<T>(string key, Transform parent = null) where T : GOItem => Instance._Get<T>(key, parent);
        public static GameObject Get(string key, Transform parent = null) => Instance._Get(key, parent);
        public static void Release(GameObject go, bool immediately = false) => Instance._Release(go, immediately);
        public static void Release(GOItem item, bool immediately = false) => Instance._Release(item, immediately);
        public static void Clear() => Instance._Clear();

        private void _Register(string key, GameObject mold)
        {
            Debug.Assert(string.IsNullOrEmpty(key) == false, "[GOPool] Register : Key is null or empty.");
            Debug.Assert(mold != null, "[GOPool] Register : Mold is null.");

            _pools.Add(key, new PoolData(mold));
        }

        private void _Unregister(string key)
        {
            Debug.Assert(string.IsNullOrEmpty(key) == false, "[GOPool] Unregister : Key is null or empty.");

            _pools.Remove(key);
        }

        private T _Get<T>(string key, Transform parent) where T : GOItem
        {
            var item = _Get(key, parent);

            if (item == null)
            {
                return null;
            }
            else
            {
                return item.GetComponent<T>();
            }
        }

        private GameObject _Get(string key, Transform parent)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("[GOPool] Get : Key is null or empty.");
                return null;
            }

            if (_pools.ContainsKey(key) == false)
            {
                Debug.LogError("[GOPool] Get : Item are not registered.");
                return null;
            }

            GOItem item = null;
            var pool = _pools[key].Pool;

            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].IsInUse == false)
                {
                    item = pool[i];
                    break;
                }
            }

            if (item == null)
            {
                item = CreateItem(_pools[key]);
                if (item == null) return null;
            }

            if (parent == null)
            {
                item.transform.SetParent(transform);
            }
            else
            {
                item.transform.SetParent(parent);
            }

            item.OnGet();

            return item.gameObject;
        }

        private void _Release(GameObject go, bool immediately)
        {
            var item = go.GetComponent<GOItem>();

            if (item != null)
            {
                _Release(item, immediately);
            }
        }

        private void _Release(GOItem item, bool immediately)
        {
            if (item == null)
            {
                Debug.LogError("[GOPool] Release : Item is null.");
                return;
            }

            item.transform.SetParent(transform);
            item.OnRelease();

            if (immediately == false)
            {
                StartCoroutine(CoRecycle(item));
            }
        }

        private void _Clear()
        {
            foreach (var data in _pools.Values)
            {
                for (int i = 0; i < data.Pool.Count; i++)
                {
                    Destroy(data.Pool[i].gameObject);
                }

                data.Pool.Clear();
            }

            _pools.Clear();
        }

        private GOItem CreateItem(PoolData data)
        {
            var go = Instantiate(data.Mold);

            go.name = go.name.Replace("(Clone)", "");

            var item = go.GetComponent<GOItem>();

            if (item == null)
            {
                item = go.AddComponent<GOItem>();
            }

            item.OnCreate();

            data.Pool.Add(item);

            return item;
        }

        private IEnumerator CoRecycle(GOItem item)
        {
            if (_trash.Contains(item) == false)
            {
                _trash.Enqueue(item);
            }

            if (_isRecycleWorking) yield break;

            _isRecycleWorking = true;

            yield return new WaitForEndOfFrame();

            while (_trash.Count > 0)
            {
                _trash.Dequeue();
            }

            _isRecycleWorking = false;
        }
    }
}