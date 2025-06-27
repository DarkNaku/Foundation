using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour {
    [SerializeField] private RectTransform _rectTransform;

    private Rect _prevSafeArea;
    private Vector3[] _corners = new Vector3[4];

    private void OnValidate() {
        _rectTransform ??= GetComponent<RectTransform>();
    }

    private void LateUpdate() {
        FitToSafeArea();
    }

    public void FitToSafeArea() {
        if (_rectTransform == null) return;
        if (Screen.safeArea == _prevSafeArea) return;

        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.min;
        Vector2 anchorMax = safeArea.min + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;

        _prevSafeArea = safeArea;
    }
}