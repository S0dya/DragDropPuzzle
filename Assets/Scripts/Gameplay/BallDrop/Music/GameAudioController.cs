using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using Gameplay.BallDrop.Balls;
using Gameplay.BallDrop.Configs;
using Gameplay.Session;
using PT.Logic.Dependency.Signals;
using PT.Logic.ProjectContext;
using PT.Logic.Configs;
using PT.Tools.Addressables;
using PT.Tools.Debugging;
using PT.Tools.Helper;
using UnityEngine;
using Zenject;

namespace Gameplay.BallDrop.Music
{
    public class GameAudioController : MonoBehaviour
    {
        [SerializeField] private AudioSource activeVocalSource;
        [SerializeField] private AudioSource inactiveVocalSource;
        
        [Inject] private SignalBus _signalBus;
        [Inject] private GameSessionData _gameSessionData;
        [Inject] private AudioManager _audioManager;
        [Inject] private GameConfig _gameConfig;
        [Inject] private IAssetProvider _assetProvider;
        [Inject] private IAssetResolver _assetResolver;

        private EventInstance _musicInstance;
        private readonly Dictionary<CharacterNameEnum, VocalInfo> _vocalsInfos = new();
        private CharacterNameEnum? _currentActiveCharacter;

        private CancellationTokenSource _songEndingWatchCts;
        private bool _songEndingSignalSent;
        
        private CancellationTokenSource _vocalCrossfadeCts;
        
        private bool _isAppAudioPaused;
        private bool _hasFocus = true;
        private bool _isUnityPauseState;
        private bool _isMenuPaused;
        private CancellationTokenSource _loadVocalsCts;
        
        private void Awake()
        {
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
            _signalBus.Subscribe<NewLowestBallSignal>(OnNewLowestBall);
            _signalBus.Subscribe<BallReachedFinishSignal>(OnBallReachedFinish);
            _signalBus.Subscribe<GameMenuOpenedSignal>(OnGameMenuOpened);
            _signalBus.Subscribe<GameMenuClosedSignal>(OnGameMenuClosed);
        }

        public async UniTask Init()
        {
            CleanupAudioInstances(); 
            
            _loadVocalsCts?.Cancel();
            _loadVocalsCts?.Dispose();
            _loadVocalsCts = new CancellationTokenSource();
            await LoadVocalsForCurrentSong(_loadVocalsCts.Token);

            SetupVocalSources();
        }
        
        private void OnGameStarted()
        {
            _songEndingSignalSent = false;
            _songEndingWatchCts?.Cancel();
            _songEndingWatchCts?.Dispose();
            _songEndingWatchCts = new CancellationTokenSource();
            _currentActiveCharacter = null;
            
            var musicToPlay = _gameSessionData.SongData.MusicName;
            _musicInstance = _audioManager.CreateInstance(musicToPlay);
            _musicInstance.start();
            
            WatchForSongEnding(_songEndingWatchCts.Token).Forget();

            if (_isAppAudioPaused || _isMenuPaused)
            {
                SetPaused(true);
            }

            DebugManager.Log(DebugCategory.Audio, $"Started music {musicToPlay}");
        }
        
