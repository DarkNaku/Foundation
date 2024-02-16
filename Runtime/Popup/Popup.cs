using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using DarkNaku.Foundation;
using DarkNaku.Foundation.Extension;
using UnityEngine.UI;

namespace DarkNaku.Popup
{
    public class Popup : SingletonBehaviour<Popup>
    {
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private int _baseSortingOrder = 10; 
        [SerializeField] private float _escapeMinInterval = 1f; 
        
        public static PopupHandler Current => Instance._Current;
        public static bool IsAnyPopupOpened => Instance._popups.Count > 0;
        public static UnityEvent<string> OnPopupShow => Instance._onPopupShow;
        public static UnityEvent<string> OnPopupHide => Instance._onPopupHide;
        
        private bool InteractableMainCanvas
        {
            get => MainGraphicRaycaster != null && MainGraphicRaycaster.enabled;
            set
            {
                if (MainGraphicRaycaster != null)
                {
                    MainGraphicRaycaster.enabled = value;
                }
            }
        }
        
        private GraphicRaycaster MainGraphicRaycaster
        {
            get
            {
                if (_mainCanvas != null)
                {
                    _mainGraphicRaycaster ??= _mainCanvas.GetComponent<GraphicRaycaster>();
                }

                return _mainGraphicRaycaster;
            }
        }
        
        private PopupHandler _Current => (_popups.Count > 0) ? _popups[^1] : null;

        private List<PopupHandler> _popups;
        private DateTime _escapePressedTime;
        private GraphicRaycaster _mainGraphicRaycaster;
        private readonly Dictionary<string, PopupHandler> _popupTable = new ();
        private readonly UnityEvent<string> _onPopupShow = new();
        private readonly UnityEvent<string> _onPopupHide = new();
        
        public static void Register([NotNull] string key, PopupHandler handler)
        {
            Instance._Register(key, handler);
        }

        public static void Unregister([NotNull] string key)
        {
            Instance._Unregister(key);
        }

        public static Coroutine Show([NotNull] string key, object param = null, Action<object> onWillHide = null, Action<object> onDidHide = null)
        {
            return Instance.StartCoroutine(Instance.CoShow(key, param, onWillHide, onDidHide));
        }
        
        public static Coroutine Hide([NotNull] string key, object result = null)
        {
            return Instance.StartCoroutine(Instance.CoHide(key, result));
        }

        public static Coroutine Hide([NotNull] PopupHandler handler, object result = null)
        {
            return Instance.StartCoroutine(Instance.CoHide(handler, result));
        }
        
        public static Coroutine HideAll(params string[] excepts)
        {
            return Instance.StartCoroutine(Instance.CoHideAll(excepts));
        }

        public static bool IsPopupOpened([NotNull] string key)
        {
            return Instance._popupTable.ContainsKey(key) && Instance._popupTable[key].IsOpened;   
        }
        
        public static T Get<T>() where T : PopupHandler
        {
            return Instance._Get<T>();
        }
        
        public static T Get<T>([NotNull] string key) where T : PopupHandler
        {
            return Instance._Get<T>(key);
        }
        
        public PopupHandler Get([NotNull] string key)
        {
            return Instance._Get(key);
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            _instance = null;
        }
        
