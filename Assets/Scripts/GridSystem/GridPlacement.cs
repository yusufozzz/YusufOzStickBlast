using System.Collections.Generic;
using GridSystem.Shapes;
using GridSystem.Sticks;
using UnityEngine;

namespace GridSystem
{
    public class GridPlacement : MonoBehaviour
    {
        [field: SerializeField]
        public float PositionTolerance { get; private set; } = 0.1f;

        [SerializeField]
        private float gridSpacing = 0.96f;

        public Line[,] HorizontalLines { get; private set; }
        public Line[,] VerticalLines { get; private set; }
        
        public void Initialize(Line[,] horizontalLines, Line[,] verticalLines)
        {
            HorizontalLines = horizontalLines;
            VerticalLines = verticalLines;
        }
        
        // New method that works with StickPoints directly
        public bool CanShapeBePlacedUsingStickPoints(IReadOnlyList<StickPoints> stickPoints)
        {
            if (stickPoints == null || stickPoints.Count == 0)
                return false;
                
            // For each available line, check if the shape can be placed
            return TryPlacementAllOverGridUsingStickPoints(stickPoints);
        }
        
        // Try to place the shape at all positions on the grid using StickPoints
        private bool TryPlacementAllOverGridUsingStickPoints(IReadOnlyList<StickPoints> stickPoints)
        {
            // Try for each potential first stick position from StickPoints
            for (int stickIndex = 0; stickIndex < stickPoints.Count; stickIndex++)
            {
                // Current StickPoint
                StickPoints currentStick = stickPoints[stickIndex];
                
                // Get all possible line positions based on stick orientation
                Line[,] lines = currentStick.IsVertical ? VerticalLines : HorizontalLines;
                
                int width = lines.GetLength(0);
                int height = lines.GetLength(1);
                
                // Try each line position as an anchor
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Line line = lines[x, y];
                        
                        // Skip occupied lines
                        if (line.IsOccupied)
                            continue;
                        
                        // Try to place the shape using this line as an anchor for the current stick
                        if (CanPlaceShapeWithAnchor(stickPoints, stickIndex, line.transform.position))
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }
        
        // Check if a shape can be placed using an anchor point for one of its sticks
        private bool CanPlaceShapeWithAnchor(IReadOnlyList<StickPoints> stickPoints, int anchorStickIndex, Vector3 anchorPosition)
        {
            // Get the anchor stick information
            StickPoints anchorStick = stickPoints[anchorStickIndex];
            
            // Check every stick in the shape
            for (int i = 0; i < stickPoints.Count; i++)
            {
                // Skip the anchor stick since it's already placed
                if (i == anchorStickIndex)
                    continue;
                
                // Get current stick info
                StickPoints currentStick = stickPoints[i];
                
                // Calculate relative position from anchor
                Vector2 relativePosition = currentStick.Position - anchorStick.Position;
                
                // Convert to world position
                Vector3 worldOffset = new Vector3(relativePosition.x * gridSpacing, relativePosition.y * gridSpacing, 0);
                Vector3 targetPosition = anchorPosition + worldOffset;
                
                // Find matching line at this position
                Line matchingLine = FindMatchingLine(targetPosition, currentStick.IsVertical);
                
                // If no matching line or the line is occupied, the shape can't be placed
                if (matchingLine == null || matchingLine.IsOccupied)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        // Find a line that matches the position and orientation
        private Line FindMatchingLine(Vector3 position, bool isVertical)
        {
            // Get matching lines
            Line[,] lines = isVertical ? VerticalLines : HorizontalLines;
            int width = lines.GetLength(0);
            int height = lines.GetLength(1);
            
            Line closestLine = null;
            float closestDistance = float.MaxValue;
            
            // Find the closest line
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Line line = lines[x, y];
                    float distance = Vector3.Distance(line.transform.position, position);
                    
                    if (distance < PositionTolerance && distance < closestDistance)
                    {
                        closestLine = line;
                        closestDistance = distance;
                    }
                }
            }
            
            return closestLine;
        }
    }
}