        private async UniTask LoadVocalsForCurrentSong(CancellationToken token)
        {
            var musicToPlay = _gameSessionData.SongData.MusicName;

            foreach (var characterData in _gameSessionData.CharacterDatas)
            {
                if (token.IsCancellationRequested) return;

                var character = characterData.CharacterName;

                try
                {
                    string assetKeyString = $"{character}_{musicToPlay}";
                    var vocalKey = (AssetKey)System.Enum.Parse(typeof(AssetKey), assetKeyString);

                    await _assetProvider.Load<AudioClip>(vocalKey);
                    if (token.IsCancellationRequested) return;

                    var clip = _assetResolver.Get<AudioClip>(vocalKey);

                    if (clip == null)
                    {
                        DebugManager.Log(DebugCategory.Audio, $"Loaded vocal clip is null for {musicToPlay} / {character}", LogType.Warning);
                        continue;
                    }

                    if (clip.loadState != AudioDataLoadState.Loaded)
                    {
                        clip.LoadAudioData();
                        await UniTask.WaitUntil(
                            () => clip.loadState == AudioDataLoadState.Loaded || clip.loadState == AudioDataLoadState.Failed,
                            cancellationToken: token);
                    }

                    if (clip.loadState == AudioDataLoadState.Failed)
                    {
                        DebugManager.Log(DebugCategory.Audio, $"Audio data failed to load for {musicToPlay} / {character}", LogType.Warning);
                        continue;
                    }

                    var vocalInfo = new VocalInfo(vocalKey, clip);
                    _vocalsInfos[character] = vocalInfo;

                    // await PrewarmVocalClip(vocalInfo, token);

                    DebugManager.Log(DebugCategory.Audio, $"Loaded + prewarmed vocal clip for {musicToPlay} / {character}");
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception e)
                {
                    DebugManager.Log(DebugCategory.Audio, $"Failed loading vocal for {musicToPlay} / {character}. {e.Message}", LogType.Warning);
                }
            }
        }
        
        private async UniTaskVoid WatchForSongEnding(CancellationToken token)
        {
            await UniTask.DelayFrame(1, cancellationToken: token);

            if (!_musicInstance.isValid()) return;

            _musicInstance.getDescription(out var eventDescription);
            eventDescription.getLength(out int eventLengthMs);

            if (eventLengthMs <= 0)
            {
                DebugManager.Log(DebugCategory.Audio, "GameAudioController: music event length is 0, song ending signal watcher skipped", LogType.Warning);
                return;
            }

            var songEndingBeforeFinishMs = Mathf.RoundToInt(_gameConfig.SongEndingBeforeFinishSeconds * 1000f);

            while (!token.IsCancellationRequested && _musicInstance.isValid() && !_songEndingSignalSent)
            {
                _musicInstance.getTimelinePosition(out int timelinePosMs);

                var remainingMs = eventLengthMs - timelinePosMs;
                if (remainingMs <= songEndingBeforeFinishMs)
                {
                    _songEndingSignalSent = true;
                    _signalBus.Fire<GameSongEndingSignal>();
                    
                    DebugManager.Log(DebugCategory.Audio, $"GameSongEndingSignal fired. Remaining ms: {remainingMs}");
                    return;
                }

                await UniTask.Delay(100, cancellationToken: token);
            }
        }

        private void OnNewLowestBall(NewLowestBallSignal signal)
        {
            if (!CanSwitchVocals()) return;
            
            var newCharacter = signal.Ball.Info.CharacterName;

            if (_currentActiveCharacter.HasValue && _currentActiveCharacter.Value == newCharacter) return;

            if (!_vocalsInfos.TryGetValue(newCharacter, out var newInfo) || newInfo.Clip == null)
            {
                DebugManager.Log(DebugCategory.Audio, $"No loaded vocal clip for {newCharacter}", LogType.Warning);
                return;
            }

            _musicInstance.getTimelinePosition(out int timelinePosMs);

            var timeSec = timelinePosMs / 1000f;
            var clip = newInfo.Clip;

            if (clip.length <= 0.05f) return;

            inactiveVocalSource.clip = clip;
            inactiveVocalSource.time = Mathf.Clamp(timeSec, 0f, clip.length - 0.05f);
            inactiveVocalSource.volume = 0f;
            inactiveVocalSource.Play();

            CancelVocalCrossfade();
            CrossfadeVocals(activeVocalSource, inactiveVocalSource, _gameConfig.VocalsRampSeconds).Forget();

            _currentActiveCharacter = newCharacter;

            DebugManager.Log(DebugCategory.Audio, $"Switched vocal to {newCharacter} at {timelinePosMs}ms");
        }
        private bool CanSwitchVocals()
        {
            if (!_musicInstance.isValid()) return false;
            if (_isAppAudioPaused || _isMenuPaused) return false;

            PLAYBACK_STATE playbackState;
            _musicInstance.getPlaybackState(out playbackState);

            if (playbackState != PLAYBACK_STATE.PLAYING) return false;

            return true;
        }

