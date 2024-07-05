using System;
using UnityEngine;
using UnityEngine.Pool;

namespace DarkNaku.GOPool
{
    [Serializable]
    public struct GOPoolData
    {
        public string Key { get; private set; }
        public IObjectPool<IGOPoolItem> Pool { get; private set; }
        
        public GOPoolData(string key, IObjectPool<IGOPoolItem> pool)
        {
            Key = key;
            Pool = pool;
        }
    }
}