using GridSystem.Lines;
using GridSystem.Shapes;
using UnityEngine;

namespace GridSystem.Sticks
{
    public class Stick: MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        public void Place(Line line)
        {
            transform.SetParent(line.transform);
            transform.position = line.transform.position;
        }
        
        public Color GetColor()
        {
            return spriteRenderer.color;
        }

        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }

        public void SetSortingOrder(int order)
        {
            spriteRenderer.sortingOrder = order;
        }

        public void Initialize(StickPoints stickPoint, Transform parent)
        {
            transform.SetParent(parent);
            transform.localPosition = stickPoint.Position;
            transform.localRotation = Quaternion.Euler(0, 0, stickPoint.IsVertical ? 0 : 90);
        }
    }
}