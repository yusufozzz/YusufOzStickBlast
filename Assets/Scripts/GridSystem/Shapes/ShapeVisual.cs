using DG.Tweening;
using GameManagement;
using UnityEngine;

namespace GridSystem.Shapes
{
    public class ShapeVisual : MonoBehaviour
    {
        [field: SerializeField]
        public Color DefaultColor { get; private set; } = Color.white;

        private Shape _shape;
        private Tween _scaleTween;
        private ShapeSettings _shapeSettings;
        private bool _isDragging;

        public void Initialize(Shape shape)
        {
            _shape = shape;
            _shapeSettings = GeneralSettings.Instance.ShapeSettings;
        }
        
        public void SetScale(bool isDragging)
        {
            if (_isDragging == isDragging) return;
            _isDragging = isDragging;
            if (!_isDragging)
            {
                transform.localScale = Vector3.one;
            }
            else
            {
                _scaleTween?.Kill();
                _scaleTween = transform.DOScale(_shapeSettings.DragScale, _shapeSettings.ScaleAnimationDuration).SetEase(Ease.Linear);
            }
        }
    }
}