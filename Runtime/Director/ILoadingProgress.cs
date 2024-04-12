using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Director
{
    public interface ILoadingProgress
    {
        void OnProgress(float progress);
    }
}