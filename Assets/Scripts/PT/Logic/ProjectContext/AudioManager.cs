using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using PT.Logic.Configs;
using PT.Logic.Save;
using PT.Tools.Debugging;
using UnityEngine;
using Zenject;

namespace PT.Logic.ProjectContext
{
    public class AudioManager : MonoBehaviour
    {
        [Inject] private AudioConfig _audioConfig;

        private readonly Dictionary<SoundEventEnum, EventInstance> _enumInstancesDict = new();
        
        private readonly Dictionary<SoundEventEnum, EventReference> _enumReferencesDict = new();

         private void Awake()
        {
            LoadGameTask().Forget();
        }

        private async UniTask LoadGameTask()
        {
            await UniTask.WaitUntil(() => RuntimeManager.HaveAllBanksLoaded);
            await UniTask.WaitUntil(() => !RuntimeManager.AnySampleDataLoading());
            DebugManager.Log(DebugCategory.Audio, "All FMOD banks loaded successfully.");

            InitAudio();
        }

        private void InitAudio()
        {
            foreach (var kvSound in _audioConfig.KvSounds.Dictionary)
            {
                _enumReferencesDict.Add(kvSound.Key, kvSound.Value);
                _enumInstancesDict.Add(kvSound.Key, CreateInstance(kvSound.Value));
            }
        }

        public void Init()
        {
            ToggleSound(GameData.SoundOn);
        }
        
        public void PlayOneShot(SoundEventEnum soundEventEnum, Vector3? position = null, bool isRelease = false)
        {
            if (!CanPlaySound(soundEventEnum))
            {
                DebugManager.Log(DebugCategory.Audio, $"[Skip] Tried playing {soundEventEnum} but sound is off or missing.");
                return;
            }

            EventInstance instance = isRelease
                ? CreateInstance(_enumReferencesDict[soundEventEnum])
                : _enumInstancesDict[soundEventEnum];

            if (position.HasValue)
                instance.set3DAttributes(RuntimeUtils.To3DAttributes(position.Value));

            instance.start();
            if (isRelease) instance.release();

            DebugManager.Log(DebugCategory.Audio, $"Playing {(isRelease ? "release" : "looped")} sound: {soundEventEnum} {(position.HasValue ? "(3D)" : "(2D)")}");
        }
        
        public EventReference GetEventReference(SoundEventEnum sound) => _enumReferencesDict[sound];
        public EventInstance CreateInstance(SoundEventEnum sound) => RuntimeManager.CreateInstance(GetEventReference(sound));
        
        private EventInstance CreateInstance(EventReference sound) => RuntimeManager.CreateInstance(sound);

        private bool CanPlaySound(SoundEventEnum soundEventEnum) => GameData.SoundOn && _enumInstancesDict.ContainsKey(soundEventEnum);
        
        public void ToggleSound(bool toggle)
        {
            DebugManager.Log(DebugCategory.Audio, $"Sound toggled: {(toggle ? "ON" : "OFF")}");
            RuntimeManager.GetBus("bus:/").setVolume(toggle ? 1 : 0);
        }
        
        private void OnApplicationPause(bool value)
        {
            if (GameData.SoundOn)
            {
                ToggleResumeSound(!value);
            }
        }

        private void OnApplicationFocus(bool value)
        {
            if (GameData.SoundOn)
            {
                ToggleResumeSound(value);
            }
        }

        internal void ToggleResumeSound(bool toggle)
        {
            RuntimeManager.PauseAllEvents(!toggle);
            if (toggle) RuntimeManager.CoreSystem.mixerResume();
            else RuntimeManager.CoreSystem.mixerSuspend();

            RuntimeManager.MuteAllEvents(!toggle);

            DebugManager.Log(DebugCategory.Audio, $"Mixer {(toggle ? "Resumed / Unmuted" : "Suspended / Muted")}");
        }
    }
}