        private void OnBallReachedFinish(BallReachedFinishSignal signal)
        {

        }
        
        private void OnGameMenuOpened()
        {
            _isMenuPaused = true;
            RefreshCombinedPauseState();
        }

        private void OnGameMenuClosed()
        {
            _isMenuPaused = false;
            RefreshCombinedPauseState();
        }
        
        private void CancelVocalCrossfade()
        {
            _vocalCrossfadeCts?.Cancel();
            _vocalCrossfadeCts?.Dispose();
            _vocalCrossfadeCts = null;
        }

        private async UniTask CrossfadeVocals(AudioSource from, AudioSource to, float duration)
        {
            CancelVocalCrossfade();

            _vocalCrossfadeCts = new CancellationTokenSource();
            var token = _vocalCrossfadeCts.Token;

            var fromStart = from != null ? from.volume : 0f;
            var toStart = to != null ? to.volume : 0f;

            var elapsed = 0f;

            while (elapsed < duration && !token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);

                elapsed += Time.deltaTime;
                var t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);

                if (from != null) from.volume = Mathf.Lerp(fromStart, 0f, t);
                if (to != null) to.volume = Mathf.Lerp(toStart, 1f, t);
            }

            if (token.IsCancellationRequested) return;

            if (from != null)
            {
                from.volume = 0f;
                from.Stop();
                from.clip = null;
            }

            if (to != null)
            {
                to.volume = 1f;
            }

            var temp = activeVocalSource;
            activeVocalSource = inactiveVocalSource;
            inactiveVocalSource = temp;

