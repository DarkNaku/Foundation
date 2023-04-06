using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Foundation
{
    public interface IPopupTransition
    {
        IEnumerator CoTransitionIn(PopupHandler handler);
        IEnumerator CoTransitionOut(PopupHandler handler);
    }
}