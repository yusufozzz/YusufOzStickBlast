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
        private readonly Dictionary<Stick, Line> _linesToPreview = new();
        private readonly Dictionary<Stick, Line> _lastPreviewedLines = new();
        private bool _isPreviewActive = false;
        
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
        
        private void HighlightArea()
        {
            GridManager.GridSquareChecker.SimulateHighlight();
        }

        public void Place()
        {
            foreach (var stick in _linesToPreview.Keys)
            {
                stick.Place(_linesToPreview[stick]);
                _linesToPreview[stick].SetOccupied(stick);
            }

            ResetPreviewArea();
        }
        
        private void PreviewArea()
        {
            if (_isPreviewActive && ArePreviewLinesEqual()) return;
            
            if (_isPreviewActive)
            {
                GridManager.GridGenerator.ResetPreview();
                _linesToPreview.Clear();
            }
            
            Debug.Log("Previewing area - configuration changed");
            
            foreach (var stick in _shape.Sticks)
            {
                if (TryGetLine(stick.transform.position, out var line) && !_linesToPreview.ContainsKey(stick))
                {
                    _linesToPreview.Add(stick, line);
                    line.Preview(stick.GetColor());
                }
            }
            HighlightArea();
            UpdateLastPreviewedLines();
            
            _isPreviewActive = true;
        }
        
        private bool ArePreviewLinesEqual()
        {
            if (_linesToPreview.Count != _lastPreviewedLines.Count) return false;
            
            foreach (var kvp in _linesToPreview)
            {
                Stick stick = kvp.Key;
                Line line = kvp.Value;
                
                if (!_lastPreviewedLines.TryGetValue(stick, out var lastLine))
                    return false;
                
                if (lastLine != line)
                    return false;
            }
            
            return true;
        }
        
        private void UpdateLastPreviewedLines()
        {
            _lastPreviewedLines.Clear();
            
            foreach (var kvp in _linesToPreview)
            {
                _lastPreviewedLines.Add(kvp.Key, kvp.Value);
            }
        }

        private void ResetPreviewArea()
        {
            GridManager.GridGenerator.ResetPreview();
            GridManager.GridSquareChecker.ResetHighlight();
            _linesToPreview.Clear();
            _lastPreviewedLines.Clear();
            _isPreviewActive = false;
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