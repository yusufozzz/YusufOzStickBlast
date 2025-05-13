using GridSystem.Dots;
using GridSystem.Lines;
using GridSystem.Squares;
using UnityEngine;

namespace GridSystem.GridSpecific
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