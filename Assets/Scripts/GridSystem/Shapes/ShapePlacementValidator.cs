using System.Collections.Generic;
using System.Linq;
using GameManagement;
using GridSystem.GridSpecific;
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
        private HashSet<Line> _linesToPreview = new ();
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
                HighlightArea();
            }
            else
            {
                ResetPreviewArea();
                ResetHighlightArea();
            }

            return canBePlaced;
        }

        private void ResetHighlightArea()
        {
            
        }

        private void HighlightArea()
        {
            
        }

        public void TryPlace()
        {
            if (!CanPlaced()) return;

            var sticks = _shape.Sticks;
            foreach (var stick in sticks)
            {
                if (TryGetLine(stick.transform.position, out var line))
                {
                    PlaceStick(stick, line);
                }
            }

            ResetPreviewArea();
        }

        private void PlaceStick(Stick stick, Line line)
        {
            stick.Place(line);
            line.SetOccupied(stick);
        }
        
        private void PreviewArea()
        {
            foreach (var stick in _shape.Sticks)
            {
                if (TryGetLine(stick.transform.position, out var line) && !_linesToPreview.Contains(line))
                {
                    _linesToPreview.Add(line);
                    line.Preview(stick.GetColor());
                }
            }
        }

        private void ResetPreviewArea()
        {
            GridManager.GridGenerator.ResetPreview();
            _linesToPreview.Clear();
        }

        private bool IsValidPlacement(Stick stick)
        {
            var canGetLine = TryGetLine(stick.transform.position, out var line);
            if (!canGetLine) return false;
            if (line.IsOccupied) return false;
            var distanceBetweenStickAndLine = stick.transform.position.GetDistance2D(line.transform.position);
            var isCloseEnough = distanceBetweenStickAndLine < 1f;
            if (!isCloseEnough) return false;
            var areTheyBothInSameDirection = Vector3.Dot(stick.transform.up, line.transform.up) > 0.3f;
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