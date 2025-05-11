using System.Collections.Generic;
using UnityEngine;

namespace GridSystem.Shapes
{
    public class Shape : MonoBehaviour
    {
        [SerializeField]
        private List<StickPoints> stickPoints = new List<StickPoints>();

        [SerializeField]
        private ShapeMovement shapeMovement;

        public void SetStickPoints(List<StickPoints> points)
        {
            stickPoints = points;
        }

        public void ClearSticks()
        {
            
        }

        public void Initialize(Transform deckSlot)
        {
            transform.SetPositionAndRotation(deckSlot.position, deckSlot.rotation);
            shapeMovement.Initialize(deckSlot, this);
        }

        public bool CanPlaced()
        {
            // Check if the shape can be placed in the current position
            // This is a placeholder implementation
            return false;
        }

        public void Place()
        {
            ShapeEvents.OnShapePlaced?.Invoke(this);
        }
    }
}