using GridSystem.Dots;
using GridSystem.Lines;
using GridSystem.Squares;
using UnityEngine;

namespace GridSystem.GridSpecific
{
    public class GridGenerator : MonoBehaviour
    {
        private Dot[,] _dots;
        public Line[,] HorizontalLines { get; private set; }
        public Line[,] VerticalLines { get; private set; }
        public Square[,] Squares { get; private set; }

        public void Generate(GridSettingsSo settings)
        {
            int size = settings.GridSize;
            float spacing = settings.Spacing;
            float offset = (size - 1) * spacing * 0.5f;

            _dots = new Dot[size, size];
            HorizontalLines = new Line[size - 1, size];
            VerticalLines = new Line[size, size - 1];
            Squares = new Square[size - 1, size - 1];

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
                        HorizontalLines[x, y] = InstantiateLine(settings.LinePrefab, _dots[x, y], _dots[x + 1, y],
                            parent, true);

                    if (y < size - 1)
                        VerticalLines[x, y] = InstantiateLine(settings.LinePrefab, _dots[x, y], _dots[x, y + 1],
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
                        HorizontalLines[x, y],
                        VerticalLines[x + 1, y],
                        HorizontalLines[x, y + 1],
                        VerticalLines[x, y]
                    };

                    Vector3 center = (_dots[x, y].transform.position + _dots[x + 1, y + 1].transform.position) * 0.5f;
                    var square = Instantiate(settings.SquarePrefab, center, Quaternion.identity, parent);
                    square.SetLines(lines);
                    Squares[x, y] = square;
                }
            }

            #endregion
        }

        private Line InstantiateLine(Line prefab, Dot dotA, Dot dotB, Transform parent, bool rotate90)
        {
            var mid = (dotA.transform.position + dotB.transform.position) * 0.5f;
            var line = Instantiate(prefab, mid, Quaternion.identity, parent);
            line.SetDots(dotA, dotB);
            dotA.SetLine(line);
            dotB.SetLine(line);

            if (rotate90)
                line.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

            return line;
        }

        public void ResetPreview()
        {
            foreach (var line in HorizontalLines)
                line.ResetPreview();

            foreach (var line in VerticalLines)
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