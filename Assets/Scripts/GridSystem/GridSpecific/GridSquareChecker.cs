using System.Collections.Generic;
using UnityEngine;

namespace GridSystem.GridSpecific
{
    public class GridSquareChecker : MonoBehaviour
    {
        [SerializeField] 
        private int lineSize;
        
        private Square[,] _squares;
        private readonly HashSet<int> _completedHorizontalLines = new HashSet<int>();
        private readonly HashSet<int> _completedVerticalLines = new HashSet<int>();
        private readonly HashSet<Square> _squaresToClear = new HashSet<Square>();
        
        public void Initialize(Square[,] squares)
        {
            Debug.Log($"GridSquareChecker initialized with {squares.GetLength(0)}x{squares.GetLength(1)} grid");
            _squares = squares;
            DetermineLineSize();
            ResetTrackedLines();
        }
        
        private void DetermineLineSize()
        {
            if (lineSize <= 0 && _squares != null)
            {
                lineSize = Mathf.Min(_squares.GetLength(0), _squares.GetLength(1));
                Debug.Log($"LineSize auto-determined to be {lineSize}");
            }
            else
            {
                Debug.Log($"Using configured lineSize: {lineSize}");
            }
        }
        
        private void ResetTrackedLines()
        {
            int hCount = _completedHorizontalLines.Count;
            int vCount = _completedVerticalLines.Count;
            int squaresCount = _squaresToClear.Count;
            
            _completedHorizontalLines.Clear();
            _completedVerticalLines.Clear();
            _squaresToClear.Clear();
            
            Debug.Log($"Reset tracked lines: Cleared {hCount} horizontal, {vCount} vertical, {squaresCount} squares");
        }
        
        private void UpdateSquareStates()
        {
            Debug.Log("Starting UpdateSquareStates");
            int completedCount = 0;
            
            if (_squares == null)
            {
                Debug.LogError("_squares is null in UpdateSquareStates!");
                return;
            }
            
            foreach (var square in _squares)
            {
                if (square == null)
                {
                    Debug.LogError("Found null square in _squares array!");
                    continue;
                }
                
                bool wasComplete = square.IsComplete();
                square.CheckIfCompleted();
                bool isNowComplete = square.IsComplete();
                
                if (isNowComplete)
                {
                    completedCount++;
                }
                
                if (!wasComplete && isNowComplete)
                {
                    Debug.Log($"Square at {square.transform.position} just completed!");
                }
            }
            
            Debug.Log($"UpdateSquareStates finished. Total completed squares: {completedCount}");
        }
        
        public void CheckForCompletedLines()
        {
            Debug.Log("CheckForCompletedLines started");
            ResetTrackedLines();
            if (_squares == null)
            {
                Debug.LogError("_squares is null in CheckForCompletedLines!");
                return;
            }
            
            _squaresToClear.Clear();
            Debug.Log("Cleared _squaresToClear");
            
            UpdateSquareStates();
            Debug.Log("Finished UpdateSquareStates");
            
            CheckHorizontalLines();
            Debug.Log($"Finished CheckHorizontalLines, found {_completedHorizontalLines.Count} completed horizontal lines");
            
            CheckVerticalLines();
            Debug.Log($"Finished CheckVerticalLines, found {_completedVerticalLines.Count} completed vertical lines");
            
            Debug.Log($"Total squares to clear: {_squaresToClear.Count}");
            
            if (_squaresToClear.Count > 0)
            {
                Debug.Log($"Clearing {_squaresToClear.Count} squares");
                ProcessCompletedSquares();
                Debug.Log("Finished ProcessCompletedSquares");
            }
            else
            {
                Debug.Log("No squares to clear");
            }
            
            Debug.Log("CheckForCompletedLines completed");
        }
        
        private void CheckHorizontalLines()
        {
            Debug.Log("Starting CheckHorizontalLines");
            int height = _squares.GetLength(1);
            
            Debug.Log($"Grid height: {height}");
            Debug.Log($"Previously completed horizontal lines: {string.Join(", ", _completedHorizontalLines)}");
            
            for (int y = 0; y < height; y++)
            {
                Debug.Log($"Checking horizontal line at y={y}");
                
                if (_completedHorizontalLines.Contains(y))
                {
                    Debug.Log($"Skipping y={y} as it was already completed");
                    continue;
                }
                
                CheckSingleHorizontalLine(y);
            }
            
            Debug.Log("Finished CheckHorizontalLines");
        }
        
