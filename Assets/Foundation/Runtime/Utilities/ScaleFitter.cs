using UnityEngine;

[ExecuteInEditMode]
public class ScaleFitter : MonoBehaviour {
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector2Int _limitResolution = new Vector2Int(640, 960);
    [SerializeField] private Vector2Int _referenceResolution = new Vector2Int(640, 1136);

    private float _limitAspect = -1f;
    private float LimitAspect {
        get {
            if ((_limitAspect < 0f) || (_limitResolution != _prevLimitResolution)) {
                _limitAspect = (_limitResolution.x / (_limitResolution.y * 1f));
            }

            return _limitAspect;
        }
    }

    private float _referenceAspect = -1f;
    private float ReferenceAspect {
        get {
            if ((_referenceAspect < 0f) || (_referenceResolution != _prevReferenceResolution)) {
                _referenceAspect = (_referenceResolution.x / (_referenceResolution.y * 1f));
            }

            return _referenceAspect;
        }
    }

    private Vector2Int _prevLimitResolution = Vector2Int.zero;
    private Vector2Int _prevReferenceResolution = Vector2Int.zero;
    private float _aspect;

    private void LateUpdate() {
        UpdateScale();
    }

    private void UpdateScale() {
        if (_camera == null) return;
        if (_camera.aspect == _aspect) return;

        if (_camera.aspect > LimitAspect) {
            transform.localScale = Vector3.one;
        } else {
            transform.localScale = Vector3.one * (_camera.aspect / ReferenceAspect);
        }

        _aspect = _camera.aspect;
    }
}