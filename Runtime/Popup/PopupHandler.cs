using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DarkNaku.Popup
{
    public abstract class PopupHandler : MonoBehaviour
    {
        public Canvas PopupCanvas
        {
            get
            {
                _popupCanvas ??= GetComponent<Canvas>();

                return _popupCanvas;
            }
        }

        public bool Interactable
        {
            get => PopupGraphicRaycaster == null && PopupGraphicRaycaster.enabled;
            set
            {
                if (PopupGraphicRaycaster != null)
                {
                    PopupGraphicRaycaster.enabled = value;
                }
            }
        }

        public bool IsInTransition { get; protected set; }

        public bool IsOpened => gameObject.activeSelf;

        protected IPopupTransition PopupTransition { get; private set; }
        
        private GraphicRaycaster PopupGraphicRaycaster
        {
            get
            {
                _graphicRaycaster ??= PopupCanvas.GetComponent<GraphicRaycaster>();

                return _graphicRaycaster;
            }
        }

        private Canvas _popupCanvas;
        private GraphicRaycaster _graphicRaycaster;
        private Action<object> _onWillHide;
        private Action<object> _onDidHide;

        public Coroutine Show(object param, System.Action<object> onWillHide, System.Action<object> onDidHide)
        {
            return StartCoroutine(CoShow(param, onWillHide, onDidHide));
        }

        public Coroutine Hide()
        {
            return StartCoroutine(CoHide(null));
        }

        public Coroutine Hide(object result)
        {
            return StartCoroutine(CoHide(result));
        }

        public void Initialize()
        {
            if (PopupCanvas == null)
            {
                Debug.LogError("[PopupHandler] Initialize : Canvas is null.");
                return;
            }
            
            PopupCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            PopupTransition = GetComponent<IPopupTransition>();
            
            OnInitialize();
        }

        public virtual void OnEscape()
        {
            Popup.Hide(this);
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

        private IEnumerator CoShow(object param, System.Action<object> onWillHide, System.Action<object> onDidHide)
        {
            if (IsInTransition)
            {
                Debug.LogErrorFormat("[PopupHandler] CoShow : Already in transition.");
                yield break;
            }

            IsInTransition = true;

            _onWillHide = onWillHide;
            _onDidHide = onDidHide;

            OnWillEnter(param);

            if (PopupTransition != null)
            {
                yield return StartCoroutine(PopupTransition.CoTransitionIn(this));
            }

            OnDidEnter(param);

            Interactable = true;

            IsInTransition = false;
        }

        private IEnumerator CoHide(object result)
        {
            if (IsInTransition)
            {
                Debug.LogErrorFormat("[PopupHandler] CoShow : Already in transition.");
                yield break;
            }

            IsInTransition = true;

            Interactable = false;

            OnWillLeave();

            _onWillHide?.Invoke(result);
            _onWillHide = null;

            if (PopupTransition != null)
            {
                yield return StartCoroutine(PopupTransition.CoTransitionOut(this));
            }

            OnDidLeave();

            _onDidHide?.Invoke(result);
            _onDidHide = null;

            IsInTransition = false;
        }
    }
}