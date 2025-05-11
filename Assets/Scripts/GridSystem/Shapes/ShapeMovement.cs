using UnityEngine;

namespace GridSystem.Shapes
{
    public class ShapeMovement: MonoBehaviour
    {
        private  Transform _deckTransform;
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
            mousePosition.y += 3;
            transform.position = mousePosition;
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
                _shape.transform.SetParent(_deckTransform);
                _shape.transform.localPosition = Vector3.zero;
            }
        }
    }
}