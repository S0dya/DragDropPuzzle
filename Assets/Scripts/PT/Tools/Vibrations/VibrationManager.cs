using MoreMountains.NiceVibrations;
using PT.Logic.Save;
using PT.Tools.Debugging;
using UnityEngine;
using Zenject;

namespace PT.Tools.Vibrations
{
    public class VibrationManager : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        
        public void Vibrate(HapticTypes type)
        {
            if (!GameData.VibroOn)
            {
                DebugManager.Log(DebugCategory.Vibration, $"Skipped vibration ({type}) - Vibro is OFF", LogType.Warning);
                return;
            }

            DebugManager.Log(DebugCategory.Vibration, $"Vibrating with type: {type}");
            MMVibrationManager.Haptic(type);
        }

        public void Vibrate(float intensity, float duration, float sharpness)
        {
            if (!GameData.VibroOn)
            {
                DebugManager.Log(DebugCategory.Vibration, $"Skipped continuous vibration - Vibro is OFF", LogType.Warning);
                return;
            }

            DebugManager.Log(DebugCategory.Vibration, $"Continuous vibration: Intensity={intensity}, Duration={duration}, Sharpness={sharpness}");
            MMVibrationManager.ContinuousHaptic(intensity, duration, sharpness);
        }

        public void StopVibration()
        {
            MMVibrationManager.StopAllHaptics();
            DebugManager.Log(DebugCategory.Vibration, "Stopped all vibrations");
        }
    }
}