using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour {
    [SerializeField] private RectTransform rectTransform;

    private Rect prevSafeArea;

    private void OnValidate() {
        rectTransform ??= GetComponent<RectTransform>();
    }

    private void LateUpdate() {
        FitToSafeArea();
    }

    public void FitToSafeArea() {
        if (rectTransform == null) return;

        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.min;
        Vector2 anchorMax = safeArea.min + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        prevSafeArea = safeArea;
    }
}