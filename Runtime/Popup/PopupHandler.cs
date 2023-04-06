using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Foundation
{
    public abstract class PopupHandler : MonoBehaviour
    {
        public Canvas PopupCanvas
        {
            get
            {
                if (_popupCanvas == null)
                {
                    _popupCanvas = GetComponent<Canvas>();
                }

                return _popupCanvas;
            }
        }

        public CanvasGroup PopupCanvasGroup
        {
            get
            {
                if (_popupCanvasGroup == null)
                {
                    _popupCanvasGroup = GetComponent<CanvasGroup>();
                }

                return _popupCanvasGroup;
            }
        }

        public bool IsInTransition { get; protected set; }

        protected IPopupTransition PopupTransition { get; private set; }

        private Canvas _popupCanvas = null;
        private CanvasGroup _popupCanvasGroup = null;
        private System.Action<object> _onWillHide = null;
        private System.Action<object> _onDidHide = null;

        public Coroutine Show(object param, System.Action<object> onWillHide, System.Action<object> onDidHide) {
            return StartCoroutine(CoShow(param, onWillHide, onDidHide));
        }

        public Coroutine Hide() {
            return StartCoroutine(CoHide(null));
        }

        public Coroutine Hide(object result) {
            return StartCoroutine(CoHide(result));
        }

        public void Initialize()
        {
            PopupTransition = GetComponent(typeof(IPopupTransition)) as IPopupTransition;
            OnInitialize();
        }

        public virtual void OnEscape()
        {
        }


        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnWillEnter(object param)
        {
        }

        protected virtual void OnDidEnter(object param)
        {
        }

        protected virtual void OnWillLeave()
        {
        }

        protected virtual void OnDidLeave()
        {
        }

        private IEnumerator CoShow(object param, System.Action<object> onWillHide, System.Action<object> onDidHide) {
            if (IsInTransition) {
                Debug.LogErrorFormat("[PopupHandler] CoShow : Already in transition.");
                yield break;
            }

            IsInTransition = true;

            _onWillHide = onWillHide;
            _onDidHide = onDidHide;

            OnWillEnter(param);

            if (PopupTransition != null) {
                yield return StartCoroutine(PopupTransition.CoTransitionIn(this));
            }

            OnDidEnter(param);

            if (PopupCanvasGroup != null) {
                PopupCanvasGroup.interactable = true;
            }

            IsInTransition = false;
        }

        private IEnumerator CoHide(object result) {
            if (IsInTransition) {
                Debug.LogErrorFormat("[PopupHandler] CoShow : Already in transition.");
                yield break;
            }

            IsInTransition = true;

            if (PopupCanvasGroup != null) {
                PopupCanvasGroup.interactable = false;
            }

            OnWillLeave();

            _onWillHide?.Invoke(result);
            _onWillHide = null;

            if (PopupTransition != null) {
                yield return StartCoroutine(PopupTransition.CoTransitionOut(this));
            }

            OnDidLeave();

            _onDidHide?.Invoke(result);
            _onDidHide = null;

            IsInTransition = false;
        }
    }
}