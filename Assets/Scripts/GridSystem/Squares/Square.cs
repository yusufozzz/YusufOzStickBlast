﻿using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameManagement;
using GridSystem.Lines;
using ParticleSystem;
using UnityEngine;

namespace GridSystem.Squares
{
    public class Square : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        
        private readonly List<Line> _lines = new();
        private bool _isCompleted;
        private Tween _scaleTween;

        public void SetLines(IEnumerable<Line> lines)
        {
            _lines.Clear();
            _lines.AddRange(lines);
        }

        private bool IsCompleted()
        {
            return _lines.All(line => line.IsOccupied);
        }
        
        public bool IsCompletedOrPreviewed()
        {
            return _lines.All(line => line.IsOccupied || line.IsPreviewed);
        }

        public void TryComplete()
        {
            if (IsCompleted() && !_isCompleted)
            {
                _isCompleted = true;
                Complete();
            }
        }

        private void Complete()
        {
            foreach (var line in _lines)
            {
                line.SetAsMemberOfCompletedSquare(this);
            }

            Animate();
        }

        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }

        private void Animate()
        {
            _scaleTween?.Kill();
            _scaleTween = spriteRenderer.transform.DOScale(Vector3.one, GeneralSettings.Instance.SquareAnimationDuration);
        }

        public void Clear()
        {
            _isCompleted = false;
            _scaleTween?.Kill();
            foreach (var line in _lines)
            {
                line.RemoveMemberOfCompletedSquare(this);
                line.Clear();
            }
            PlayParticle();
            spriteRenderer.transform.localScale = Vector3.zero;
        }

        private void PlayParticle()
        {
            var particleManager = ManagerType.Particle.GetManager<ParticleManager>();
            if (particleManager != null)
            {
                particleManager.PlayParticle(ParticleType.SquareBlast, transform.position);
            }
            else
            {
                Debug.LogError("ParticleManager not found");
            }
        }
    }
}