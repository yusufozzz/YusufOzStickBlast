using System.Collections.Generic;
using GameManagement;
using UnityEngine;

namespace AudioSystem
{
    public class AudioManager : ManagerBase
    {
        [SerializeField] 
        private AudioSource audioSource;
        
        [SerializeField] 
        private SoundClip[] soundClips;
        
        private readonly Dictionary<SoundType, SoundClip> _soundMap = new ();

        public override void SetUp()
        {
            base.SetUp();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            
            InitializeSoundMap();
        }
        
        private void InitializeSoundMap()
        {
            foreach (SoundClip clip in soundClips)
            {
                _soundMap.TryAdd(clip.type, clip);
            }
        }
        
        public void PlaySound(SoundType type)
        {
            if (_soundMap.TryGetValue(type, out SoundClip soundClip))
            {
                audioSource.PlayOneShot(soundClip.clip);
            }
        }
        
        public void StopAllSounds()
        {
            audioSource.Stop();
        }
    }
}