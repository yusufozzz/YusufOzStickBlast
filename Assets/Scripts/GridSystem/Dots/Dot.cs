using System.Collections.Generic;
using GridSystem.Lines;
using GridSystem.Visuals;
using UnityEngine;

namespace GridSystem.Dots
{
    public class Dot : MonoBehaviour
    {
        private ItemVisual _itemVisual;
        public List<Line> Lines { get; private set; } = new();

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
            Lines.Add(line);
        }
        
        public bool CheckIfThereIsOccupiedLine()
        {
            foreach (var line in Lines)
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