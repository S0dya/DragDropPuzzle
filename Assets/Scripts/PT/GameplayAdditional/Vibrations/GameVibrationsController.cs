using System;
using MoreMountains.NiceVibrations;
using PT.Tools.Vibrations;
using Zenject;

namespace PT.GameplayAdditional.Vibrations
{
    public class GameVibrationsController : IInitializable, IDisposable
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private VibrationManager _vibration;

        public void Initialize()
        {
            // _signalBus.Subscribe<BoardMergeSignal>(OnMerge);
        }
        public void Dispose()
        {
            // _signalBus.Unsubscribe<BoardMergeSignal>(OnMerge);
        }
        
        // private void OnMerge(BoardMergeSignal s)
        // {
        //     _vibration.Vibrate(HapticTypes.LightImpact);
        // }
    }
}