            _vocalCrossfadeCts?.Dispose();
            _vocalCrossfadeCts = null;
        }
        
        private void SetPaused(bool isPaused)
        {
            try
            {
                if (_musicInstance.isValid())
                {
                    _musicInstance.setPaused(isPaused);
                }

                activeVocalSource.PauseOrUnPause(isPaused);
                inactiveVocalSource.PauseOrUnPause(isPaused);

                if (isPaused)
                {
                    CancelVocalCrossfade();
                }
            }
            catch (Exception e)
            {
                DebugManager.Log(DebugCategory.Gameplay, $"GameAudioController: failed to set pause state. {e.Message}", LogType.Warning);
            }
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<GameStartedSignal>(OnGameStarted);
            _signalBus.Unsubscribe<NewLowestBallSignal>(OnNewLowestBall);
            _signalBus.Unsubscribe<BallReachedFinishSignal>(OnBallReachedFinish);
            _signalBus.Unsubscribe<GameMenuOpenedSignal>(OnGameMenuOpened);
            _signalBus.Unsubscribe<GameMenuClosedSignal>(OnGameMenuClosed);

            _songEndingWatchCts?.Cancel();
            _songEndingWatchCts?.Dispose();

            _loadVocalsCts?.Cancel();
            _loadVocalsCts?.Dispose();
            
            _vocalsInfos.Clear();

            if (_musicInstance.isValid())
            {
                _musicInstance.stop(STOP_MODE.IMMEDIATE);
                _musicInstance.release();
            }
            
            CleanupAudioInstances();
        }
        
        private void CleanupAudioInstances()
        {
            CancelVocalCrossfade();

            if (activeVocalSource != null)
            {
                activeVocalSource.Stop();
                activeVocalSource.clip = null;
                activeVocalSource.volume = 0f;
            }

            if (inactiveVocalSource != null)
            {
                inactiveVocalSource.Stop();
                inactiveVocalSource.clip = null;
                inactiveVocalSource.volume = 0f;
            }

            foreach (var info in _vocalsInfos.Values)
            {
                _assetProvider.Release(info.AssetKey);
            }
            _vocalsInfos.Clear();

            if (_musicInstance.isValid())
            {
                _musicInstance.stop(STOP_MODE.IMMEDIATE);
                _musicInstance.release();
            }
        }
        
        private void OnApplicationPause(bool isPaused)
        {
            _isUnityPauseState = isPaused;
            RefreshAppPauseState();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            _hasFocus = hasFocus;
            RefreshAppPauseState();
        }
        
        private async UniTask PrewarmVocalClip(VocalInfo vocalInfo, CancellationToken token)
        {
            if (vocalInfo == null || vocalInfo.Clip == null || vocalInfo.IsPrewarmed) return;
            if (inactiveVocalSource == null) return;

            var clip = vocalInfo.Clip;

            if (clip.length <= 0f) 
            {
                vocalInfo.SetPrewarmed();
                return;
            }

            var prevClip = inactiveVocalSource.clip;
            var prevTime = inactiveVocalSource.time;
            var prevVolume = inactiveVocalSource.volume;
            var prevLoop = inactiveVocalSource.loop;
            var wasPlaying = inactiveVocalSource.isPlaying;

            inactiveVocalSource.Stop();
            inactiveVocalSource.clip = clip;
            inactiveVocalSource.volume = 0f;
            inactiveVocalSource.loop = false;
            inactiveVocalSource.time = 0f;
            inactiveVocalSource.Play();

            await UniTask.Yield(PlayerLoopTiming.Update, token);

            inactiveVocalSource.Pause();
            inactiveVocalSource.Stop();
            inactiveVocalSource.clip = prevClip;
            inactiveVocalSource.volume = prevVolume;
            inactiveVocalSource.loop = prevLoop;

            if (prevClip != null)
            {
                inactiveVocalSource.time = Mathf.Clamp(prevTime, 0f, Mathf.Max(0f, prevClip.length - 0.01f));
            }

            if (wasPlaying && prevClip != null)
            {
                inactiveVocalSource.UnPause();
            }

            vocalInfo.SetPrewarmed();
        }
        
        private void SetupVocalSources()
        {
            if (activeVocalSource != null)
            {
                activeVocalSource.playOnAwake = false;
                activeVocalSource.loop = false;
                activeVocalSource.spatialBlend = 0f;
                activeVocalSource.volume = 0f;
                activeVocalSource.Stop();
            }

            if (inactiveVocalSource != null)
            {
                inactiveVocalSource.playOnAwake = false;
                inactiveVocalSource.loop = false;
                inactiveVocalSource.spatialBlend = 0f;
                inactiveVocalSource.volume = 0f;
                inactiveVocalSource.Stop();
            }
        }
        
        private void RefreshAppPauseState()
        {
            _isAppAudioPaused = _isUnityPauseState || !_hasFocus;
            RefreshCombinedPauseState();
        }

        private void RefreshCombinedPauseState()
        {
            var shouldPause = _isAppAudioPaused || _isMenuPaused;
            SetPaused(shouldPause);

            DebugManager.Log(DebugCategory.Audio, $"GameAudioController combined pause state: {shouldPause}");
        }
    }
    
    // NEW CODE
    class VocalInfo
    {
        public AssetKey AssetKey { get; private set; }
        public AudioClip Clip { get; private set; }
        public bool IsPrewarmed { get; private set; }

        public VocalInfo(AssetKey assetKey, AudioClip clip)
        {
            AssetKey = assetKey;
            Clip = clip;
        }

        public void SetPrewarmed()
        {
            IsPrewarmed = true;
        }
    }

    // NEW CODE
    static class AudioSourceExtensions
    {
        public static void PauseOrUnPause(this AudioSource source, bool isPaused)
        {
            if (source == null) return;

            if (isPaused)
            {
                if (source.isPlaying) source.Pause();
            }
            else
            {
                if (source.clip != null) source.UnPause();
            }
        }
    }
}