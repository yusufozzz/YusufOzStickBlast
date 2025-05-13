using System.Collections.Generic;
using GameManagement;
using GridSystem.Shapes.ShapeComponents;
using GridSystem.Sticks;
using PoolSystem;
using UnityEngine;

namespace GridSystem.Shapes
{
    public class Shape : MonoBehaviour
    {
        [field: SerializeField]
        public ShapeVisual ShapeVisual { get; private set; }

        [SerializeField]
        private List<StickPoints> stickPoints = new ();

        public IReadOnlyList<StickPoints> StickPoints => stickPoints;

        public List<Stick> Sticks { get; private set; } = new ();

        [SerializeField]
        private ShapeMovement shapeMovement;

        [SerializeField]
        private ShapePlacementValidator shapePlacementValidator;

        private ShapeSettings _shapeSettings;

        public bool IsPlaced { get; private set; }

        public void Initialize(Transform deckSlot)
        {
            _shapeSettings = GeneralSettings.Instance.ShapeSettings;
            shapePlacementValidator.Initialize(this, _shapeSettings);
            ShapeVisual.Initialize(this, _shapeSettings);
            shapeMovement.Initialize(this, _shapeSettings);
            CreateSticks();
            shapeMovement.SetDeckSlot(deckSlot);
        }

        private void CreateSticks()
        {
            var poolManager = ManagerType.Pool.GetManager<PoolManager>();
            foreach (var stickPoint in stickPoints)
            {
                var stick = poolManager.StickPool.GetObject();
                stick.Initialize(stickPoint, transform);
                Sticks.Add(stick);
            }
            ShapeVisual.SetSortingOrder(_shapeSettings.DragSortingOrder);
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
            ShapeVisual.SetSortingOrder(_shapeSettings.PlaceSortingOrder);
            shapePlacementValidator.Place();
            IsPlaced = true;
            ShapeEvents.OnShapePlaced?.Invoke(this);
        }

        public void SendToDeck(Transform deckTransform)
        {
            transform.SetParent(deckTransform);
            transform.localPosition = Vector3.zero;
            IsPlaced = false;
            ShapeVisual.SetSortingOrder(_shapeSettings.DragSortingOrder);
        }
    }
}