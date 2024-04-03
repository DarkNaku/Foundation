using System.Collections.Generic;
using DarkNaku.Foundation;
using UnityEngine;
using UnityEngine.Events;

namespace DarkNaku.Toast
{
    public interface IToast
    {
        void OnClickToastView(ToastView toastView);
    }
    
    [RequireComponent(typeof(Canvas))]
    public class Toast : SingletonBehaviour<Toast>, IToast
    {
        [SerializeField] private ToastView _toastView;
        [SerializeField] private bool _dismissByClick;
        [SerializeField] private bool _useYieldSeat = true;
        [SerializeField, Min(1)] private int _maxVisibleCount = 1;
        [SerializeField] private RectTransform _exposureLocation;

        public static UnityEvent<ToastView> OnClickToast => Instance._onClickToast;
        
        private Canvas ToastCanvas => _toastCanvas ??= GetComponent<Canvas>();
        private UnityEvent<ToastView> _onClickToast = new UnityEvent<ToastView>();
        
        private Canvas _toastCanvas;
        private HashSet<ToastView> _toastViews = new();
        private Queue<ToastView> _toastQueue = new();
        private List<ToastView> _toastViewsInUse = new();
        
        public static T Show<T>() where T : ToastView => Instance._Show<T>();

        public void OnClickToastView(ToastView toastView)
        {
            if (_dismissByClick)
            {
                toastView.Dismiss();
            }
            
            _onClickToast.Invoke(toastView);
        }

        private void LateUpdate()
        {
            if (_toastQueue.Count == 0) return;
            if (_toastViewsInUse.Count >= _maxVisibleCount) return;

            if (_useYieldSeat)
            {
                for (int i = 0; i < _toastViewsInUse.Count; i++)
                {
                    _toastViewsInUse[i].Move();
                }
            }
            
            var toastView = _toastQueue.Dequeue();
            
            _toastViewsInUse.Add(toastView);
            
            StartCoroutine(toastView.Show(() => _toastViewsInUse.Remove(toastView)));
        }
            
        private T _Show<T>() where T : ToastView
        {
            var toastView = GetToastView();
            
            _toastQueue.Enqueue(toastView);
            
            if (_exposureLocation != null)
            {
                toastView.RT.anchoredPosition = _exposureLocation.anchoredPosition;
            }
            else
            {
                toastView.transform.position = transform.position;
            }
            
            return toastView as T;
        }

        private ToastView GetToastView()
        {
            ToastView toastView = null;
            
            foreach (var item in _toastViews)
            {
                if (item.IsUsable && _toastQueue.Contains(item) == false)
                {
                    toastView = item;
                    break;
                }
            }

            if (toastView == null)
            {
                toastView = Instantiate(_toastView, transform);
                
                toastView.Initialize(this);
                
                _toastViews.Add(toastView);
            }

            return toastView;
        }
    }
}