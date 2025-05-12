using System.Collections.Generic;
using GameManagement;
using GridSystem.Sticks;
using GridSystem.Visuals;
using UnityEngine;

namespace GridSystem
{
    public class Line : MonoBehaviour
    {
        public bool IsOccupied => _stick != null;
        private readonly List<Dot> _dots = new();
        private Stick _stick;
        private ItemVisual _itemVisual;
        public void SetDots(Dot a, Dot b)
        {
            _dots.Clear();
            _dots.Add(a);
            _dots.Add(b);
            _itemVisual ??= GetComponent<ItemVisual>();
        }

        public void SetOccupied(Stick stick)
        {
            _stick = stick;
            SetColor(stick.GetColor());
            Debug.Log($"Line occupied by {stick.name}");
        }

        private void SetColor(Color getColor)
        {
            _itemVisual.SetColor(getColor);
            foreach (var dot in _dots)
            {
                dot.SetColor(getColor);
            }
        }

        public void Preview(Color color)
        {
            if (_itemVisual.IsPreviewed) return;
            _itemVisual.Preview(color);
            foreach (var dot in _dots)
            {
                dot.Preview(color);
            }
        }

        public void ResetPreview()
        {
            if (!_itemVisual.IsPreviewed) return;
            _itemVisual.ResetPreview();
            foreach (var dot in _dots)
            {
                dot.ResetPreview();
            }
        }

        public void Clear()
        {
            if (_stick == null) return;
            Destroy(_stick.gameObject);
            _stick = null;
            foreach (var dot in _dots)
            {
                dot.SetColor(GeneralSettings.Instance.DefaultColorList.dotColor);
            }
            _itemVisual.SetColor(GeneralSettings.Instance.DefaultColorList.lineColor);
        }

        public void SetAsMemberOfCompletedSquare()
        {
        }
    }
}