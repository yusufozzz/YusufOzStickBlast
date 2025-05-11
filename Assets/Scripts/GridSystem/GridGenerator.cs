using System.Collections.Generic;
using System.Linq;
using GameManagement;
using GridSystem.Sticks;
using GridSystem.Shapes;
using UnityEngine;

namespace GridSystem
{
    public class GridGenerator : MonoBehaviour
    {
        private Dot[,] _dots;
        private Line[,] _horizontalLines;
        private Line[,] _verticalLines;
        private List<Square> _squares;

        /// <summary>
        /// Generates dots, lines, and squares based on the given settings.
        /// </summary>
        public void Generate(GridSettingsSo settings)
        {
            int size = settings.GridSize;
            float spacing = settings.Spacing;
            float offset = (size - 1) * spacing * 0.5f;

            _dots = new Dot[size, size];
            _horizontalLines = new Line[size - 1, size];
            _verticalLines = new Line[size, size - 1];
            _squares = new List<Square>();

            var dotRoot = CreateRoot("Dots");
            var lineRoot = CreateRoot("Lines");
            var squareRoot = CreateRoot("Squares");

            GenerateDots(settings, size, spacing, offset, dotRoot);
            GenerateLines(settings, size, lineRoot);
            GenerateSquares(settings, squareRoot);
        }

        private void GenerateDots(GridSettingsSo settings, int size, float spacing, float offset, Transform parent)
        {
            #region Dots
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector3 pos = new Vector3(x * spacing - offset, y * spacing - offset, 0f);
                    var dot = Instantiate(settings.DotPrefab, pos, Quaternion.identity, parent);
                    dot.name = $"Dot_{x}_{y}";
                    dot.SetCoordinates(x, y);
                    _dots[x, y] = dot;
                }
            }
            #endregion
        }

        private void GenerateLines(GridSettingsSo settings, int size, Transform parent)
        {
            #region Lines
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (x < size - 1)
                        _horizontalLines[x, y] = InstantiateLine(settings.LinePrefab, _dots[x, y], _dots[x + 1, y], parent, true);

                    if (y < size - 1)
                        _verticalLines[x, y] = InstantiateLine(settings.LinePrefab, _dots[x, y], _dots[x, y + 1], parent, false);
                }
            }
            #endregion
        }

        private void GenerateSquares(GridSettingsSo settings, Transform parent)
        {
            #region Squares
            int count = settings.GridSize - 1;
            for (int y = 0; y < count; y++)
            {
                for (int x = 0; x < count; x++)
                {
                    var lines = new[]
                    {
                        _horizontalLines[x, y],
                        _verticalLines[x + 1, y],
                        _horizontalLines[x, y + 1],
                        _verticalLines[x, y]
                    };

                    Vector3 center = (_dots[x, y].transform.position + _dots[x + 1, y + 1].transform.position) * 0.5f;
                    var square = Instantiate(settings.SquarePrefab, center, Quaternion.identity, parent);
                    square.name = $"Square_{x}_{y}";
                    square.SetLines(lines);
                    _squares.Add(square);
                    ManagerType.Grid.GetManager<GridManager>().GridChecker.Register(square);
                }
            }
            #endregion
        }

        private Line InstantiateLine(Line prefab, Dot a, Dot b, Transform parent, bool rotate90)
        {
            var mid = (a.transform.position + b.transform.position) * 0.5f;
            var line = Instantiate(prefab, mid, Quaternion.identity, parent);
            line.SetDots(a, b);

            if (rotate90)
                line.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

            return line;
        }

        /// <summary>
        /// Resets all line previews to default state.
        /// </summary>
        public void ResetPreview()
        {
            foreach (var line in _horizontalLines)
                line.ResetPreview();

            foreach (var line in _verticalLines)
                line.ResetPreview();
        }

        #region Placement Check
        /// <summary>
        /// Determines if the given shape can be placed anywhere on the grid without overlapping occupied lines.
        /// </summary>
        public bool CanShapeBePlaced(Shape shape)
        {
            var pts = shape.StickPoints;
            if (pts == null || !pts.Any())
                return false;

            // Convert stick float positions to integer grid offsets
            var pointData = pts.Select(p => new
            {
                p.IsVertical,
                Pos = Vector2Int.RoundToInt(p.Position)
            }).ToList();

            int minX = pointData.Min(d => d.Pos.x);
            int minY = pointData.Min(d => d.Pos.y);
            int maxX = pointData.Max(d => d.Pos.x);
            int maxY = pointData.Max(d => d.Pos.y);

            int horW = _horizontalLines.GetLength(0);
            int horH = _horizontalLines.GetLength(1);
            int vertW = _verticalLines.GetLength(0);
            int vertH = _verticalLines.GetLength(1);

            int shapeW = maxX - minX + 1;
            int shapeH = maxY - minY + 1;

            for (int yOff = 0; yOff <= vertH - shapeH; yOff++)
            {
                for (int xOff = 0; xOff <= vertW - shapeW; xOff++)
                {
                    bool ok = true;
                    foreach (var d in pointData)
                    {
                        int gx = xOff + (d.Pos.x - minX);
                        int gy = yOff + (d.Pos.y - minY);
                        if (d.IsVertical)
                        {
                            if (gx < 0 || gx >= vertW || gy < 0 || gy >= vertH || _verticalLines[gx, gy].IsOccupied)
                            {
                                ok = false;
                                break;
                            }
                        }
                        else
                        {
                            if (gx < 0 || gx >= horW || gy < 0 || gy >= horH || _horizontalLines[gx, gy].IsOccupied)
                            {
                                ok = false;
                                break;
                            }
                        }
                    }

                    if (ok)
                        return true;
                }
            }

            return false;
        }
        #endregion

        private Transform CreateRoot(string name)
        {
            var root = new GameObject(name).transform;
            root.SetParent(transform);
            return root;
        }
    }
}
