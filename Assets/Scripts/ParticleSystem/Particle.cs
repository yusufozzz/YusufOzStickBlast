using System;
using UnityEngine;

namespace ParticleSystem
{
    public class Particle : MonoBehaviour
    {
        [field: SerializeField]
        public ParticleType Type { get; private set; }

        [SerializeField]
        private UnityEngine.ParticleSystem system;
        
        private Action _onDisable;

        public void Init(Action onDisable)
        {
            system.Play();
            if (_onDisable == null)
                _onDisable = onDisable;
        }

        private void OnDisable()
        {
            _onDisable?.Invoke();
        }
        
        public void SetColor(Color color)
        {
            var main = system.main;
            main.startColor = color;
        }
    }
}