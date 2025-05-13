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
            CheckPlacement();
            SetSortingOrder(100);
        }

        private void UpdatePosition()
        {
            Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            mousePosition.y += 2;
            transform.position = mousePosition;
        }

        private void SetSortingOrder(int i)
        {
            _shape.SetSortingOrder(i);
        }

        private void CheckPlacement()
        {
            _shape.CanPlaced();
        }

        private void OnMouseUp()
        {
            if (_shape.IsPlaced) return;
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