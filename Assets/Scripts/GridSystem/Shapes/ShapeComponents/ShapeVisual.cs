using DG.Tweening;
using UnityEngine;

namespace GridSystem.Shapes.ShapeComponents
{
    public class ShapeVisual : ShapeComponent
    {
        private Tween _scaleTween;
        private bool _isDragging;

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
                _scaleTween = transform.DOScale(ShapeSettings.DragScale, ShapeSettings.ScaleAnimationDuration)
                    .SetEase(Ease.Linear);
            }
        }

        public void SetSortingOrder(int newOrder)
        {
            Shape.Sticks.ForEach(stick => stick.SetSortingOrder(newOrder));
        }

        public void SetColor(Color activeColor)
        {
            Shape.Sticks.ForEach(stick => stick.SetColor(activeColor));
        }
    }
}