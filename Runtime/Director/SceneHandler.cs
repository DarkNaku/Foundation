using System.Collections;
using UnityEngine;


namespace DarkNaku.Foundation
{
    public abstract class SceneHandler : MonoBehaviour
    {
        public virtual IEnumerator CoInitialize(object param)
        {
            yield break;
        }

        public virtual void OnWillEnter(object param)
        {
        }

        public virtual void OnDidEnter(object param)
        {
        }

        public virtual void OnWillLeave()
        {
        }

        public virtual void OnDidLeave()
        {
        }

        public virtual void OnProgress(float progress)
        {
        }
    }
}