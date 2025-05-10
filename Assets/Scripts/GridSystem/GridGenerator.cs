using GameManagement;
using UnityEngine;

namespace GridSystem
{
    public class GridGenerator : MonoBehaviour
    {
        private Dot[,] _dots;
        private Line[,] _horizontalLines;
        private Line[,] _verticalLines;

        public void Generate(GridSettingsSo settings)
        {
            int gridSize = settings.GridSize;
            float spacing = settings.Spacing;
            float offset = (gridSize - 1) * spacing * 0.5f;

            _dots = new Dot[gridSize, gridSize];
            _horizontalLines = new Line[gridSize - 1, gridSize];
            _verticalLines = new Line[gridSize, gridSize - 1];

            Transform dotRoot = CreateRoot("Dots");
            Transform lineRoot = CreateRoot("Lines");
            Transform squareRoot = CreateRoot("Squares");

            CreateDots(settings, dotRoot, gridSize, spacing, offset);
            CreateLines(settings, lineRoot, gridSize, spacing, offset);
            CreateSquares(settings, squareRoot, gridSize, spacing, offset);
        }

        private void CreateDots(GridSettingsSo settings, Transform parent, int size, float spacing, float offset)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector3 pos = new Vector3(x * spacing - offset, y * spacing - offset, 0f);
                    Dot dot = Instantiate(settings.DotPrefab, pos, Quaternion.identity, parent);
                    dot.name = $"Dot_{x}_{y}";
                    dot.SetCoordinates(x, y);
                    _dots[x, y] = dot;
                }
            }
        }

        private void CreateLines(GridSettingsSo settings, Transform parent, int size, float spacing, float offset)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (x < size - 1)
                    {
                        Dot a = _dots[x, y];
                        Dot b = _dots[x + 1, y];
                        Line line = InstantiateLine(settings.LinePrefab, a, b, parent, isHorizontal: true);
                        _horizontalLines[x, y] = line;
                    }

                    if (y < size - 1)
                    {
                        Dot a = _dots[x, y];
                        Dot b = _dots[x, y + 1];
                        Line line = InstantiateLine(settings.LinePrefab, a, b, parent, isHorizontal: false);
                        _verticalLines[x, y] = line;
                    }
                }
            }
        }

        private void CreateSquares(GridSettingsSo settings, Transform parent, int size, float spacing, float offset)
        {
            for (int y = 0; y < size - 1; y++)
            {
                for (int x = 0; x < size - 1; x++)
                {
                    Line top = _horizontalLines[x, y];
                    Line right = _verticalLines[x + 1, y];
                    Line bottom = _horizontalLines[x, y + 1];
                    Line left = _verticalLines[x, y];

                    Vector3 center = (_dots[x, y].transform.position + _dots[x + 1, y + 1].transform.position) * 0.5f;
                    Square square = Instantiate(settings.SquarePrefab, center, Quaternion.identity, parent);
                    square.name = $"Square_{x}_{y}";
                    square.SetLines(new[] { top, right, bottom, left });

                    ManagerType.Grid.GetManager<GridManager>().GridChecker.Register(square);
                }
            }
        }

        private Line InstantiateLine(Line prefab, Dot a, Dot b, Transform parent, bool isHorizontal)
        {
            Vector3 mid = (a.transform.position + b.transform.position) * 0.5f;
            Line line = Instantiate(prefab, mid, Quaternion.identity, parent);
            line.SetDots(a, b);

            if (isHorizontal)
                line.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

            return line;
        }

        private Transform CreateRoot(string name)
        {
            Transform root = new GameObject(name).transform;
            root.SetParent(transform);
            return root;
        }
    }
}
