using System.Collections.Generic;
using GridSystem.Lines;
using GridSystem.Shapes;
using UnityEngine;

namespace GridSystem.GridSpecific
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

        public bool CanShapeBePlacedUsingStickPoints(IReadOnlyList<StickPoints> stickPoints)
        {
            if (stickPoints == null || stickPoints.Count == 0)
                return false;

            return TryPlacementAllOverGridUsingStickPoints(stickPoints);
        }

        private bool TryPlacementAllOverGridUsingStickPoints(IReadOnlyList<StickPoints> stickPoints)
        {
            for (int stickIndex = 0; stickIndex < stickPoints.Count; stickIndex++)
            {
                StickPoints currentStick = stickPoints[stickIndex];

                Line[,] lines = currentStick.IsVertical ? VerticalLines : HorizontalLines;

                int width = lines.GetLength(0);
                int height = lines.GetLength(1);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Line line = lines[x, y];

                        if (line.IsOccupied)
                            continue;

                        if (CanPlaceShapeWithAnchor(stickPoints, stickIndex, line.transform.position))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool CanPlaceShapeWithAnchor(IReadOnlyList<StickPoints> stickPoints, int anchorStickIndex,
            Vector3 anchorPosition)
        {
            StickPoints anchorStick = stickPoints[anchorStickIndex];

            for (int i = 0; i < stickPoints.Count; i++)
            {
                if (i == anchorStickIndex)
                    continue;

                StickPoints currentStick = stickPoints[i];

                Vector2 relativePosition = currentStick.Position - anchorStick.Position;

                Vector3 worldOffset =
                    new Vector3(relativePosition.x * gridSpacing, relativePosition.y * gridSpacing, 0);
                Vector3 targetPosition = anchorPosition + worldOffset;

                Line matchingLine = FindMatchingLine(targetPosition, currentStick.IsVertical);

                if (matchingLine == null || matchingLine.IsOccupied)
                {
                    return false;
                }
            }

            return true;
        }

        private Line FindMatchingLine(Vector3 position, bool isVertical)
        {
            Line[,] lines = isVertical ? VerticalLines : HorizontalLines;
            int width = lines.GetLength(0);
            int height = lines.GetLength(1);

            Line closestLine = null;
            float closestDistance = float.MaxValue;

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