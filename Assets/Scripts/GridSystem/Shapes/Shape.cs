using System.Collections.Generic;
using System.Linq;
using GridSystem.Sticks;
using UnityEngine;

namespace GridSystem.Shapes
{
    public class Shape : MonoBehaviour
    {
        [SerializeField]
        private List<StickPoints> stickPoints = new List<StickPoints>();

        public List<Stick> Sticks { get; private set; } = new List<Stick>();

        [SerializeField]
        private ShapeMovement shapeMovement;

        [SerializeField]
        private ShapePlacementValidator shapePlacementValidator;

        public void Initialize(Transform deckSlot)
        {
            transform.SetPositionAndRotation(deckSlot.position, deckSlot.rotation);
            shapeMovement.Initialize(deckSlot, this);
            shapePlacementValidator.Initialize(this);
            Sticks = GetComponentsInChildren<Stick>().ToList();
        }
        
        public void SetStickPoints(List<StickPoints> points)
        {
            stickPoints = points;
        }

        public void ClearSticks()
        {
            
        }

        public bool CanPlaced()
        {
            return shapePlacementValidator.CanPlaced();
        }

        public void Place()
        {
            ShapeEvents.OnShapePlaced?.Invoke(this);
        }
    }
}