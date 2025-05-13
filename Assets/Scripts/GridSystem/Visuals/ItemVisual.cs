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
        private Color _defaultColor;

        private void Awake()
        {
            _defaultColor = mainSpriteRenderer.color;
        }

        public void Preview(Color color)
        {
            previewSpriteRenderer.enabled = true;
            color.a = 0.5f;
            previewSpriteRenderer.color = color;
        }
        
        public void ResetPreview()
        {
            previewSpriteRenderer.enabled = false;
            previewSpriteRenderer.color = _defaultColor;
        }
        
        public void SetColor(Color color)
        {
            mainSpriteRenderer.color = color;
        }
    }
}