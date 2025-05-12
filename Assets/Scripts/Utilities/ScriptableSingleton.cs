using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static T _instance;
        public static T Instance {
            get {
                if (_instance == null)
                {
                    _instance = Resources.Load<T>(typeof(T).Name);
                }
                return _instance;
            }
        }

        private void Awake()
        {
            _instance = Resources.Load<T>(typeof(T).Name);
        }
#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            FindDuplicate();
            MoveToResourcesFolder();
        }

        private void FindDuplicate()
        {
            T[] assets = Resources.LoadAll<T>("");
            if (assets.Length > 1)
            {
                Debug.LogWarning("Multiple instances of " + typeof(T).Name + " found in Resources folder. Deleting duplicates.");
                for (int i = 1; i < assets.Length; i++)
                {
                    Debug.LogWarning("Deleted: " + assets[i]);
                    DestroyImmediate(assets[i], true);
                }
            }
        }

        private void MoveToResourcesFolder()
        {
            if (!AssetDatabase.GetAssetPath(this).Contains("Resources"))
            {
                AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(this), "Assets/Resources/" + this.name + ".asset");
            }
        }
#endif

    }
}