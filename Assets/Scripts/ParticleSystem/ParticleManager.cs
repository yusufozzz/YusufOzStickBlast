using System.Collections.Generic;
using GameManagement;
using UnityEngine;

namespace ParticleSystem
{
    public class ParticleManager: ManagerBase
    {
        [SerializeField]
        private List<Particle> particles;
        
        private readonly Dictionary<ParticleType, Queue<Particle>> _particlePool = new Dictionary<ParticleType, Queue<Particle>>();
        
        public override void SetUp()
        {
            base.SetUp();
            foreach (var particle in particles)
            {
                if (!_particlePool.ContainsKey(particle.Type))
                {
                    _particlePool[particle.Type] = new Queue<Particle>();
                }
                _particlePool[particle.Type].Enqueue(particle);
            }
        }
        
        public void PlayParticle(ParticleType type, Vector3 position)
        {
            // Check if the pool exists for this type
            if (!_particlePool.ContainsKey(type))
            {
                Debug.LogError($"No pool exists for particle type: {type}");
                return;
            }
            
            Particle particle;
            
            // Get existing or create new particle
            if (_particlePool[type].Count > 0)
            {
                particle = _particlePool[type].Dequeue();
            }
            else
            {
                var prefab = particles.Find(p => p.Type == type);
                if (prefab == null)
                {
                    Debug.LogError($"No prefab found for particle type: {type}");
                    return;
                }
                
                particle = Instantiate(prefab);
            }
            
            // Position, activate and initialize
            particle.transform.position = position;
            particle.gameObject.SetActive(true);
            particle.Init(() => ReturnToPool(particle));
        }
        
        private void ReturnToPool(Particle particle)
        {
            particle.gameObject.SetActive(false);
            
            // Ensure the pool exists
            if (!_particlePool.ContainsKey(particle.Type))
            {
                _particlePool[particle.Type] = new Queue<Particle>();
            }
            
            _particlePool[particle.Type].Enqueue(particle);
        }
    }
}