using System;
using UnityEngine;

namespace GridSystem
{
    public class GridGizmos : MonoBehaviour
    {
        [SerializeField]
        private bool showGizmos = true;
        
        [SerializeField]
        private Color emptyLineColor = new Color(0f, 1f, 0f, 0.5f);
        
        [SerializeField]
        private Color occupiedLineColor = new Color(1f, 0f, 0f, 0.5f);
        
        [SerializeField] 
        private float lineThickness = 0.2f;
        
        [SerializeField]
        private bool showOnlyEmptyLines = false;

        private GridPlacement _gridPlacement;

        private void Awake()
        {
            _gridPlacement = GetComponent<GridPlacement>();
            if (_gridPlacement == null)
            {
                Debug.LogError("GridGizmos requires a GridPlacement component.");
                enabled = false;
                return;
            }
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos || _gridPlacement == null)
                return;
            
            DrawGridLinesGizmos();
        }
        
        private void DrawGridLinesGizmos()
        {
            Line[,] horizontalLines = _gridPlacement.HorizontalLines;
            Line[,] verticalLines = _gridPlacement.VerticalLines;
            
            if (horizontalLines == null || verticalLines == null)
                return;
            
            int horizontalWidth = horizontalLines.GetLength(0);
            int horizontalHeight = horizontalLines.GetLength(1);
            
            int verticalWidth = verticalLines.GetLength(0);
            int verticalHeight = verticalLines.GetLength(1);
            
            for (int y = 0; y < horizontalHeight; y++)
            {
                for (int x = 0; x < horizontalWidth; x++)
                {
                    Line line = horizontalLines[x, y];
                    if (line != null)
                    {
                        if (showOnlyEmptyLines && line.IsOccupied)
                            continue;
                            
                        DrawLineGizmo(line, true);
                    }
                }
            }
            
            for (int y = 0; y < verticalHeight; y++)
            {
                for (int x = 0; x < verticalWidth; x++)
                {
                    Line line = verticalLines[x, y];
                    if (line != null)
                    {
                        if (showOnlyEmptyLines && line.IsOccupied)
                            continue;
                            
                        DrawLineGizmo(line, false);
                    }
                }
            }
        }
        
        private void DrawLineGizmo(Line line, bool isHorizontal)
        {
            if (line == null) return;
            
            Vector3 position = line.transform.position;
            
            Gizmos.color = line.IsOccupied ? occupiedLineColor : emptyLineColor;
            
            float length = 0.96f;
            
            Vector3 start, end;
            
            if (isHorizontal)
            {
                start = position + new Vector3(-length / 2f, 0f, 0f);
                end = position + new Vector3(length / 2f, 0f, 0f);
            }
            else
            {
                start = position + new Vector3(0f, -length / 2f, 0f);
                end = position + new Vector3(0f, length / 2f, 0f);
            }
            
            Gizmos.DrawLine(start, end);
            
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0f);
            
            Vector3 thickness = perpendicular * lineThickness * 0.5f;
            
            Vector3[] corners = new Vector3[4]
            {
                start + thickness,
                start - thickness,
                end - thickness,
                end + thickness
            };
            
            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[1], corners[2]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[3], corners[0]);
        }
        
        public void ToggleGizmos(bool show)
        {
            showGizmos = show;
        }
        
        public void ToggleOnlyEmptyLines(bool onlyEmpty)
        {
            showOnlyEmptyLines = onlyEmpty;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying && showGizmos && _gridPlacement != null)
            {
                DrawGridLinesGizmos();
            }
        }
    }
}