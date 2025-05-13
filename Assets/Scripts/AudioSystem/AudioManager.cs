using GameManagement;
using UnityEngine;

namespace AudioSystem
{
    public class AudioManager : ManagerBase
    {
        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private AudioClip[] audioClips;

    }
}