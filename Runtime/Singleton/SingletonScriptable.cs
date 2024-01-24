using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DarkNaku.Foundation
{
    public abstract class SingletonScriptable<T> : ScriptableObject where T : ScriptableObject
    {
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
                        string resourcePath = System.IO.Path.Combine(Application.dataPath, RESOURCES_PATH);

                        if (System.IO.Directory.Exists(resourcePath) == false)
                        {
                            AssetDatabase.CreateFolder("Assets", RESOURCES_PATH);
                        }

                        string fullPath = System.IO.Path.Combine(System.IO.Path.Combine("Assets", RESOURCES_PATH), AssetName + ".asset");

                        AssetDatabase.CreateAsset(_instance, fullPath);
#endif
                    }

                    if (Application.isPlaying)
                    {
                        (_instance as SingletonScriptable<T>).OnInstantiate();
                    }
                }

                return _instance;
            }
        }

        protected virtual void OnCreateInstance()
        {
        }

        protected virtual void OnInstantiate()
        {
        }
    }
}