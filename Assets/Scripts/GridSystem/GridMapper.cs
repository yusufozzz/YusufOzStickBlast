using System.Collections.Generic;
using System.Text;
using GridSystem.Shapes;
using GridSystem.Sticks;
using UnityEngine;

namespace GridSystem
{
    public class GridMapper : MonoBehaviour
    {
        [SerializeField]
        private float positionTolerance = 0.5f;
        
        private Line[,] _horizontalLines;
        private Line[,] _verticalLines;
        private int _gridSize;
        
        private void OnEnable()
        {
            ShapeEvents.RefreshGridMap += RefreshGridMap;
        }
        
        private void OnDisable()
        {
            ShapeEvents.RefreshGridMap -= RefreshGridMap;
        }

        public void Initialize(Line[,] horizontalLines, Line[,] verticalLines, int gridSize)
        {
            _horizontalLines = horizontalLines;
            _verticalLines = verticalLines;
            _gridSize = gridSize;
            
            // Initial debug
            DebugGridState();
        }

        public void RefreshGridMap()
        {
            DebugGridState();
        }

        private Line FindLineForStick(Stick stick)
        {
            Vector3 stickPosition = stick.transform.position;
            bool isVertical = Mathf.Approximately(stick.transform.eulerAngles.z, 0f);
            
            Line closestLine = null;
            float closestDistance = float.MaxValue;
            
            if (isVertical)
            {
                for (int y = 0; y < _gridSize - 1; y++)
                {
                    for (int x = 0; x < _gridSize; x++)
                    {
                        Line line = _verticalLines[x, y];
                        float distance = Vector3.Distance(line.transform.position, stickPosition);
                        
                        if (distance < positionTolerance && distance < closestDistance)
                        {
                            closestLine = line;
                            closestDistance = distance;
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < _gridSize; y++)
                {
                    for (int x = 0; x < _gridSize - 1; x++)
                    {
                        Line line = _horizontalLines[x, y];
                        float distance = Vector3.Distance(line.transform.position, stickPosition);
                        
                        if (distance < positionTolerance && distance < closestDistance)
                        {
                            closestLine = line;
                            closestDistance = distance;
                        }
                    }
                }
            }
            
            return closestLine;
        }

        public void DebugGridState()
        {
            // Single comprehensive grid visualization
            StringBuilder gridDebug = new StringBuilder("Grid State - Lines (X=occupied, O=empty):\n\n");
            
            // Build a visual representation of the grid with all lines
            // We'll use a grid that's (2*gridSize-1) x (2*gridSize-1)
            // Odd rows/columns will represent dots, even rows/columns will represent lines
            
            char[,] visualGrid = new char[2 * _gridSize - 1, 2 * _gridSize - 1];
            
            // Initialize with empty spaces
            for (int y = 0; y < 2 * _gridSize - 1; y++)
            {
                for (int x = 0; x < 2 * _gridSize - 1; x++)
                {
                    visualGrid[y, x] = ' ';
                }
            }
            
            // Mark dots with '+'
            for (int y = 0; y < _gridSize; y++)
            {
                for (int x = 0; x < _gridSize; x++)
                {
                    visualGrid[y * 2, x * 2] = '+';
                }
            }
            
            // Mark horizontal lines
            for (int y = 0; y < _gridSize; y++)
            {
                for (int x = 0; x < _gridSize - 1; x++)
                {
                    Line line = _horizontalLines[x, y];
                    visualGrid[y * 2, x * 2 + 1] = line.IsOccupied ? 'X' : 'O';
                }
            }
            
            // Mark vertical lines
            for (int y = 0; y < _gridSize - 1; y++)
            {
                for (int x = 0; x < _gridSize; x++)
                {
                    Line line = _verticalLines[x, y];
                    visualGrid[y * 2 + 1, x * 2] = line.IsOccupied ? 'X' : 'O';
                }
            }
            
            // Output the visual grid (bottom-to-top to match Unity's Y-up)
            for (int y = 2 * _gridSize - 2; y >= 0; y--)
            {
                for (int x = 0; x < 2 * _gridSize - 1; x++)
                {
                    gridDebug.Append(visualGrid[y, x]);
                }
                gridDebug.AppendLine();
            }
            
            // Add counts of occupied lines
            int horizontalOccupied = 0;
            int verticalOccupied = 0;
            
            for (int y = 0; y < _gridSize; y++)
            {
                for (int x = 0; x < _gridSize - 1; x++)
                {
                    if (_horizontalLines[x, y].IsOccupied) horizontalOccupied++;
                }
            }
            
            for (int y = 0; y < _gridSize - 1; y++)
            {
                for (int x = 0; x < _gridSize; x++)
                {
                    if (_verticalLines[x, y].IsOccupied) verticalOccupied++;
                }
            }
            
            gridDebug.AppendLine();
            gridDebug.AppendLine($"Occupied lines: {horizontalOccupied + verticalOccupied} of {(_gridSize-1)*_gridSize*2}");
            gridDebug.AppendLine($"  Horizontal: {horizontalOccupied} of {(_gridSize-1)*_gridSize}");
            gridDebug.AppendLine($"  Vertical: {verticalOccupied} of {_gridSize*(_gridSize-1)}");
            
            Debug.Log(gridDebug.ToString());
        }

        // Method to debug a specific stick's position and find closest line
        public void DebugStickPlacement(Stick stick)
        {
            Vector3 position = stick.transform.position;
            bool isVertical = Mathf.Approximately(stick.transform.eulerAngles.z, 0f);
            
            StringBuilder debug = new StringBuilder($"Stick at {position}, IsVertical: {isVertical}\n");
            
            Line closestLine = FindLineForStick(stick);
            if (closestLine != null)
            {
                debug.AppendLine($"Closest line found at {closestLine.transform.position}");
                debug.AppendLine($"Distance: {Vector3.Distance(closestLine.transform.position, position)}");
                debug.AppendLine($"Line is occupied: {closestLine.IsOccupied}");
            }
            else
            {
                debug.AppendLine("No matching line found within tolerance");
            }
            
            Debug.Log(debug.ToString());
        }

        public Line[,] GetVerticalLines()
        {
            return _verticalLines;
        }
        
        public Line[,] GetHorizontalLines()
        {
            return _horizontalLines;
        }
    }
}