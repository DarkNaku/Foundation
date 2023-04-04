using UnityEngine;

namespace DarkNaku.Foundation
{
    public class GOItem : MonoBehaviour
    {
        public bool IsInUse { get; private set; }

        public virtual void OnCreate()
        {
        }

        public virtual void OnGet()
        {
            gameObject.SetActive(true);
            IsInUse = true;
        }

        public virtual void OnRelease()
        {
            gameObject.SetActive(false);
            IsInUse = false;
        }
    }
}