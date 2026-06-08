using Zenject;

namespace PT.Tools.Debugging
{
    public class SignalsLogger : IInitializable
    {
        readonly SignalBus _bus;

        public SignalsLogger(SignalBus bus)
        {
            _bus = bus;
        }

        public void Initialize()
        {
            // _bus.Subscribe<object>(OnAnySignal); // doesnt work
        }

        private void OnAnySignal(object signal)
        {
            DebugManager.Log(DebugCategory.Observer, $"{signal.GetType().Name}");
        }
    }
}