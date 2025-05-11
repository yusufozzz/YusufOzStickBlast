using UnityEngine;

namespace GridSystem.Shapes
{
    public class ShapeMovement: MonoBehaviour
    {
        private  Transform _deckTransform;
        private Camera _mainCamera;
        private Shape _shape;
        private bool _isPlaced;
        
        public void Initialize(Transform deckTransform, Shape shape)
        {
            _deckTransform = deckTransform;
            transform.SetParent(null);
            _mainCamera = Camera.main;
            _shape = shape;
        }

        private void OnMouseDrag()
        {
            UpdatePosition();
            CheckPlacement();
        }

        private void CheckPlacement()
        {
            _shape.CanPlaced();
        }

        private void OnMouseUp()
        {
            HandlePlacement();
        }

        private void UpdatePosition()
        {
            Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            mousePosition.y += 2;
            transform.position = mousePosition;
        }

        private void HandlePlacement()
        {
            var canBePlaced = _shape.CanPlaced() && !_isPlaced;
            if (canBePlaced)
            {
                _shape.Place();
                _isPlaced = true;
            }
            else
            {
                _shape.transform.SetParent(_deckTransform);
                _shape.transform.localPosition = Vector3.zero;
            }
        }
    }
}