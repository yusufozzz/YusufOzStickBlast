using System.Collections.Generic;
using GridSystem.Lines;
using GridSystem.Visuals;
using UnityEngine;

namespace GridSystem
{
    public class Dot : MonoBehaviour
    {
        private ItemVisual _itemVisual;
        private readonly List<Line> _lines = new();

        public void SetUp()
        {
            _itemVisual ??= GetComponent<ItemVisual>();
        }

        public void Preview(Color color)
        {
            if (_itemVisual.IsPreviewed) return;
            _itemVisual.Preview(color);
        }

        public void ResetPreview()
        {
            if (!_itemVisual.IsPreviewed) return;
            _itemVisual.ResetPreview();
        }

        public void SetColor(Color color)
        {
            _itemVisual.SetColor(color);
        }

        public void SetLine(Line line)
        {
            _lines.Add(line);
        }
        
        public bool CheckIfThereIsOccupiedLine()
        {
            foreach (var line in _lines)
            {
                if (line.IsOccupied)
                {
                    return true;
                }
            }
            return false;
        }
    }
}