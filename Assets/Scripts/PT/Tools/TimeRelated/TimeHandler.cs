using PT.Logic.Dependency.Signals;
using UnityEngine;
using Zenject;

namespace PT.Tools.TimeRelated
{
    public class TimeHandler : MonoBehaviour
    {
        [Inject] private TimeManager _timeToggler;
        [Inject] private SignalBus _signalBus;
        
        private void Awake()
        {
            _signalBus.Subscribe<GameStartedSignal>(ToggleOn);
            // _signalBus.Subscribe<GameEndedSignal>(ToggleOff);

            _signalBus.Subscribe<GameMenuOpenedSignal>(ToggleOff);
            _signalBus.Subscribe<GameMenuClosedSignal>(ToggleOn);
            
            _signalBus.Subscribe<SceneUnloadedSignal>(ToggleOn);
        }

        private void ToggleOff()
        {
            _timeToggler.RequestTimeScale("Toggle", 0);
        }
        private void ToggleOn()
        {
            _timeToggler.RequestTimeScale("Toggle", 1);
        }
    }
}