using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class Line : MonoBehaviour
    {
        [SerializeField]
        private List<Dot> dots = new();

        public bool IsOccupied { get; private set; }
        public IReadOnlyList<Dot> Dots => dots;

        public void SetDots(Dot a, Dot b)
        {
            dots.Clear();
            dots.Add(a);
            dots.Add(b);
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