using System.Collections.Generic;
using GridSystem.Shapes;
using UnityEngine;

namespace GridSystem
{
    public class Square : MonoBehaviour
    {
        private readonly List<Line> _lines = new();
        public StickPoints StickPoints { get; private set; }

        public void SetLines(IEnumerable<Line> lines)
        {
            _lines.Clear();
            _lines.AddRange(lines);
        }

        public bool IsComplete()
        {
            foreach (var line in _lines)
            {
                if (!line.IsOccupied)
                    return false;
            }
            return true;
        }

        public IReadOnlyList<Line> Lines => _lines;
    }
}