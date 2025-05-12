using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameManagement;
using GridSystem.Lines;
using ParticleSystem;
using UnityEngine;

namespace GridSystem
{
    public class Square : MonoBehaviour
    {
        private readonly List<Line> _lines = new();

        [SerializeField]
        private SpriteRenderer spriteRenderer;
        
        private bool _isCompleted;
        private Tween _scaleTween;

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
            if (IsComplete() && !_isCompleted)
            {
                _isCompleted = true;
                Complete();
            }
        }

        private void Complete()
        {
            foreach (var line in _lines)
            {
                line.SetAsMemberOfCompletedSquare();
            }

            Animate();
        }

        private void Animate()
        {
            _scaleTween?.Kill();
            _scaleTween = spriteRenderer.transform.DOScale(Vector3.one, GeneralSettings.Instance.SquareAnimationDuration);
        }

        public void Clear()
        {
            if (!_isCompleted) return;
            _isCompleted = false;
            _scaleTween?.Kill();
            foreach (var line in _lines)
            {
                line.Clear();
            }
            PlayParticle();
            spriteRenderer.transform.localScale = Vector3.zero;
        }

        private void PlayParticle()
        {
            ManagerType.Particle.GetManager<ParticleManager>().PlayParticle(ParticleType.SquareBlast, transform.position);
        }
    }
}