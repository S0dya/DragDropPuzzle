using System;
using PT.Logic.Dependency.Signals;
using Zenject;

namespace PT.Tools.Vibrations
{
    public class VibrationSignalsController : IInitializable, IDisposable
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private VibrationManager _vibrationManager;

        public void Initialize()
        {
            _signalBus.Subscribe<AdCountdownStartSignal>(OnStop);
            _signalBus.Subscribe<AdOpenedSignal>(OnStop);
            _signalBus.Subscribe<AdClosedSignal>(OnStop);
            
            // _signalBus.Subscribe<GameOverSignal>(OnGameOver);
        }
        public void Dispose()
        {
            _signalBus.Unsubscribe<AdCountdownStartSignal>(OnStop);
            _signalBus.Unsubscribe<AdOpenedSignal>(OnStop);
            _signalBus.Unsubscribe<AdClosedSignal>(OnStop);
        }
        
        void OnStop() => _vibrationManager.StopVibration();
    }
}