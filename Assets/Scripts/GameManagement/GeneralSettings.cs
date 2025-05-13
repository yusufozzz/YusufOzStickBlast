using System;
using UnityEngine;
using Utilities;

namespace GameManagement
{
    [CreateAssetMenu(fileName = "GeneralSettings", menuName = "ScriptableObjects/GeneralSettings")]
    public class GeneralSettings : ScriptableSingleton<GeneralSettings>
    {
        [field: SerializeField]
        public ShapeSettings ShapeSettings { get; private set; }
        
        [field: SerializeField]
        public ColorList DefaultColorList { get; private set; }

        [field: SerializeField]
        public float SquareAnimationDuration { get; private set; } = 0.25f;

        [Serializable]
        public class ColorList
        {
            public Color dotColor = Color.white;
            public Color lineColor = Color.white;
        }
    }
    
    [Serializable]
    public class ShapeSettings
    {
        [field: SerializeField]
        public float DragScale { get; private set; } = 1.25f;

        [field: SerializeField]
        public float DragAnimationDuration { get; private set; } = 0.15f;

        [field: SerializeField]
        public int DragSortingOrder { get; private set; } = 100;

        [field: SerializeField]
        public int PlaceSortingOrder { get; private set; } = 40;
    }
}