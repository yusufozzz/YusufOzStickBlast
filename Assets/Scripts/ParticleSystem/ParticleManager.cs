using System.Collections.Generic;
using GameManagement;
using LevelSystem;
using UnityEngine;

namespace ParticleSystem
{
    public class ParticleManager: ManagerBase
    {
        [SerializeField]
        private List<Particle> particles;
        
        private readonly Dictionary<ParticleType, Queue<Particle>> _particlePool = new ();
        private LevelManager LevelManager => ManagerType.Level.GetManager<LevelManager>();
        
        public override void SetUp()
        {
            base.SetUp();
            foreach (var particle in particles)
            {
                if (!_particlePool.ContainsKey(particle.Type))
                {
                    _particlePool[particle.Type] = new Queue<Particle>();
                }
            }
        }
        
        public void PlayParticle(ParticleType type, Vector3 position)
        {
            if (!_particlePool.ContainsKey(type))
            {
                Debug.LogError($"No pool exists for particle type: {type}");
                return;
            }
            
            Particle particle;
            
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
            
            particle.transform.position = position;
            particle.gameObject.SetActive(true);
            particle.SetColor(LevelManager.ActiveShapeColor);
            particle.Init(() => ReturnToPool(particle));
        }
        
        private void ReturnToPool(Particle particle)
        {
            particle.gameObject.SetActive(false);
            
            if (!_particlePool.ContainsKey(particle.Type))
            {
                _particlePool[particle.Type] = new Queue<Particle>();
            }
            
            _particlePool[particle.Type].Enqueue(particle);
        }
    }
}