using System;
using UnityEngine;

namespace GridSystem.Shapes
{
    [Serializable]
    public class StickPoints
    {
        [field: SerializeField]
        public Vector2 Position { get; private set; }

        [field: SerializeField]
        public bool IsVertical { get; private set; }

        public StickPoints(Vector2 position, bool isVertical)
        {
            Position = position;
            IsVertical = isVertical;
        }
    }
}