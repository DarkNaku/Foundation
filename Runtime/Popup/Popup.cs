using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using DarkNaku.Foundation;
using UnityEngine.UI;

namespace DarkNaku.Popup
{
    public class Popup : SingletonBehaviour<Popup>
    {
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private int _baseSortingOrder = 10; 
        [SerializeField] private float _escapeMinInterval = 1f; 
        
        public static IPopupHandler Current => Instance._Current;
        public static bool IsAnyPopupShow => Instance._popups.Count > 0;
        public static UnityEvent<string, IPopupHandler> OnPopupShow => Instance._onPopupShow;
        public static UnityEvent<string, IPopupHandler> OnPopupHide => Instance._onPopupHide;
        
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
                if (_mainGraphicRaycaster == null && _mainCanvas != null)
                {
                    _mainGraphicRaycaster ??= _mainCanvas.GetComponent<GraphicRaycaster>();
                }

                return _mainGraphicRaycaster;
            }
        }
        
        private IPopupHandler _Current => (_popups.Count > 0) ? _popups[^1] : null;

        private List<IPopupHandler> _popups;
        private DateTime _escapePressedTime;
        private GraphicRaycaster _mainGraphicRaycaster;
        private readonly Dictionary<string, IPopupHandler> _popupTable = new ();
        private readonly UnityEvent<string, IPopupHandler> _onPopupShow = new();
        private readonly UnityEvent<string, IPopupHandler> _onPopupHide = new();
        
        public static void Register([NotNull] string key, IPopupHandler handler)
        {
            Instance._Register(key, handler);
        }

        public static void Unregister([NotNull] string key)
        {
            Instance._Unregister(key);
        }

        public static IPopupHandler Show([NotNull] string key)
        {
            return Instance._Show<IPopupHandler>(key);
        }
        
        public static IPopupHandler Hide([NotNull] string key)
        {
            return Instance._Hide<IPopupHandler>(key);
        }

        public static IPopupHandler Hide([NotNull] IPopupHandler handler)
        {
            return Instance._Hide<IPopupHandler>(handler);
        }

        public static T Show<T>([NotNull] string key) where T : class, IPopupHandler
        {
            return Instance._Show<T>(key);
        }
        
        public static T Hide<T>([NotNull] string key) where T : class, IPopupHandler
        {
            return Instance._Hide<T>(key);
        }

        public static T Hide<T>([NotNull] IPopupHandler handler) where T : class, IPopupHandler
        {
            return Instance._Hide<T>(handler);
        }
        
        public static Coroutine HideAll(params string[] excepts)
        {
            return Instance.StartCoroutine(Instance.CoHideAll(excepts));
        }

        public static bool IsPopupShow([NotNull] string key)
        {
            return Instance._popupTable.ContainsKey(key) && Instance._popupTable[key].IsShow;   
        }
        
        public static T Get<T>() where T : class, IPopupHandler
        {
            return Instance._Get<T>();
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
                var handler = item.GetComponent<IPopupHandler>();

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
        
        private void _Register([NotNull] string key, [NotNull] IPopupHandler handler)
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

        private T _Show<T>(string key) where T : class, IPopupHandler
        {
            if (_popupTable.ContainsKey(key) == false)
            {
                Debug.LogErrorFormat("[Popup] CoShowPopup : Popup is not registed - {0}", key);
                return null;
            }

            var popup = _popupTable[key];

            StartCoroutine(CoShow(popup));

            return popup as T;
        }

        private IEnumerator CoShow([NotNull] IPopupHandler handler)
        {
            if (handler.IsInTransition)
            {
                Debug.LogErrorFormat("[Popup] CoShowPopup : {0} is in transition.", handler.Name);
                yield break;
            }

            if (_popups.Contains(handler))
            {
                Debug.LogErrorFormat("[Popup] CoShowPopup : View already shown. - {0}", handler.Name);
                yield break;
            }

            if (_popups.Count > 0)
            {
                var last = _popups.Last();
                last.Interactable = false;
                handler.PopupCanvas.sortingOrder = last.PopupCanvas.sortingOrder + 1;
            }
            else
            {
                if (_mainCanvas == null)
                {
                    handler.PopupCanvas.sortingOrder = _baseSortingOrder;
                }
                else
                {
                    InteractableMainCanvas = false;
                    handler.PopupCanvas.sortingOrder = _mainCanvas.sortingOrder + 1;
                }
            }

            _popups.Add(handler);

            yield return handler.Show();

            _onPopupShow.Invoke(handler.Name, handler);
        }

        private T _Hide<T>([NotNull] string key) where T : class, IPopupHandler
        {
            if (_popups.Count <= 0)
            {
                Debug.LogError("[Popup] Hide : Popup is not show.");
                return null;
            }

            if (_popupTable.ContainsKey(key))
            {
                Debug.LogErrorFormat("[Popup] Hide : Popup is not registed. - {0}", key);
                return null;
            }

            return _Hide<T>(_popupTable[key]);
        }

        private T _Hide<T>([NotNull] IPopupHandler handler) where T : class, IPopupHandler
        {
            if (handler.IsInTransition)
            {
                Debug.LogErrorFormat("[Popup] Hide : {0} is in transition.", handler.Name);
                return null;
            }

            StartCoroutine(CoHide(handler));

            return handler as T;
        }

        private IEnumerator CoHide([NotNull] IPopupHandler handler)
        {
            var isCurrent = _popups.Last() == Current;

            _popups.Remove(handler);

            yield return handler.Hide();

            if (isCurrent)
            {
                if (_popups.Count > 0)
                {
                    _popups.Last().Interactable = true;
                }
                else
                {
                    InteractableMainCanvas = true;
                }
            }

            _onPopupHide.Invoke(handler.Name, handler);
        }
        
        private IEnumerator CoHideAll(params string[] excepts)
        {
            for (int i = _popups.Count - 1; i < _popups.Count; i--)
            {
                var handler = _popups[i];
                
                if (excepts.Contains(handler.Name)) continue;
                
                yield return CoHide(handler);
            }
        }

        private T _Get<T>() where T : class, IPopupHandler
        {
            foreach (var handler in _popupTable.Values)
            {
                if (handler is T) return handler as T;
            }

            return null;
        }
    }
}