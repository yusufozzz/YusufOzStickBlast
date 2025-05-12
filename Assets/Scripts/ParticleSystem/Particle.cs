using System;
using UnityEngine;

namespace ParticleSystem
{
    public class Particle : MonoBehaviour
    {
        [field: SerializeField]
        public ParticleType Type { get; private set; }

        [SerializeField]
        private UnityEngine.ParticleSystem particleSystem;
        
        private Action _onDisable;

        public void Init(Action onDisable)
        {
            particleSystem.Play();
            if (_onDisable == null)
                _onDisable = onDisable;
        }

        private void OnDisable()
        {
            _onDisable?.Invoke();
        }
    }
}