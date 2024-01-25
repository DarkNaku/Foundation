using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DarkNaku.Foundation
{
    public abstract class SingletonScriptable<T> : ScriptableObject where T : ScriptableObject
    {
        protected virtual string AssetPath { get; } = RESOURCES_PATH;
        
        private const string RESOURCES_PATH = "Resources";

        private static string _assetName;
        private static string AssetName
        {
            get
            {
                if (string.IsNullOrEmpty(_assetName))
                {
                    _assetName = typeof(T).ToString();
                }

                return _assetName;
            }
        }

        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load(AssetName) as T;

                    if (_instance == null)
                    {
                        _instance = CreateInstance<T>();

                        (_instance as SingletonScriptable<T>).OnCreateInstance();
#if UNITY_EDITOR
                        var assetPath = (_instance as SingletonScriptable<T>).AssetPath;
                        var resourcePath = System.IO.Path.Combine(Application.dataPath, assetPath);

                        if (System.IO.Directory.Exists(resourcePath) == false)
                        {
                            AssetDatabase.CreateFolder("Assets", assetPath);
                        }

                        AssetDatabase.CreateAsset(_instance, $"Assets/{assetPath}/{AssetName}.asset");
#endif
                    }
                }

                return _instance;
            }
        }

        protected virtual void OnCreateInstance()
        {
        }
    }
}