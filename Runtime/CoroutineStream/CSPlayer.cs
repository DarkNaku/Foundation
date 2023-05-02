using UnityEngine;

namespace DarkNaku.Foundation {
    public class CSPlayer : SingletonBehaviour<CSPlayer> {
        protected override void OnInstantiate() {
            DontDestroyOnLoad(gameObject);
        }

        public static CoroutineStream CoroutineStream() {
            return new CoroutineStream(Instance);
        }
    }
}