        private void CheckSingleHorizontalLine(int y)
        {
            Debug.Log($"Checking single horizontal line at y={y}");
            
            int width = _squares.GetLength(0);
            int consecutiveCompleted = 0;
            List<Square> lineSquares = new List<Square>();
            
            Debug.Log($"Grid width: {width}, lineSize required: {lineSize}");
            
            for (int x = 0; x < width; x++)
            {
                if (_squares[x, y] == null)
                {
                    Debug.LogError($"Square at [{x},{y}] is null!");
                    consecutiveCompleted = 0;
                    lineSquares.Clear();
                    continue;
                }
                
                bool isComplete = _squares[x, y].IsComplete();
                Debug.Log($"Square at [{x},{y}] complete: {isComplete}");
                
                if (isComplete)
                {
                    consecutiveCompleted++;
                    lineSquares.Add(_squares[x, y]);
                    Debug.Log($"Found complete square at [{x},{y}], consecutive count: {consecutiveCompleted}");
                    
                    if (consecutiveCompleted == lineSize)
                    {
                        Debug.Log($"Found {lineSize} consecutive completed squares at horizontal line y={y}!");
                        MarkHorizontalLineCompleted(y, lineSquares);
                        break;
                    }
                }
                else
                {
                    Debug.Log($"Found incomplete square at [{x},{y}], resetting consecutive count");
                    consecutiveCompleted = 0;
                    lineSquares.Clear();
                }
            }
            
            Debug.Log($"Finished checking horizontal line at y={y}, max consecutive squares: {consecutiveCompleted}");
        }
        
        private void MarkHorizontalLineCompleted(int y, List<Square> lineSquares)
        {
            Debug.Log($"Marking horizontal line at y={y} as completed");
            _completedHorizontalLines.Add(y);
            
            Debug.Log($"Adding {lineSquares.Count} squares to _squaresToClear");
            
            foreach (var square in lineSquares)
            {
                _squaresToClear.Add(square);
                Debug.Log($"Added square at {square.transform.position} to _squaresToClear");
            }
            
            Debug.Log($"Total squares to clear after marking horizontal line: {_squaresToClear.Count}");
        }
        
        private void CheckVerticalLines()
        {
            Debug.Log("Starting CheckVerticalLines");
            int width = _squares.GetLength(0);
            
            Debug.Log($"Grid width: {width}");
            Debug.Log($"Previously completed vertical lines: {string.Join(", ", _completedVerticalLines)}");
            
            for (int x = 0; x < width; x++)
            {
                Debug.Log($"Checking vertical line at x={x}");
                
                if (_completedVerticalLines.Contains(x))
                {
                    Debug.Log($"Skipping x={x} as it was already completed");
                    continue;
                }
                
                CheckSingleVerticalLine(x);
            }
            
            Debug.Log("Finished CheckVerticalLines");
        }
        
        private void CheckSingleVerticalLine(int x)
        {
            Debug.Log($"Checking single vertical line at x={x}");
            
            int height = _squares.GetLength(1);
            int consecutiveCompleted = 0;
            List<Square> lineSquares = new List<Square>();
            
            Debug.Log($"Grid height: {height}, lineSize required: {lineSize}");
            
            for (int y = 0; y < height; y++)
            {
                if (_squares[x, y] == null)
                {
                    Debug.LogError($"Square at [{x},{y}] is null!");
                    consecutiveCompleted = 0;
                    lineSquares.Clear();
                    continue;
                }
                
                bool isComplete = _squares[x, y].IsComplete();
                Debug.Log($"Square at [{x},{y}] complete: {isComplete}");
                
                if (isComplete)
                {
                    consecutiveCompleted++;
                    lineSquares.Add(_squares[x, y]);
                    Debug.Log($"Found complete square at [{x},{y}], consecutive count: {consecutiveCompleted}");
                    
                    if (consecutiveCompleted == lineSize)
                    {
                        Debug.Log($"Found {lineSize} consecutive completed squares at vertical line x={x}!");
                        MarkVerticalLineCompleted(x, lineSquares);
                        break;
                    }
                }
                else
                {
                    Debug.Log($"Found incomplete square at [{x},{y}], resetting consecutive count");
                    consecutiveCompleted = 0;
                    lineSquares.Clear();
                }
            }
            
            Debug.Log($"Finished checking vertical line at x={x}, max consecutive squares: {consecutiveCompleted}");
        }
        
        private void MarkVerticalLineCompleted(int x, List<Square> lineSquares)
        {
            Debug.Log($"Marking vertical line at x={x} as completed");
            _completedVerticalLines.Add(x);
            
            Debug.Log($"Adding {lineSquares.Count} squares to _squaresToClear");
            
            foreach (var square in lineSquares)
            {
                _squaresToClear.Add(square);
                Debug.Log($"Added square at {square.transform.position} to _squaresToClear");
            }
            
            Debug.Log($"Total squares to clear after marking vertical line: {_squaresToClear.Count}");
        }
        
        private void ProcessCompletedSquares()
        {
            Debug.Log($"ProcessCompletedSquares started with {_squaresToClear.Count} squares");
            
            int index = 0;
            foreach (var square in _squaresToClear)
            {
                if (square == null)
                {
                    Debug.LogError($"Square at index {index} in _squaresToClear is null!");
                    continue;
                }
                
                Debug.Log($"Clearing square {index} at position {square.transform.position}");
                square.Clear();
                Debug.Log($"Cleared square {index}");
                index++;
            }
            
            Debug.Log("ProcessCompletedSquares completed");
        }
    }
}