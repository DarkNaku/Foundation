using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkNaku.Toast
{
    public class ToastView : MonoBehaviour, IPointerClickHandler
    {
        public bool IsUsable => !gameObject.activeSelf;
        
        public RectTransform RT => _rectTransform ??= GetComponent<RectTransform>();
        
        private RectTransform _rectTransform;
        private IToast _toast;
        private Action _onComplete;

        public void Initialize(IToast toast)
        {
            _toast = toast;
            
            gameObject.SetActive(false);
            
            OnInitialize();
        }
        
        public IEnumerator Show(Action onComplete)
        {
            gameObject.SetActive(true);
            
            yield return CoShow();
            
            gameObject.SetActive(false);
            
            onComplete?.Invoke();
        }
        
        public void Dismiss()
        {
            gameObject.SetActive(false);
            
            _onComplete?.Invoke();
        }

        public virtual void Move()
        {
            RT.anchoredPosition += Vector2.up * RT.sizeDelta.y;
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual IEnumerator CoShow()
        {
            yield break;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _toast?.OnClickToastView(this);
        }
    }
}