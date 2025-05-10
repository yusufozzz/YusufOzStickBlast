using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class Line : MonoBehaviour
    {
        public bool IsOccupied { get; private set; }
        public IReadOnlyList<Dot> Dots => _dots;
        private readonly List<Dot> _dots = new();
        public void SetDots(Dot a, Dot b)
        {
            _dots.Clear();
            _dots.Add(a);
            _dots.Add(b);
        }

        public bool TryOccupy()
        {
            if (IsOccupied)
                return false;

            IsOccupied = true;
            return true;
        }

        public void ResetOccupation()
        {
            IsOccupied = false;
        }
    }
}