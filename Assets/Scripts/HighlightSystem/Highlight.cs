using DG.Tweening;
using GameManagement;
using UnityEngine;

namespace HighlightSystem
{
    public class Highlight : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer[] highLightEffects;

        private Sequence _sequence;

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void PlayAnimation()
        {
            StopAnimation();
            _sequence = DOTween.Sequence();
            foreach (var highLightEffect in highLightEffects)
            {
                highLightEffect.color = Color.white;
                var fadeTween = highLightEffect.DOFade(0.7f, 0.15f).SetLoops(999, LoopType.Yoyo);
                _sequence.Join(fadeTween);
            }
        }

        public void ReturnToPool()
        {
            StopAnimation();
            ManagerType.Highlight.GetManager<HighlightManager>().ReturnHighlight(this);
        }

        private void StopAnimation()
        {
            _sequence?.Kill();
        }
    }
}