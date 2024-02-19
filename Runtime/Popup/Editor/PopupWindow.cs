using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using DarkNaku.Popup;
using UnityEngine.UI;

namespace DarkNaku.Popup
{
    public class PopupWindow : EditorWindow
    {
        private const string KEY_POPUP_NAME = "CREATE_POPUP_NAME";
        private const string KEY_POPUP_REFERENCE_RESOLUTION_X = "CREATE_POPUP_REFERENCE_RESOLUTION_X";
        private const string KEY_POPUP_REFERENCE_RESOLUTION_Y = "CREATE_POPUP_REFERENCE_RESOLUTION_Y";

        private Vector2Int _referenceResolution = new Vector2Int(1080, 1920);
        private string _popupPath = "_Project/Scripts/PopupHandlers";
        private string _popupName = "";
        private string _script =
@"using UnityEngine;
using DarkNaku.Popup;
        
public class ##CLASS_NAME## : PopupHandler 
{
}";

        [MenuItem("Tools/Create New Popup")]
        public static void ShowWindow()
        {
            GetWindow<PopupWindow>("Create New Popup");
        }

        private void OnGUI()
        {
            _referenceResolution = EditorGUILayout.Vector2IntField("Reference Resolution", _referenceResolution);
            _popupPath = EditorGUILayout.TextField("Popup Path:", _popupPath);
            _popupName = EditorGUILayout.TextField("Popup Name:", _popupName);

            if (GUILayout.Button("Create"))
            {
                CreatePopup();
            }
        }

        private void CreatePopup()
        {
            if (string.IsNullOrEmpty(_popupName))
            {
                Debug.LogError("[PopupWindow] CreatePopup : Name is empty.");
                return;
            }

            var path = Path.Combine(Application.dataPath, _popupPath);
            var handlerName = $"{_popupName}Handler";
            var fileName = $"{handlerName}.cs";

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            string filePath = Path.Combine(path, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(_script.Replace("##CLASS_NAME##", handlerName));
            }

            EditorPrefs.SetString(KEY_POPUP_NAME, _popupName);
            EditorPrefs.SetInt(KEY_POPUP_REFERENCE_RESOLUTION_X, _referenceResolution.x);
            EditorPrefs.SetInt(KEY_POPUP_REFERENCE_RESOLUTION_Y, _referenceResolution.y);

            AssetDatabase.ImportAsset($"Assets/{_popupPath}/{fileName}", ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void ScriptReloaded()
        {
            if (EditorPrefs.HasKey(KEY_POPUP_NAME) == false) return;

            var popupName = EditorPrefs.GetString(KEY_POPUP_NAME);
            var handlerName = $"{popupName}Handler";

            var go = new GameObject(popupName);

            var canvas = go.AddComponent<Canvas>();
            var canvasScaler = go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
            go.AddComponent(Type.GetType($"{handlerName}, Assembly-CSharp"));
            go.transform.SetParent(GetPopupRoot());

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.vertexColorAlwaysGammaSpace = true;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(
                EditorPrefs.GetInt(KEY_POPUP_REFERENCE_RESOLUTION_X, 1080),
                EditorPrefs.GetInt(KEY_POPUP_REFERENCE_RESOLUTION_Y, 1920)
                );

            EditorPrefs.DeleteKey(KEY_POPUP_NAME);
            EditorPrefs.DeleteKey(KEY_POPUP_REFERENCE_RESOLUTION_X);
            EditorPrefs.DeleteKey(KEY_POPUP_REFERENCE_RESOLUTION_Y);
        }

        private static Transform GetPopupRoot()
        {
            Popup popupRoot = null;

            var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (GameObject go in rootGameObjects)
            {
                popupRoot = go.GetComponent<Popup>();

                if (popupRoot != null)
                {
                    break;
                }
            }

            if (popupRoot == null)
            {
                var go = new GameObject("Popup");
                popupRoot = go.AddComponent<Popup>();
            }

            return popupRoot.transform;
        }
    }
}