using GridSystem.Dots;
using GridSystem.Lines;
using UnityEngine;

namespace GridSystem
{
    [CreateAssetMenu(fileName = "GridSettings", menuName = "Game/Grid Settings")]
    public class GridSettingsSo : ScriptableObject
    {
        [field: SerializeField]
        public Dot DotPrefab { get; private set; }

        [field: SerializeField]
        public Line LinePrefab { get; private set; }

        [field: SerializeField]
        public Square SquarePrefab { get; private set; }

        [field: SerializeField]
        public int GridSize { get; private set; } = 6;

        [field: SerializeField]
        public float Spacing { get; private set; } = 3.0f;
    }
}