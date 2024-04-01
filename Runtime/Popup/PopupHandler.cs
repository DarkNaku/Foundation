using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DarkNaku.Popup
{
    public abstract class PopupHandler<T> : MonoBehaviour, IPopupHandler where T : PopupHandler<T>
    {
        public string Name => gameObject.name;

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

        public bool IsShow => gameObject.activeSelf;

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
        private Action<T> _onWillHide;
        private Action<T> _onDidHide;

        public void Initialize()
        {
            if (PopupCanvas == null)
            {
                Debug.LogError("[PopupHandler] Initialize : Canvas is null.");
                return;
            }
            
            PopupCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            PopupTransition = GetComponent<IPopupTransition>();
            gameObject.SetActive(false);
            
            OnInitialize();
        }

        public IEnumerator Show()
        {
            yield return null;

            gameObject.SetActive(true);

            yield return CoShow();
        }

        public IEnumerator Hide()
        {
            yield return null;

            yield return CoHide();

            gameObject.SetActive(false);
        }

        public virtual void OnEscape()
        {
            Popup.Hide(this);
        }

        public T OnWillHide(Action<T> callback)
        {
            _onWillHide = callback;

            return this as T;
        }

        public T OnDidHide(Action<T> callback)
        {
            _onDidHide = callback;

            return this as T;
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnWillShow()
        {
        }

        protected virtual void OnDidShow()
        {
        }

        protected virtual void OnWillHide()
        {
        }

        protected virtual void OnDidHide()
        {
        }

        private IEnumerator CoShow()
        {
            if (IsInTransition)
            {
                Debug.LogErrorFormat("[PopupHandler] CoShow : Already in transition.");
                yield break;
            }

            IsInTransition = true;

            OnWillShow();

            if (PopupTransition != null)
            {
                yield return PopupTransition.CoTransitionIn();
            }

            OnDidShow();

            Interactable = true;

            IsInTransition = false;
        }

        private IEnumerator CoHide()
        {
            if (IsInTransition)
            {
                Debug.LogErrorFormat("[PopupHandler] CoShow : Already in transition.");
                yield break;
            }

            IsInTransition = true;

            Interactable = false;

            OnWillHide();

            _onWillHide?.Invoke(this as T);
            _onWillHide = null;

            if (PopupTransition != null)
            {
                yield return PopupTransition.CoTransitionOut();
            }

            OnDidHide();

            _onDidHide?.Invoke(this as T);
            _onDidHide = null;

            IsInTransition = false;
        }
    }
}