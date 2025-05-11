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
            SetColor(GetColor());
        }
        
        public Color GetColor()
        {
            return spriteRenderer.color;
        }

        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }
    }
}