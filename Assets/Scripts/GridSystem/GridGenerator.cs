using System.Collections.Generic;
using GameManagement;
using UnityEngine;

namespace GridSystem
{
    public class GridGenerator : MonoBehaviour
    {
        private Dot[,] _dots;
        private Line[,] _horizontalLines;
        private Line[,] _verticalLines;
        private List<Square> _squares = new();

        public void Generate(GridSettingsSo settings)
        {
            int size = settings.GridSize;
            float spacing = settings.Spacing;
            float offset = (size - 1) * spacing * 0.5f;

            _dots = new Dot[size, size];
            _horizontalLines = new Line[size - 1, size];
            _verticalLines = new Line[size, size - 1];

            var dotRoot = CreateRoot("Dots");
            var lineRoot = CreateRoot("Lines");
            var squareRoot = CreateRoot("Squares");

            GenerateDots(settings, size, spacing, offset, dotRoot);
            GenerateLines(settings, size, lineRoot);
            GenerateSquares(settings, squareRoot);
            GetComponent<GridPlacement>().Initialize(_horizontalLines, _verticalLines);
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
                    dot.SetUp();
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
                        _horizontalLines[x, y] = InstantiateLine(settings.LinePrefab, _dots[x, y], _dots[x + 1, y],
                            parent, true);

                    if (y < size - 1)
                        _verticalLines[x, y] = InstantiateLine(settings.LinePrefab, _dots[x, y], _dots[x, y + 1],
                            parent, false);
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

        public void ResetPreview()
        {
            foreach (var line in _horizontalLines)
                line.ResetPreview();

            foreach (var line in _verticalLines)
                line.ResetPreview();
        }

        private Transform CreateRoot(string name)
        {
            var root = new GameObject(name).transform;
            root.SetParent(transform);
            return root;
        }
    }
}