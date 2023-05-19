using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace DarkNaku.Foundation
{
    [RequireComponent(typeof(Canvas))]
    public class Fader : SingletonBehaviour<Fader>
    {
        private Canvas FaderCanvas
        {
            get
            {
                if (_faderCanvas == null)
                {
                    _faderCanvas = GetComponent<Canvas>();
                }

                return _faderCanvas;
            }
        }

        private Canvas _faderCanvas;
        private Image _curtain;

        private static Tween _fadeInTween;
        private static Tween _fadeOutTween;

        public static Color FadeColor
        {
            get => Instance._curtain.color;
            set => Instance._curtain.color = value;
        }

        public static void FadeIn(float duration)
        {
            if ((_fadeInTween != null) && _fadeInTween.IsActive()) return;
            if ((_fadeOutTween != null) && _fadeOutTween.IsActive()) _fadeOutTween.Kill();

            _fadeInTween = Instance._curtain.DOFade(0f, duration).OnComplete(() => Instance.gameObject.SetActive(false));
        }

        public static void FadeOut(float duration)
        {
            if ((_fadeOutTween != null) && _fadeOutTween.IsActive()) return;
            if ((_fadeInTween != null) && _fadeInTween.IsActive()) _fadeInTween.Kill();

            Instance.gameObject.SetActive(true);
            _fadeOutTween = Instance._curtain.DOFade(1f, duration);
        }

        protected override void OnInstantiate()
        {
            DontDestroyOnLoad(gameObject);

            FaderCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            FaderCanvas.sortingOrder = 10000;

            if (_curtain == null)
            {
                var go = new GameObject("Curtain");
                var rt = go.AddComponent<RectTransform>();

                rt.SetParent(transform);
                rt.localPosition = Vector3.zero;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;

                _curtain = go.AddComponent<Image>();
                _curtain.sprite = CreateSprite();
                _curtain.color = new Color(0f, 0f, 0f, 0f);
            }

            gameObject.SetActive(false);
        }

        private Sprite CreateSprite()
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        }
    }
}