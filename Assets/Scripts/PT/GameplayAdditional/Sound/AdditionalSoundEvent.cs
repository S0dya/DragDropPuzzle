using UnityEngine;
using Zenject;
using PT.Logic.ProjectContext;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using NaughtyAttributes;
using PT.Logic.Configs;
using PT.Tools.Helper;

namespace PT.GameplayAdditional.Sound
{
    public class AdditionalSoundEvent : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField][MinMaxSlider(1, 2000)] private Vector2 delay;
        [Space(10)]
        [SerializeField] protected SoundEventEnum[] soundEventEnumsToPlay;

        [Inject] protected AudioManager _audioManager;

        private CancellationTokenSource _cts;

        private void Start()
        {
            PlayRandomSounds();
        }

        private void OnDisable()
        {
            StopRandomSounds();
        }

        private async void PlayRandomSounds()
        {
            StopRandomSounds();

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                while (true)
                {
                    await UniTask.WaitForSeconds(Utils.GetRandomValue(delay), cancellationToken: token);
                    if (token.IsCancellationRequested) break;

                    PlaySound();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void StopRandomSounds()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }


        protected virtual void PlaySound()
        {
            _audioManager.PlayOneShot(Utils.GetRandomElement(soundEventEnumsToPlay));
        }
    }
}