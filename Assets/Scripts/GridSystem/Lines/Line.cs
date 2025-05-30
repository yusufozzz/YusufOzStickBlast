﻿using System.Collections.Generic;
using GameManagement;
using GridSystem.Dots;
using GridSystem.Squares;
using GridSystem.Sticks;
using GridSystem.Visuals;
using PoolSystem;
using UnityEngine;

namespace GridSystem.Lines
{
    public class Line : MonoBehaviour
    {
        public bool IsOccupied => _stick != null;
        public bool IsPreviewed => _itemVisual.IsPreviewed;
        public int MemberOfCompletedSquaresCount => _memberOfCompletedSquares.Count;
        private readonly List<Dot> _dots = new();
        private Stick _stick;
        private ItemVisual _itemVisual;
        private readonly HashSet<Square> _memberOfCompletedSquares = new ();
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
            if (_memberOfCompletedSquares.Count == 0)
            {
                if (_stick == null) return;
                var poolManager = ManagerType.Pool.GetManager<PoolManager>();
                poolManager.StickPool.ReturnObject(_stick);
                _stick = null;
                foreach (var dot in _dots)
                {
                    if(!dot.CheckIfThereIsOccupiedLine())
                        dot.SetColor(GeneralSettings.Instance.DefaultColorList.dotColor);
                }
                _itemVisual.SetColor(GeneralSettings.Instance.DefaultColorList.lineColor);
            }
        }

        public void SetAsMemberOfCompletedSquare(Square square)
        {
            _memberOfCompletedSquares.Add(square);
        }
        
        public void RemoveMemberOfCompletedSquare(Square square)
        {
            _memberOfCompletedSquares.Remove(square);
        }
    }
}