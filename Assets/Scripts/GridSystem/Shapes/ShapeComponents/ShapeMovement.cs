using AudioSystem;
using GameManagement;
using UnityEngine;

namespace GridSystem.Shapes.ShapeComponents
{
    public class ShapeMovement : ShapeComponent
    {
        private Transform _deckTransform;
        private Camera _mainCamera;

        public override void Initialize(Shape shape, ShapeSettings shapeSettings)
        {
            base.Initialize(shape, shapeSettings);
            _mainCamera = Camera.main;
        }

        private void OnMouseDrag()
        {
            if (Shape.IsPlaced) return;
            UpdatePosition();
            UpdateScale(true);
            CheckPlacement();
        }

        private void UpdateScale(bool isDragging)
        {
            Shape.ShapeVisual.SetScale(isDragging);
        }

        private void UpdatePosition()
        {
            Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            mousePosition.y += ShapeSettings.DragYOffset;
            transform.position = mousePosition;
        }

        private void CheckPlacement()
        {
            Shape.CanPlaced();
        }

        private void OnMouseUp()
        {
            if (Shape.IsPlaced) return;
            UpdateScale(false);
            HandlePlacement();
        }

        private void HandlePlacement()
        {
            var canBePlaced = Shape.CanPlaced();
            if (canBePlaced)
            {
                Shape.Place();
            }
            else
            {
                Shape.SendToDeck(_deckTransform);
            }

            PlaySfx(canBePlaced);
        }

        private void PlaySfx(bool isValid)
        {
            if (isValid)
            {
                SoundType.ShapePlaced.PlaySfx();
            }
            else
            {
                SoundType.ShapePlaceFailed.PlaySfx();
            }
        }

        public void SetDeckSlot(Transform deckSlot)
        {
            _deckTransform = deckSlot;
        }
    }
}