using UnityEngine;

namespace GridSystem.Shapes
{
    public class ShapeVisual : MonoBehaviour
    {
        [field: SerializeField]
        public Color DefaultColor { get; private set; } = Color.white;

        private Shape _shape;

        public void Initialize(Shape shape)
        {
            _shape = shape;
            SetColor(DefaultColor);
        }

        public void SetColor(Color color)
        {
            foreach (var stick in _shape.Sticks)
            {
                stick.SetColor(color);
            }
        }
    }
}