        protected override void OnAwake()
        {
            _popups = new();
            
            FindAllPopup();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape)) Escape();
        }

        private void FindAllPopup()
        {
            _popupTable.Clear();
            
            transform.ForEachChild((item) =>
            {
                var handler = item.GetComponent<PopupHandler>();

                if (handler != null)
                {
                    _Register(item.name, handler);
                }
            });
        }
        
        private void Escape()
        {
            if (_Current == null) return;
            if (_Current.IsInTransition) return;
            if ((DateTime.Now - _escapePressedTime).TotalSeconds < _escapeMinInterval) return;

            Current.OnEscape();

            _escapePressedTime = DateTime.Now;
        }
        
        private void _Register([NotNull] string key, [NotNull] PopupHandler handler)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("[Popup] Register : Key is null or empty.");
                return;
            }

            if (_popupTable.ContainsKey(key))
            {
                Debug.LogErrorFormat("[Popup] Register : Popup is already registed - {0}", key);
                return;
            }
            
            handler.Initialize();
            handler.gameObject.SetActive(false);
            _popupTable.Add(key, handler);
        }

        private void _Unregister([NotNull] string key)
        {
            if (_popupTable.ContainsKey(key))
            {
                _popupTable.Remove(key);
            }
            else
            {
                Debug.LogErrorFormat("[Popup] Register : Popup is not registed - {0}", key);
            }
        }

        private IEnumerator CoShow(string key, object param, Action<object> onWillHide, Action<object> onDidHide)
        {
            if (_popupTable.ContainsKey(key) == false)
            {
                Debug.LogErrorFormat("[Popup] CoShowPopup : Popup is not registed - {0}", key);
                yield break;
            }

            var popup = _popupTable[key];

            if (popup.IsInTransition)
            {
                Debug.LogErrorFormat("[Popup] CoShowPopup : {0} is in transition.", key);
                yield break;
            }

            if (_popups.Contains(popup))
            {
                Debug.LogErrorFormat("[Popup] CoShowPopup : View already shown. - {0}", key);
                yield break;
            }

            popup.gameObject.SetActive(true);

            if (_popups.Count > 0)
            {
                var last = _popups.Last();
                last.Interactable = false;
                popup.PopupCanvas.sortingOrder = last.PopupCanvas.sortingOrder + 1;
            }
            else
            {
                if (_mainCanvas == null)
                {
                    popup.PopupCanvas.sortingOrder = _baseSortingOrder;
                }
                else
                {
                    InteractableMainCanvas = false;
                    popup.PopupCanvas.sortingOrder = _mainCanvas.sortingOrder + 1;
                }
            }

            _popups.Add(popup);

            _onPopupShow.Invoke(key);

            yield return popup.Show(param, onWillHide, onDidHide);
        }

        private IEnumerator CoHide([NotNull] string key, object result)
        {
            if (_popupTable.ContainsKey(key))
            {
                yield return CoHide(_popupTable[key], result);
            }
        }

        private IEnumerator CoHide([NotNull] PopupHandler handler, object result)
        {
            if (_popups.Count <= 0)
            {
                Debug.LogError("[Popup] CoHidePopup : Popup is not opened.");
                yield break;
            }

            if (handler.IsInTransition)
            {
                Debug.LogErrorFormat("[Popup] CoHidePopup : {0} is in transition.", handler.name);
                yield break;
            }
            
            _popups.Remove(handler);

            yield return handler.Hide(result);

            handler.gameObject.SetActive(false);

            if (_popups.Count > 0)
            {
                _popups.Last().Interactable = true;
            }
            else
            {
                InteractableMainCanvas = true;
            }

            _onPopupHide.Invoke(handler.name);
        }
        
        private IEnumerator CoHideAll(params string[] excepts)
        {
            for (int i = _popups.Count - 1; i < _popups.Count; i--)
            {
                var handler = _popups[i];
                
                if (excepts.Contains(handler.name)) continue;
                
                yield return CoHide(handler, null);
            }
        }

        private T _Get<T>() where T : PopupHandler
        {
            foreach (var handler in _popupTable.Values)
            {
                if (handler is T) return handler as T;
            }

            return null;
        }
        
        private T _Get<T>(string key) where T : PopupHandler
        {
            foreach (var keyValue in _popupTable)
            {
                if (keyValue.Key.Equals(key) && keyValue.Value is T)
                {
                    return keyValue.Value as T;
                }
            }

            return null;
        }
        
        private PopupHandler _Get(string key)
        {
            return _popupTable.ContainsKey(key) ? _popupTable[key] : null;
        }
    }
}