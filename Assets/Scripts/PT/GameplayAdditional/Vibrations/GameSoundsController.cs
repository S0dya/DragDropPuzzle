using System;
using PT.Logic.ProjectContext;
using Zenject;

namespace PT.GameplayAdditional.Vibrations
{
    public class GameSoundsController : IInitializable, IDisposable
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private AudioManager _audioManager;

        public void Initialize()
        {
            // _signalBus.Subscribe<GridElementPushedSignal>(OnElementPushed);
        }
        public void Dispose()
        {
            // _signalBus.Unsubscribe<GridElementPushedSignal>(OnElementPushed);
        }

        // private void OnElementPushed(GridElementPushedSignal s)
        // {
        //     _audioManager.PlayOneShot(SoundEventEnum.ElementPushed, isRelease: true);
        // }
    }
}