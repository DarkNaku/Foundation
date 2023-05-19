using System.Collections;

namespace DarkNaku.Foundation
{
    public interface ITransition
    {
        IEnumerator CoTransitionIn(string prevSceneName);
        IEnumerator CoTransitionOut(string nextSceneName);
    }
}