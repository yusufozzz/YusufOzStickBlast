using System.Collections.Generic;
using System.Linq;
using GameManagement;
using GridSystem.Lines;
using GridSystem.Sticks;
using UnityEngine;
using Utilities;

namespace GridSystem.Shapes
{
    public class ShapePlacementValidator: MonoBehaviour
    {
        private Shape _shape;
        private GridManager GridManager => ManagerType.Grid.GetManager<GridManager>();
        private readonly int _lineLayer = 10;
        public void Initialize(Shape shape)
        {
            _shape = shape;
        }
        public bool CanPlaced()
        {
            var canBePlaced = _shape.Sticks.All(IsValidPlacement);
            if (canBePlaced)
            {
                PreviewArea();
            }
            else
            {
                ResetPreviewArea();
            }

            return canBePlaced;
        }
        
        public bool TryPlace()
        {
            if (!CanPlaced()) return false;

            var sticks = _shape.Sticks;
            foreach (var stick in sticks)
            {
                if (TryGetLine(stick.transform.position, out var line))
                {
                    PlaceStick(stick, line);
                }
            }

            ResetPreviewArea();
            return true;
        }

        private void PlaceStick(Stick stick, Line line)
        {
            stick.Place(line);
            line.SetOccupied(stick);
        }
        
        private void PreviewArea()
        {
            var linesToPreview = new HashSet<Line>();
            foreach (var stick in _shape.Sticks)
            {
                if (TryGetLine(stick.transform.position, out var line))
                {
                    linesToPreview.Add(line);
                    line.Preview(stick.GetColor());
                }
            }
        }

        private void ResetPreviewArea()
        {
            GridManager.GridGenerator.ResetPreview();
        }

        private bool IsValidPlacement(Stick stick)
        {
            var canGetLine = TryGetLine(stick.transform.position, out var line);
            if (!canGetLine) return false;
            if (line.IsOccupied) return false;
            var distanceBetweenStickAndLine = stick.transform.position.GetDistance2D(line.transform.position);
            var isCloseEnough = distanceBetweenStickAndLine < 0.3f;
            if (!isCloseEnough) return false;
            var areTheyBothInSameDirection = Vector3.Dot(stick.transform.up, line.transform.up) > 0.2f;
            return areTheyBothInSameDirection;
        }

        private bool TryGetLine(Vector3 stickPoint, out Line line)
        {
            line = null;
            var hit = Physics2D.Raycast(stickPoint, Vector2.zero, Mathf.Infinity, 1 << _lineLayer);
            return hit.collider != null && hit.collider.TryGetComponent(out line);
        }
    }
}