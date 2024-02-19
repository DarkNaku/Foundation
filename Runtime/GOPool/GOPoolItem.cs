using UnityEngine;
using UnityEngine.Pool;

namespace DarkNaku.GOPool
{
    public class GOPoolItem : MonoBehaviour, IGOPoolItem
    {
        public GameObject GO => gameObject;
        public IObjectPool<IGOPoolItem> Pool { get; set; }

        public void OnGetItem()
        {
            GO.SetActive(true);
        }

        public void OnReleaseItem()
        {
            GO.SetActive(false);
        }

        public void OnDestroyItem()
        {
        }
    }
}