using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameManagement
{
    public class GameInstaller : MonoBehaviour
    {
        [SerializeField]
        private List<ManagerBase> managers;

        private void Awake()
        {
            SetUpManagers();
        }

        private void SetUpManagers()
        {
            ManagerAccess.SetManagers(managers);
            foreach (var manager in managers)
            {
                manager.SetUp();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Create All Managers")]
        private void CreateAllManagers()
        {
            var children = GetComponentsInChildren<ManagerBase>();
            for (int i = children.Length - 1; i >= 0; i--)
            {
                DestroyImmediate(children[i].gameObject);
            }
            managers.Clear();

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
                    managers.Add(managerComponent);
                }
                else
                {
                    Debug.LogError(
                        $"Failed to add component for ManagerType.{type}. Class '{targetClassName}' not found or doesn't inherit from ManagerBase.");
                }
            }

            EditorUtility.SetDirty(gameObject);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}