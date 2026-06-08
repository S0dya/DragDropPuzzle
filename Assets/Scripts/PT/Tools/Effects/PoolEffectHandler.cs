using System;
using UnityEngine;

namespace PT.Tools.Effects
{
    public class PoolEffectHandler : MonoBehaviour
    {
        [SerializeField] private ParticleSystem effectParticleSystem;

        private Action<ParticleSystem> _returnAction;

        public void Init(Action<ParticleSystem> returnAction)
        {
            _returnAction = returnAction;
        }

        public void OnParticleSystemStopped()
        {
            _returnAction.Invoke(effectParticleSystem);
        }
    }
}
