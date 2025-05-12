using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridSystem
{
    public class Square : MonoBehaviour
    {
        private readonly List<Line> _lines = new();

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        public void SetLines(IEnumerable<Line> lines)
        {
            _lines.Clear();
            _lines.AddRange(lines);
        }

        public bool IsComplete()
        {
            return _lines.All(line => line.IsOccupied);
        }

        public void CheckIfCompleted()
        {
            if (IsComplete())
            {
                Complete();
            }
        }

        private void Complete()
        {
            foreach (var line in _lines)
            {
                line.SetAsMemberOfCompletedSquare();
            }
            spriteRenderer.transform.localScale = Vector3.one;
        }

        public void Clear()
        {
            foreach (var line in _lines)
            {
                line.Clear();
            }
            
            spriteRenderer.transform.localScale = Vector3.zero;
        }
    }
}