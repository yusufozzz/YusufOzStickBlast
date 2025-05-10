using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameManagement
{
    public class ManagerBase : MonoBehaviour
    {
        [field: SerializeField]
        public ManagerType Type { get; private set; }

        #region UnityL

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        public virtual void SubscribeEvents()
        {
        }

        public virtual void UnsubscribeEvents()
        {
        }

        public virtual void SetUp()
        {
        }
#if UNITY_EDITOR
        public void SetType_Editor(ManagerType type)
        {
            Type = type;
            EditorUtility.SetDirty(gameObject);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}