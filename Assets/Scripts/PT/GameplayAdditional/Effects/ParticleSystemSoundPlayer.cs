using PT.Logic.Configs;
using PT.Logic.ProjectContext;
using UnityEngine;
using Zenject;

namespace PT.GameplayAdditional.Effects
{
    [RequireComponent(typeof(ParticleSystemWatcher))]
    public class ParticleSystemSoundPlayer : MonoBehaviour
    {
        [SerializeField] private ParticleSystemWatcher particleSystemWatcher;
        [SerializeField] private SoundEventEnum soundEnum;
        
        [Inject] private AudioManager _audioManager;

        private void Awake()
        {
            if (particleSystemWatcher == null) particleSystemWatcher = GetComponent<ParticleSystemWatcher>(); 
        }

        private void OnEnable()
        {
            particleSystemWatcher.OnStarted += PlaySound;
        }
        private void OnDisable()
        {
            particleSystemWatcher.OnStarted -= PlaySound;
        }
        
        private void PlaySound()
        {
            _audioManager.PlayOneShot(soundEnum);
        }
    }
}