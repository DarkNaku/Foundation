using UnityEngine;
using UnityEngine.Pool;

namespace DarkNaku.GOPool
{
    public interface IGOPoolItem
    {
        GameObject GO { get; }
        IObjectPool<IGOPoolItem> Pool { get; set; }

        void OnGetItem();
        void OnReleaseItem();
        void OnDestroyItem();
    }
}