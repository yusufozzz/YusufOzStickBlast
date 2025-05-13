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
}