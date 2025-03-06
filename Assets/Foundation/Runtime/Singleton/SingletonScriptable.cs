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

        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var assetName = typeof(T).Name;

                    _instance = Resources.Load(assetName) as T;

                    if (_instance == null)
                    {
                        _instance = CreateInstance<T>();

#if UNITY_EDITOR
                        var assetPath = (_instance as SingletonScriptable<T>).AssetPath;
                        var resourcePath = System.IO.Path.Combine(Application.dataPath, assetPath);

                        if (System.IO.Directory.Exists(resourcePath) == false)
                        {
                            AssetDatabase.CreateFolder("Assets", assetPath);
                        }

                        AssetDatabase.CreateAsset(_instance, $"Assets/{assetPath}/{assetName}.asset");
#endif
                    }

                    if (Application.isPlaying) (_instance as SingletonScriptable<T>).OnInitialize();
                }

                return _instance;
            }
        }

        protected virtual void OnInitialize()
        {
        }
    }
}