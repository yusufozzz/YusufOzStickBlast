using UnityEngine;

namespace GridSystem.Visuals
{
    public class ItemVisual: MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer mainSpriteRenderer;
        
        [SerializeField]
        private SpriteRenderer previewSpriteRenderer;

        public bool IsPreviewed => previewSpriteRenderer.enabled;

        public void Preview(Color color)
        {
            previewSpriteRenderer.enabled = true;
            previewSpriteRenderer.color = color;
        }
        
        public void ResetPreview()
        {
            previewSpriteRenderer.enabled = false;
            previewSpriteRenderer.color = Color.white;
        }
        
        public void SetColor(Color color)
        {
            mainSpriteRenderer.color = color;
        }
    }
}