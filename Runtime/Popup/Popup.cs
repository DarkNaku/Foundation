using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Foundation
{
    public class Popup : SingletonBehaviour<Popup> 
    {
        #region Static Property

        public static int BaseSortingOrder
        {
            get => Instance._baseSortingOrder;
            set => Instance._baseSortingOrder = value;
        }

        public static Canvas MainCanvas
        {
            get => Instance._MainCanvas;
            set => Instance._MainCanvas = value;
        }

        public static bool IsPopupOpened => Instance._IsPopupOpened;
        public static PopupHandler Current => Instance._Current;

        #endregion

        private bool _IsPopupOpened => _Current != null;
        private PopupHandler _Current => (_popupStack.Count > 0) ? _popupStack.Peek() : null;

        private Canvas _MainCanvas 
        {
            get => _mainCanvas;
            set {
                _mainCanvas = value;

                if (_mainCanvas != null)
                {
                    _mainCanvasGroup = _mainCanvas.GetComponent<CanvasGroup>();
                }
            }
        }

        private int _baseSortingOrder = 0;
        private Canvas _mainCanvas = null;
        private CanvasGroup _mainCanvasGroup = null;
        private DateTime _escapePressedTime = default;
        private Stack<PopupHandler> _popupStack = new();
        private Dictionary<string, PopupHandler> _popupTable = new();

        #region Static Method 

        public static void Register(string key, PopupHandler handler)
        {
            Instance._Register(key, handler);
        }

        public static void Unregister(string key)
        {
            Instance._Unregister(key);
        }

        public static Coroutine ShowPopup(string key)
        {
            return Instance._ShowPopup(key, null, null, null);
        }

        public static Coroutine ShowPopup(string key, Action<object> onDidHide)
        {
            return Instance._ShowPopup(key, null, null, onDidHide);
        }

        public static Coroutine ShowPopup(string key, object param, Action<object> onDidHide)
        {
            return Instance._ShowPopup(key, param, null, onDidHide);
        }

        public static Coroutine ShowPopup(string key, object param, Action<object> onWillHide, Action<object> onDidHide)
        {
            return Instance._ShowPopup(key, param, onWillHide, onDidHide);
        }

        public static Coroutine HidePopup(PopupHandler handler)
        {
            return Instance._HidePopup(handler, null);
        }

        public static Coroutine HidePopup(PopupHandler handler, object result)
        {
            return Instance._HidePopup(handler, result);
        }

        #endregion

        private void Update() {
            if (Input.GetKeyUp(KeyCode.Escape)) Escape();
        }

        private void _Register(string key, PopupHandler handler)
        {
            Debug.Assert(string.IsNullOrEmpty(key) == false, "[Popup] Register : Key is null or empty.");
            Debug.Assert(handler != null, "[Popup] Register : PopupHandler is null.");

            if (_popupTable.ContainsKey(key)) {
                Debug.LogErrorFormat("[Popup] Register : Popup is already registed - {0}", key);
            }
            else
            {
                handler.gameObject.SetActive(false);
                _popupTable.Add(key, handler);
            }
        }

        private void _Unregister(string key)
        {
            Debug.Assert(string.IsNullOrEmpty(key) == false, "[Popup] Unregister : Key is null or empty.");

            if (_popupTable.ContainsKey(key)) {
                _popupTable.Remove(key);
            }
            else
            {
                Debug.LogErrorFormat("[Popup] Register : Popup is not registed - {0}", key);
            }
        }

        private Coroutine _ShowPopup(string key, object param, Action<object> onWillHide, Action<object> onDidHide) {
            return StartCoroutine(CoShowPopup(key, param, onWillHide, onDidHide));
        }

        private IEnumerator CoShowPopup(string key, object param, System.Action<object> onWillHide, System.Action<object> onDidHide) {
            if (_popupTable.ContainsKey(key) == false) {
                Debug.LogErrorFormat("[Popup] CoShowPopup : Popup is not registed - {0}", key);
                yield break;
            }

            var popup = _popupTable[key];

            if (popup.IsInTransition) {
                Debug.LogErrorFormat("[View] CoShowPopup : {0} is in transition.", key);
                yield break;
            }

            if (_popupStack.Contains(popup)) {
                Debug.LogErrorFormat("[View] CoShowPopup : View already shown. - {0}", key);
                yield break;
            }

            popup.gameObject.SetActive(true);

            if (_popupStack.Count > 0)
            {
                var current = _popupStack.Peek();
                current.PopupCanvasGroup.interactable = false;
                popup.PopupCanvas.sortingOrder = current.PopupCanvas.sortingOrder + 1;
            }
            else
            {
                if (_mainCanvasGroup == null)
                {
                    popup.PopupCanvas.sortingOrder = _baseSortingOrder;
                }
                else
                {
                    _mainCanvasGroup.interactable = false;
                    popup.PopupCanvas.sortingOrder = _mainCanvas.sortingOrder + 1;
                }
            }

            _popupStack.Push(popup);

            yield return popup.Show(param, onWillHide, onDidHide);
        }

        private Coroutine _HidePopup(PopupHandler handler, object result) {
            return StartCoroutine(CoHidePopup(handler, result));
        }

        private IEnumerator CoHidePopup(PopupHandler handler, object result) {
            if (_popupStack.Count <= 0) {
                Debug.LogError("[Popup] CoHidePopup : Popup is not opened.");
                yield break;
            }

            var current = _popupStack.Peek();

            if (handler != current) {
                Debug.LogError("[Popup] CoHidePopup : Hide is only available for top-level popups.");
                yield break;
            }

            if (current.IsInTransition) {
                Debug.LogErrorFormat("[Popup] CoHidePopup : {0} is in transition.", current.name);
                yield break;
            }

            yield return current.Hide(result);

            current.gameObject.SetActive(false);

            _popupStack.Pop();

            if (_popupStack.Count > 0) {
                _popupStack.Peek().PopupCanvasGroup.interactable = true;
            } else {
                if (_mainCanvasGroup != null) {
                    _mainCanvasGroup.interactable = true;
                }
			}
        }

        private void Escape() {
            if (_Current == null) return;
            if (_Current.IsInTransition) return;
            if ((DateTime.Now - _escapePressedTime).TotalSeconds < 1) return;

            Current.OnEscape();

            _escapePressedTime = System.DateTime.Now;
        }
    }
}