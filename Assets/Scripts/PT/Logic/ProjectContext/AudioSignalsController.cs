using PT.Logic.Dependency.Signals;
using Zenject;

namespace PT.Logic.ProjectContext
{
    public class AudioSignalsController : IInitializable
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private AudioManager _audioManager;

        public void Initialize()
        {
            _signalBus.Subscribe<AdCountdownStartSignal>(OnAdCountdown);
            _signalBus.Subscribe<AdOpenedSignal>(OnAdOpened);
            _signalBus.Subscribe<AdClosedSignal>(OnAdClosed);
        }

        private void OnAdCountdown() => _audioManager.ToggleResumeSound(false);
        private void OnAdOpened() => _audioManager.ToggleResumeSound(false);
        private void OnAdClosed() => _audioManager.ToggleResumeSound(true);
    }
}