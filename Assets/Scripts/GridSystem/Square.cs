using System.Collections.Generic;
using GridSystem.Visuals;
using UnityEngine;

namespace GridSystem
{
    public class Square : MonoBehaviour
    {
        private readonly List<Line> _lines = new();
        private ItemVisual _itemVisual;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            _itemVisual = GetComponent<ItemVisual>();
        }

        public void SetLines(IEnumerable<Line> lines)
        {
            _lines.Clear();
            _lines.AddRange(lines);
        }

        public bool IsComplete()
        {
            foreach (var line in _lines)
            {
                if (!line.IsOccupied) return false;
            }
            
            return true;
        }

        public void CheckIfCompleted()
        {
            Debug.Log(_lines.Count);
            Debug.Log($"Checking square: {gameObject.name}");
            if (IsComplete())
            {
                spriteRenderer.transform.localScale = Vector3.one;
            }
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