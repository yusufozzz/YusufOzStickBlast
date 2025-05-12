using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class GridSquareChecker : MonoBehaviour
    {
        [SerializeField] 
        private int lineSize;
        
        private Square[,] _squares;
        private HashSet<int> _completedHorizontalLines = new HashSet<int>();
        private HashSet<int> _completedVerticalLines = new HashSet<int>();
        private HashSet<Square> _squaresToClear = new HashSet<Square>();
        
        public void Initialize(Square[,] squares)
        {
            _squares = squares;
            DetermineLineSize();
            ResetTrackedLines();
        }
        
        private void DetermineLineSize()
        {
            if (lineSize <= 0 && _squares != null)
            {
                lineSize = Mathf.Min(_squares.GetLength(0), _squares.GetLength(1));
            }
        }
        
        private void ResetTrackedLines()
        {
            _completedHorizontalLines.Clear();
            _completedVerticalLines.Clear();
            _squaresToClear.Clear();
        }
        
        private void UpdateSquareStates()
        {
            foreach (var square in _squares)
            {
                square.CheckIfCompleted();
            }
        }
        
        public void CheckForCompletedLines()
        {
            _squaresToClear.Clear();
            UpdateSquareStates();
            CheckHorizontalLines();
            CheckVerticalLines();
            
            if (_squaresToClear.Count > 0)
            {
                ProcessCompletedSquares();
            }
        }
        
        private void CheckHorizontalLines()
        {
            int height = _squares.GetLength(1);
            
            for (int y = 0; y < height; y++)
            {
                if (!_completedHorizontalLines.Contains(y))
                {
                    CheckSingleHorizontalLine(y);
                }
            }
        }
        
        private void CheckSingleHorizontalLine(int y)
        {
            int width = _squares.GetLength(0);
            int consecutiveCompleted = 0;
            List<Square> lineSquares = new List<Square>();
            
            for (int x = 0; x < width; x++)
            {
                if (_squares[x, y].IsComplete())
                {
                    consecutiveCompleted++;
                    lineSquares.Add(_squares[x, y]);
                    
                    if (consecutiveCompleted == lineSize)
                    {
                        MarkHorizontalLineCompleted(y, lineSquares);
                        break;
                    }
                }
                else
                {
                    consecutiveCompleted = 0;
                    lineSquares.Clear();
                }
            }
        }
        
        private void MarkHorizontalLineCompleted(int y, List<Square> lineSquares)
        {
            _completedHorizontalLines.Add(y);
            
            foreach (var square in lineSquares)
            {
                _squaresToClear.Add(square);
            }
        }
        
        private void CheckVerticalLines()
        {
            int width = _squares.GetLength(0);
            
            for (int x = 0; x < width; x++)
            {
                if (!_completedVerticalLines.Contains(x))
                {
                    CheckSingleVerticalLine(x);
                }
            }
        }
        
        private void CheckSingleVerticalLine(int x)
        {
            int height = _squares.GetLength(1);
            int consecutiveCompleted = 0;
            List<Square> lineSquares = new List<Square>();
            
            for (int y = 0; y < height; y++)
            {
                if (_squares[x, y].IsComplete())
                {
                    consecutiveCompleted++;
                    lineSquares.Add(_squares[x, y]);
                    
                    if (consecutiveCompleted == lineSize)
                    {
                        MarkVerticalLineCompleted(x, lineSquares);
                        break;
                    }
                }
                else
                {
                    consecutiveCompleted = 0;
                    lineSquares.Clear();
                }
            }
        }
        
        private void MarkVerticalLineCompleted(int x, List<Square> lineSquares)
        {
            _completedVerticalLines.Add(x);
            
            foreach (var square in lineSquares)
            {
                _squaresToClear.Add(square);
            }
        }
        
        private void ProcessCompletedSquares()
        {
            foreach (var square in _squaresToClear)
            {
                square.Clear();
            }
        }
        
        public bool IsGridComplete()
        {
            return IsAllLinesComplete() || IsAllSquaresComplete();
        }
        
        private bool IsAllLinesComplete()
        {
            int width = _squares.GetLength(0);
            int height = _squares.GetLength(1);
            
            return _completedHorizontalLines.Count == height || _completedVerticalLines.Count == width;
        }
        
        private bool IsAllSquaresComplete()
        {
            foreach (var square in _squares)
            {
                if (!square.IsComplete())
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public void Reset()
        {
            ResetTrackedLines();
        }
    }
}