using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using DarkNaku.Popup;
using UnityEngine.UI;

public class PopupWindow : EditorWindow
{
    private string _popupName = "";
    private string _scriptTemplate = 
@"using UnityEngine;
using DarkNaku.Popup;
        
public class #SCRIPTNAME# : PopupHandler 
{
}";
    
    [MenuItem("Tools/PopupWindow")]
    public static void ShowWindow()
    {
        GetWindow<PopupWindow>("Create Popup");
    }

    private void OnGUI()
    {
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

        var popupPath = "Scripts/Popup";
        var path = Path.Combine(Application.dataPath, popupPath);
        var handlerName = $"{_popupName}Handler";
        var fileName = $"{handlerName}.cs";

        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }

        string filePath = Path.Combine(path, fileName);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.Write(_scriptTemplate.Replace("#SCRIPTNAME#", handlerName));
        }

        AssetDatabase.Refresh();

        var go = new GameObject(_popupName);

        go.AddComponent<Canvas>();
        go.AddComponent<CanvasScaler>();
        go.AddComponent<GraphicRaycaster>();

        var script = AssetDatabase.LoadAssetAtPath<MonoScript>($"Assets/{popupPath}/{fileName}");

        Debug.Log(script.GetClass());

        if (script != null)
        {
            go.AddComponent(script.GetClass());
        }

        var popupRoot = GetPopupRoot();
        go.transform.SetParent(popupRoot);
    }

    private Transform GetPopupRoot()
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