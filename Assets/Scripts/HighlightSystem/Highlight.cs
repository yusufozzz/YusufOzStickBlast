using DG.Tweening;
using UnityEngine;

namespace HighlightSystem
{
    public class Highlight : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer[] highLightEffects;

        private Sequence _sequence;

        public void SetPositionAndRotation(Vector3 position, bool isVertical)
        {
            transform.SetPositionAndRotation(position, isVertical ? Quaternion.identity : Quaternion.Euler(0, 0, 90));
        }

        public void PlayAnimation()
        {
            StopAnimation();
            gameObject.SetActive(true);
            _sequence = DOTween.Sequence();
            foreach (var highLightEffect in highLightEffects)
            {
                highLightEffect.color = Color.green;
                var fadeTween = highLightEffect.DOFade(0.7f, 0.15f).SetLoops(999, LoopType.Yoyo);
                _sequence.Join(fadeTween);
            }
        }

        public void StopAnimation()
        {
            _sequence?.Kill();
            gameObject.SetActive(false);
        }
    }
}