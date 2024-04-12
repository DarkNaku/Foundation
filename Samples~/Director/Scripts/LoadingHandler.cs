using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkNaku.Director;
using UnityEngine.UI;
using DarkNaku.Attribute;

public class LoadingHandler : SceneHandler, ILoadingProgress, ISceneTransition
{
    [SerializeField, AutomaticReference(REFERENCE_TYPE.Find)] private Slider _slider;
    [SerializeField, AutomaticReference(REFERENCE_TYPE.Find)] private Image _curtain;

    public override void OnEnter()
    {
        Debug.Log($"OnEnter - Scene Loading");
    }

    public override void OnExit()
    {
        Debug.Log($"OnExit - Scene Loading");
    }

    public void OnProgress(float progress)
    {
        _slider.value = progress;
    }

    public IEnumerator CoTransitionIn(string prevSceneName)
    {
        _curtain.gameObject.SetActive(true);
        _curtain.color = Color.black;

        var remainTime = 0.5f;

        while (remainTime > 0)
        {
            yield return null;

            remainTime = Mathf.Max(0f, remainTime - Time.deltaTime);

            var color = _curtain.color;
            color.a = remainTime / 0.5f;
            _curtain.color = color;
        }

        _curtain.gameObject.SetActive(false);
    }

    public IEnumerator CoTransitionOut(string nextSceneName)
    {
        _curtain.gameObject.SetActive(true);
        _curtain.color = new Color(0f, 0f, 0f, 0f);

        var remainTime = 0.5f;

        while (remainTime > 0)
        {
            yield return null;

            remainTime = Mathf.Max(0f, remainTime - Time.deltaTime);

            var color = _curtain.color;
            color.a = 1f - (remainTime / 0.5f);
            _curtain.color = color;
        }

        //_curtain.gameObject.SetActive(false);
    }
}