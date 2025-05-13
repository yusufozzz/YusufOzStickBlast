using System.Collections.Generic;
using System.Linq;
using GameManagement;
using GridSystem.Sticks;
using PoolSystem;
using UnityEngine;

namespace GridSystem.Shapes
{
    public class Shape : MonoBehaviour
    {
        [SerializeField]
        private List<StickPoints> stickPoints = new List<StickPoints>();
        public IReadOnlyList<StickPoints> StickPoints => stickPoints;

        public List<Stick> Sticks { get; private set; } = new List<Stick>();

        [SerializeField]
        private ShapeMovement shapeMovement;

        [SerializeField]
        private ShapePlacementValidator shapePlacementValidator;

        public bool IsPlaced { get; private set; }

        public void Initialize(Transform deckSlot)
        {
            CreateSticks();
            transform.SetPositionAndRotation(deckSlot.position, deckSlot.rotation);
            shapeMovement.Initialize(deckSlot, this);
            shapePlacementValidator.Initialize(this);
        }

        private void CreateSticks()
        {
            var poolManager = ManagerType.Pool.GetManager<PoolManager>();
            foreach (var stickPoint in stickPoints)
            {
                var stick = poolManager.StickPool.GetObject();
                stick.Initialize(stickPoint,transform);
                Sticks.Add(stick);
            }
        }

        public void SetStickPoints(List<StickPoints> points)
        {
            stickPoints = points;
        }

        public void ClearSticks()
        {
            Sticks.Clear();
        }

        public bool CanPlaced()
        {
            return shapePlacementValidator.CanPlaced();
        }

        public void Place()
        {
            SetSortingOrder(40);
            shapePlacementValidator.Place();
            IsPlaced = true;
            ShapeEvents.OnShapePlaced?.Invoke(this);
        }
        
        public void ReturnDeck(Transform deckTransform)
        {
            transform.SetParent(deckTransform);
            transform.localPosition = Vector3.zero;
            IsPlaced = false;
            SetSortingOrder(100);
        }

        public void SetSortingOrder(int order)
        {
            Sticks.ForEach(stick => stick.SetSortingOrder(order));
        }
    }
}