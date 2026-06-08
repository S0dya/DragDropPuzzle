using PT.Logic.Configs;
using PT.Logic.ProjectContext;
using UnityEngine;
using Zenject;

namespace PT.GameplayAdditional.Sound
{
    public class SimpleSoundEventPlayer : MonoBehaviour
    {
        [SerializeField] private SoundEventEnum soundEvent;

        [Header("Additional")]
        [SerializeField] private Transform transformSoundPlayer;

        [Inject] private AudioManager _audioManager;

        public void PlaySound()
        {
            if (transformSoundPlayer != null)
                _audioManager.PlayOneShot(soundEvent, transformSoundPlayer.position);
            else
                _audioManager.PlayOneShot(soundEvent);
        }
    }
}
