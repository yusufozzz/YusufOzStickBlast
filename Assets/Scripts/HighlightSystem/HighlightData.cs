using UnityEngine;

namespace HighlightSystem
{
    public class HighlightData
    {
        public Vector3 position;
        public bool isVertical;

        public HighlightData(Vector3 transformPosition, bool b)
        {
            position = transformPosition;
            isVertical = b;
        }
    }
}