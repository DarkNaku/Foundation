using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Director
{
    public interface ISceneTransition
    {
        IEnumerator CoTransitionIn(string prevSceneName);
        IEnumerator CoTransitionOut(string nextSceneName);
    }
}