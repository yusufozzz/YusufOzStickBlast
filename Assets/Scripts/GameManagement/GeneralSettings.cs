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

        [Serializable]
        public class ColorList
        {
            public Color dotColor = Color.white;
            public Color lineColor = Color.white;
        }
    }
}