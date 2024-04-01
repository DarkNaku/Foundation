using System.Collections;

namespace DarkNaku.Popup
{
    public interface IPopupTransition
    {
        IEnumerator CoTransitionIn();
        IEnumerator CoTransitionOut();
    }
}