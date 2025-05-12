using System;
using UnityEngine;
using Utilities;

namespace GameManagement
{
    [CreateAssetMenu(fileName = "GeneralSettings", menuName = "ScriptableObjects/GeneralSettings")]
    public class GeneralSettings : ScriptableSingleton<GeneralSettings>
    {
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
}