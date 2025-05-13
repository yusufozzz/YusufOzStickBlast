using System.Collections.Generic;
using GameManagement;
using GridSystem.Lines;
using GridSystem.Squares;
using HighlightSystem;
using UnityEngine;

namespace GridSystem.GridSpecific
{
    public class GridSquareChecker : MonoBehaviour
    {
        [SerializeField]
        private int lineSize;

        private Square[,] _squares;
        private readonly HashSet<int> _completedOrPreviewedHorizontalSquareGroup = new HashSet<int>();
        private readonly HashSet<int> _completedOrPreviewedVerticalSquareGroup = new HashSet<int>();
        private readonly HashSet<Square> _squaresToClear = new HashSet<Square>();
        private HighlightManager HighlightManager => ManagerType.Highlight.GetManager<HighlightManager>();

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
            _completedOrPreviewedHorizontalSquareGroup.Clear();
            _completedOrPreviewedVerticalSquareGroup.Clear();
            _squaresToClear.Clear();
        }
        
        public void UpdateSquareStates()
        {
            foreach (var square in _squares)
            {
                square.TryComplete();
            }
        }

        public void SimulateHighlight()
        {
            Debug.Log("Simulating highlight");
            ResetTrackedLines();
            CheckHorizontalMatch();
            CheckVerticalMatch();
            PlayHighlight();
        }

        private void PlayHighlight()
        {
            foreach (var square in _squaresToClear)
            {
                var squarePosition = square.transform.position;
                var highlight = HighlightManager.GetHighlight();
                highlight.SetPosition(squarePosition);
                highlight.PlayAnimation();
            }
        }

        public void ResetHighlight()
        {
            Debug.Log("Resetting highlight");
            HighlightManager.ClearAllHighlights();
        }

        public void ProcessMatching()
        {
            if (_squaresToClear.Count > 0)
            {
                ClearMatchedSquares();
            }
            ResetHighlight();
            ResetTrackedLines();
        }

        private void CheckHorizontalMatch()
        {
            int height = _squares.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                if (_completedOrPreviewedHorizontalSquareGroup.Contains(y))
                    continue;

                CheckSingleHorizontalMatch(y);
            }
        }

        private void CheckSingleHorizontalMatch(int y)
        {
            int width = _squares.GetLength(0);
            int consecutiveCompleted = 0;
            List<Square> lineSquares = new List<Square>();

            for (int x = 0; x < width; x++)
            {
                if (_squares[x, y] == null)
                {
                    consecutiveCompleted = 0;
                    lineSquares.Clear();
                    continue;
                }

                bool isComplete = _squares[x, y].IsCompletedOrPreviewed();

                if (isComplete)
                {
                    consecutiveCompleted++;
                    lineSquares.Add(_squares[x, y]);

                    if (consecutiveCompleted == lineSize)
                    {
                        MarkHorizontalMatchCompleted(y, lineSquares);
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

        private void MarkHorizontalMatchCompleted(int y, List<Square> lineSquares)
        {
            Debug.Log($"Marking horizontal match completed at y: {y}");
            _completedOrPreviewedHorizontalSquareGroup.Add(y);

            foreach (var square in lineSquares)
            {
                _squaresToClear.Add(square);
            }
        }

        private void CheckVerticalMatch()
        {
            int width = _squares.GetLength(0);

            for (int x = 0; x < width; x++)
            {
                if (_completedOrPreviewedVerticalSquareGroup.Contains(x))
                    continue;

                CheckSingleVerticalMatch(x);
            }
        }

        private void CheckSingleVerticalMatch(int x)
        {
            int height = _squares.GetLength(1);
            int consecutiveCompleted = 0;
            List<Square> lineSquares = new List<Square>();

            for (int y = 0; y < height; y++)
            {
                if (_squares[x, y] == null)
                {
                    consecutiveCompleted = 0;
                    lineSquares.Clear();
                    continue;
                }

                bool isComplete = _squares[x, y].IsCompletedOrPreviewed();

                if (isComplete)
                {
                    consecutiveCompleted++;
                    lineSquares.Add(_squares[x, y]);

                    if (consecutiveCompleted == lineSize)
                    {
                        MarkVerticalMatchCompleted(x, lineSquares);
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

        private void MarkVerticalMatchCompleted(int x, List<Square> lineSquares)
        {
            Debug.Log($"Marking vertical match completed at x: {x}");
            _completedOrPreviewedVerticalSquareGroup.Add(x);

            foreach (var square in lineSquares)
            {
                _squaresToClear.Add(square);
            }
        }

        private void ClearMatchedSquares()
        {
            foreach (var square in _squaresToClear)
            {
                if (square == null)
                    continue;

                square.Clear();
            }
        }
    }
}