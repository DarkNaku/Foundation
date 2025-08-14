using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteStretcher : MonoBehaviour {
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool maintainAspectRatio = true;

    private float prevAspect = 0f;

    private void OnValidate() {
        spriteRenderer ??= GetComponent<SpriteRenderer>();
    }

    private void LateUpdate() {
        Stretch();
    }

    private void Stretch() {
        if (spriteRenderer == null) return;
        if (targetCamera == null) return;
        if (Mathf.Approximately(targetCamera.aspect, prevAspect)) return;

        transform.localScale = Vector3.one;

        var sprite = spriteRenderer.sprite;
        var pixelsPerUnit = sprite.pixelsPerUnit;
        var unitWidth = sprite.texture.width / pixelsPerUnit;
        var unitHeight = sprite.texture.height / pixelsPerUnit;
        var screenHeight = targetCamera.orthographicSize * 2f;
        var screenWidth = screenHeight * targetCamera.aspect;
        var scaleX = screenWidth / unitWidth;
        var scaleY = screenHeight / unitHeight;

		switch (spriteRenderer.drawMode) {
            case SpriteDrawMode.Sliced:
            case SpriteDrawMode.Tiled:
                spriteRenderer.size = new Vector2(screenWidth, screenHeight);
                break;
            default:
                if (maintainAspectRatio) {
                    var scale = Mathf.Max(scaleX, scaleY);
                    transform.localScale = new Vector3(scale, scale, 1F);
                } else {
                    transform.localScale = new Vector3(scaleX, scaleY, 1F);
                }
                break;
        }

        prevAspect = targetCamera.aspect;
    }
}
