using System.Collections.Generic;
using UnityEngine;

namespace GridSystem.Shapes
{
    public class Shape : MonoBehaviour
    {
        [SerializeField] private GameObject stickPrefab;

        private readonly List<GameObject> _spawnedSticks = new();

        #region Public Methods

        public void BuildFromStructure(ShapeStructure structure, float spacing)
        {
            ClearSticks();

            // First create all sticks
            foreach (var stick in structure.StickTransforms)
            {
                Vector3 pos = new Vector3(stick.Position.x * spacing, stick.Position.y * spacing, 0f);
                GameObject obj = Instantiate(stickPrefab, transform);
                obj.transform.localPosition = pos;
                obj.transform.localRotation = Quaternion.Euler(0f, 0f, stick.IsVertical ? 0f : 90f);
                _spawnedSticks.Add(obj);
            }

            // Calculate the exact bounds from renderers to get a precise center point
            Bounds totalBounds = CalculatePreciseBounds();
            
            // Apply the offset to all sticks to center the pivot
            Vector3 pivotOffset = totalBounds.center;
            foreach (var stick in _spawnedSticks)
            {
                stick.transform.localPosition -= pivotOffset;
            }

            // Update the collider to match the new centered bounds
            UpdateCollider();
        }

        #endregion

        #region Private Methods

        private Bounds CalculatePreciseBounds()
        {
            if (_spawnedSticks.Count == 0)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            // Initialize bounds with the first renderer's bounds
            Renderer firstRenderer = _spawnedSticks[0].GetComponentInChildren<Renderer>();
            if (firstRenderer == null)
            {
                // Fallback to transform positions if no renderers are found
                return CalculateBoundsFromTransforms();
            }

            // Convert the world space bounds to local space
            Bounds firstBounds = firstRenderer.bounds;
            Vector3 min = transform.InverseTransformPoint(firstBounds.min);
            Vector3 max = transform.InverseTransformPoint(firstBounds.max);
            Bounds localBounds = new Bounds();
            localBounds.SetMinMax(min, max);

            // Include all other renderers
            for (int i = 1; i < _spawnedSticks.Count; i++)
            {
                Renderer renderer = _spawnedSticks[i].GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    Bounds rendererBounds = renderer.bounds;
                    min = transform.InverseTransformPoint(rendererBounds.min);
                    max = transform.InverseTransformPoint(rendererBounds.max);
                    
                    localBounds.Encapsulate(min);
                    localBounds.Encapsulate(max);
                }
            }

            return localBounds;
        }

        private Bounds CalculateBoundsFromTransforms()
        {
            Bounds bounds = new Bounds(_spawnedSticks[0].transform.localPosition, Vector3.zero);
            
            foreach (var stick in _spawnedSticks)
            {
                bounds.Encapsulate(stick.transform.localPosition);
                
                // Approximate the stick size based on scale
                // This is a fallback, but Renderer-based calculation is more accurate
                float stickLength = 1.0f;  // Assuming the stick's length is 1 unit
                Vector3 size = stick.transform.localScale * stickLength;
                Vector3 halfSize = size * 0.5f;
                
                bounds.Encapsulate(stick.transform.localPosition + new Vector3(halfSize.x, halfSize.y, 0));
                bounds.Encapsulate(stick.transform.localPosition - new Vector3(halfSize.x, halfSize.y, 0));
            }
            
            return bounds;
        }

        private void UpdateCollider()
        {
            var box = TryGetComponent(out BoxCollider2D existingCollider) 
                ? existingCollider 
                : gameObject.AddComponent<BoxCollider2D>();

            if (_spawnedSticks.Count == 0)
            {
                box.size = Vector2.zero;
                box.offset = Vector2.zero;
                return;
            }

            Bounds totalBounds = CalculatePreciseBounds();
            
            box.offset = totalBounds.center;
            box.size = totalBounds.size;
        }

        private void ClearSticks()
        {
            foreach (var stick in _spawnedSticks)
            {
                if (stick != null)
                {
                    #if UNITY_EDITOR
                    DestroyImmediate(stick);
                    #else
                    Destroy(stick);
                    #endif
                }
            }
            _spawnedSticks.Clear();
        }

        #endregion
    }
}