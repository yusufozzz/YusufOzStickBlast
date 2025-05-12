using System.Collections.Generic;
using GridSystem.Shapes;
using GridSystem.Sticks;
using UnityEngine;

namespace GridSystem
{
    public class GridPlacement : MonoBehaviour
    {
        [field: SerializeField]
        public float PositionTolerance { get; private set; } = 0.5f;

        public Line[,] HorizontalLines { get; private set; }
        public Line[,] VerticalLines { get; private set; }
        
        public void Initialize(Line[,] horizontalLines, Line[,] verticalLines)
        {
            HorizontalLines = horizontalLines;
            VerticalLines = verticalLines;
        }
        
        // Check if a shape can be placed in its current orientation anywhere on the grid
        public bool CanShapeBePlaced(Shape shape)
        {
            // Get the current sticks and their orientations
            var sticks = shape.Sticks;
            if (sticks.Count == 0)
                return false;
                
            // For each available line, check if the shape can be placed starting from there
            return TryPlacementAllOverGrid(shape);
        }
        
        // Try to place the shape at all positions on the grid
        private bool TryPlacementAllOverGrid(Shape shape)
        {
            // Get all possible placement positions for the shape's first stick
            var possiblePositions = GetPossiblePositions(shape.Sticks[0]);
            
            foreach (var placementPosition in possiblePositions)
            {
                // Calculate the offset from the current position
                Vector3 offset = placementPosition - shape.Sticks[0].transform.position;
                
                // Check if the shape can be placed with this offset
                if (CanPlaceShapeWithOffset(shape, offset))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        // Get all possible positions for a stick
        private List<Vector3> GetPossiblePositions(Stick stick)
        {
            List<Vector3> positions = new List<Vector3>();
            
            // Check if the stick is vertical or horizontal
            bool isVertical = Mathf.Approximately(stick.transform.eulerAngles.z, 0f);
            
            // Get all lines with matching orientation
            Line[,] lines = isVertical ? VerticalLines : HorizontalLines;
            
            // Get dimensions
            int width = lines.GetLength(0);
            int height = lines.GetLength(1);
            
            // Check all lines
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Line line = lines[x, y];
                    
                    // Skip occupied lines
                    if (line.IsOccupied)
                        continue;
                    
                    positions.Add(line.transform.position);
                }
            }
            
            return positions;
        }
        
        // Check if a shape can be placed with an offset from its current position
        private bool CanPlaceShapeWithOffset(Shape shape, Vector3 offset)
        {
            // Check every stick in the shape
            foreach (var stick in shape.Sticks)
            {
                // Calculate the projected position with offset
                Vector3 projectedPosition = stick.transform.position + offset;
                
                // Find matching line at this position
                Line matchingLine = FindMatchingLine(projectedPosition, stick.transform.eulerAngles.z);
                
                // If no matching line or the line is occupied, the shape can't be placed
                if (matchingLine == null || matchingLine.IsOccupied)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        // Find a line that matches the position and orientation
        private Line FindMatchingLine(Vector3 position, float zRotation)
        {
            // Determine if the stick is vertical or horizontal
            bool isVertical = Mathf.Approximately(zRotation, 0f);
            
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