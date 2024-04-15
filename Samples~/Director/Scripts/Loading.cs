using System.Collections;
using System.Collections.Generic;
using DarkNaku.Attribute;
using DarkNaku.Director;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour, ISceneLoading, ISceneTransition, ILoadingProgress
{
    [SerializeField, AutomaticReference] private CanvasGroup _canvasGroup;
    [SerializeField, AutomaticReference(REFERENCE_TYPE.InChildren)] private Slider _slider;
    
    public void Initialize()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator CoTransitionIn(string prevSceneName)
    {
        gameObject.SetActive(true);
        
        _canvasGroup.alpha = 0f;

        var remainTime = 0.5f;

        while (remainTime > 0)
        {
            yield return null;

            remainTime = Mathf.Max(0f, remainTime - Time.deltaTime);
            
            _canvasGroup.alpha = 1f - (remainTime / 0.5f);
        }
    }

    public IEnumerator CoTransitionOut(string nextSceneName)
    {
        var remainTime = 0.5f;

        while (remainTime > 0)
        {
            yield return null;

            remainTime = Mathf.Max(0f, remainTime - Time.deltaTime);

            _canvasGroup.alpha = remainTime / 0.5f;
        }

        gameObject.SetActive(false);
    }

    public void OnProgress(float progress)
    {
        _slider.value = progress;
    }
}