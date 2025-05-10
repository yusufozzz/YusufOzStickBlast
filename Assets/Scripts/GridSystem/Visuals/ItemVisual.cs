using UnityEngine;

namespace GridSystem.Visuals
{
    public class ItemVisual: MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer visualSprite;

        public void SetPreviewColor(Color color)
        {
            visualSprite.color = color;
        }
    }
}