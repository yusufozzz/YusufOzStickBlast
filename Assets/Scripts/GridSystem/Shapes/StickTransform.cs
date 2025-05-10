using UnityEngine;

namespace GridSystem.Shapes
{
    public class StickTransform
    {
        public Vector2Int Position { get; }
        public bool IsVertical { get; }

        public StickTransform(Vector2Int position, bool isVertical)
        {
            Position = position;
            IsVertical = isVertical;
        }
    }
}