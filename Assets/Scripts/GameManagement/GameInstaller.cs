using System;
using System.Linq;
using UnityEngine;
using YufisUtility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameManagement
{
    public class GameInstaller : MonoBehaviour
    {
        [SerializeField]
        private UnitySerializedDictionary<ManagerType, ManagerBase> managers = new();

        private void Awake()
        {
            SetUpManagers();
        }

        private void SetUpManagers()
        {
            foreach (var manager in managers.Values)
            {
                manager.SetUp();
            }

            ManagerAccess.SetManagers(managers);
        }

#if UNITY_EDITOR
        [ContextMenu("Create All Managers")]
        private void CreateAllManagers()
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }

            foreach (ManagerType type in Enum.GetValues(typeof(ManagerType)))
            {
                GameObject managerObject = new GameObject(type + "Manager");
                managerObject.transform.SetParent(transform);

                string targetClassName = type + "Manager";
                Type managerType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .FirstOrDefault(t => t.Name == targetClassName && typeof(ManagerBase).IsAssignableFrom(t));

                if (managerType != null)
                {
                    ManagerBase managerComponent = (ManagerBase)managerObject.AddComponent(managerType);
                    managerComponent.SetType_Editor(type);
                }
                else
                {
                    Debug.LogError($"Failed to add component for ManagerType.{type}. Class '{targetClassName}' not found or doesn't inherit from ManagerBase.");
                }
            }

            EditorUtility.SetDirty(gameObject);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif

    }
}