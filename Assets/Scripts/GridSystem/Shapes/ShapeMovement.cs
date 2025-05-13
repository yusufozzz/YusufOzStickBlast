using AudioSystem;
using GameManagement;
using UnityEngine;

namespace GridSystem.Shapes
{
    public class ShapeMovement : MonoBehaviour
    {
        private Transform _deckTransform;
        private Camera _mainCamera;
        private Shape _shape;

        public void Initialize(Transform deckTransform, Shape shape)
        {
            _deckTransform = deckTransform;
            transform.SetParent(null);
            _mainCamera = Camera.main;
            _shape = shape;
        }

        private void OnMouseDrag()
        {
            if (_shape.IsPlaced) return;
            UpdatePosition();
            UpdateScale(true);
            CheckPlacement();
        }

        private void UpdateScale(bool isDragging)
        {
            _shape.ShapeVisual.SetScale(isDragging);
        }

        private void UpdatePosition()
        {
            Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            mousePosition.y += GeneralSettings.Instance.ShapeSettings.DragYOffset;
            transform.position = mousePosition;
        }

        private void CheckPlacement()
        {
            _shape.CanPlaced();
        }

        private void OnMouseUp()
        {
            if (_shape.IsPlaced) return;
            UpdateScale(false);
            HandlePlacement();
        }

        private void HandlePlacement()
        {
            var canBePlaced = _shape.CanPlaced();
            if (canBePlaced)
            {
                _shape.Place();
            }
            else
            {
                _shape.ReturnDeck(_deckTransform);
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
    }
}