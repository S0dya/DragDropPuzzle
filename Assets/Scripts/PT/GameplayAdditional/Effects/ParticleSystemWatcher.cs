using System;
using UnityEngine;

namespace PT.GameplayAdditional.Effects
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemWatcher : MonoBehaviour
    {
        [SerializeField] private ParticleSystem ps;
        
        public event Action OnStarted;
        public event Action OnStopped;
        
        private bool _wasPlaying;

        private void Awake()
        {
            if (ps == null) ps = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            bool playing = ps.isPlaying;

            if (!_wasPlaying && playing) OnStarted?.Invoke();
            else if (_wasPlaying && !playing) OnStopped?.Invoke();

            _wasPlaying = playing;
        }
    }
}