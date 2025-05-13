using System.Collections.Generic;
using AudioSystem;
using GameManagement;
using GridSystem.Squares;
using HighlightSystem;
using PoolSystem;
using UnityEngine;

namespace GridSystem.GridSpecific
{
    public class GridSquareChecker : MonoBehaviour
    {
        [SerializeField]
        private int lineSize;

        private Square[,] _squares;
        private readonly HashSet<int> _completedOrPreviewedHorizontalSquareGroup = new ();
        private readonly HashSet<int> _completedOrPreviewedVerticalSquareGroup = new ();
        private readonly HashSet<Square> _squaresToClear = new ();
        private readonly HashSet<HighlightData> _highLightDataSet = new ();
        private PoolManager PoolManager => ManagerType.Pool.GetManager<PoolManager>();

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
            _highLightDataSet.Clear();
        }

        public void SimulateHighlight()
        {
            ResetTrackedLines();
            CheckHorizontalMatch();
            CheckVerticalMatch();
            PlayHighlight();
        }

        public void ResetHighlight()
        {
            PoolManager.HighlightPool.ClearAllHighlights();
        }
        
        private void UpdateSquareStates()
        {
            foreach (var square in _squares)
            {
                square.TryComplete();
            }
        }

        public void ProcessMatching()
        {
            UpdateSquareStates();
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
            _completedOrPreviewedHorizontalSquareGroup.Add(y);
            var middleSquare = _squares[2,y];
            _highLightDataSet.Add(new HighlightData(middleSquare.transform.position, false));
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
            _completedOrPreviewedVerticalSquareGroup.Add(x);
            var middleSquare = _squares[x, 2];
            _highLightDataSet.Add(new HighlightData(middleSquare.transform.position, true));
            foreach (var square in lineSquares)
            {
                _squaresToClear.Add(square);
            }
        }

        private void PlayHighlight()
        {
            foreach (var hds in _highLightDataSet)
            {
                var highlight = PoolManager.HighlightPool.GetObject();
                highlight.SetPositionAndRotation(hds.position, hds.isVertical);
                highlight.PlayAnimation();
            }
        }

        private void ClearMatchedSquares()
        {
            foreach (var square in _squaresToClear)
            {
                square.Clear();
            }
            PlaySfx();
        }

        private void PlaySfx()
        {
            SoundType.GridMatch.PlaySfx();
        }
    }
}