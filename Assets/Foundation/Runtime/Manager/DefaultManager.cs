using UnityEngine;

namespace DarkNaku.Foundation {
    public sealed class DefaultManager : Manager<DefaultManager> {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration() {
            _instance = null;
        }
    }
}