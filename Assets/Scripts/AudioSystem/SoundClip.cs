using GameManagement;
using UnityEngine;

namespace AudioSystem
{
    [System.Serializable]
    public class SoundClip
    {
        public SoundType type;
        public AudioClip clip;
    }
    
    public enum SoundType
    {
        ShapePlaced,
        ShapePlaceFailed,
        GridMatch,
        GameLose,
    }
    
    public static class SoundTypeExtensions
    {
        public static void PlaySfx(this SoundType soundType)
        {
            AudioManager audioManager = ManagerType.Audio.GetManager<AudioManager>();
            audioManager.PlaySound(soundType);
        }
    }
}