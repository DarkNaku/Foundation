using System;
using UnityEngine;
using UnityEngine.Pool;

namespace DarkNaku.GOPool
{
    [Serializable]
    public class GOPoolData
    {
        [SerializeField] private string _key;
        [SerializeField] private GameObject _prefab;
        [SerializeField, Min(0)] private int _warmUpCount;

        public string Key => _key;
        public GameObject Prefab => _prefab;
        public int WarmUpCount => _warmUpCount;
        public IObjectPool<IGOPoolItem> Pool { get; set; }

        public GOPoolData(string key, GameObject prefab, int warmUpCount = 0)
        {
            _key = key;
            _prefab = prefab;
            _warmUpCount = warmUpCount;
        }
    }
}