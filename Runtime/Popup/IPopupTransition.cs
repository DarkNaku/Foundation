using System.Collections;

namespace DarkNaku.Popup
{
    public interface IPopupTransition
    {
        IEnumerator CoTransitionIn();
        IEnumerator CoTransitionIn(PopupHandler handler);
        IEnumerator CoTransitionOut();
        IEnumerator CoTransitionOut(PopupHandler handler);
    }
}