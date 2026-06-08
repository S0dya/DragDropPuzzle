#if UNITY_WEBGL
//using CrazyGames;
using System;
using PT.Logic.Configs;
using PT.Logic.Dependency.Signals;
using UnityEngine;
using YG;
using Zenject;

namespace PT.Logic.PlatformRelated.Web
{
    public class AdditionalWebSDKLogic : IInitializable, IDisposable
    {
        [Inject] private GameConfig _gameConfig;
        [Inject] private SignalBus _signalBus;
        
        public void Initialize()
        {
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
            _signalBus.Subscribe<GameEndedSignal>(OnGameFinished);
            _signalBus.Subscribe<GameMenuOpenedSignal>(OnGameFinished);
            _signalBus.Subscribe<GameMenuClosedSignal>(OnGameStarted);
        }
        public void Dispose()
        {
            _signalBus.Unsubscribe<GameStartedSignal>(OnGameStarted);
            _signalBus.Unsubscribe<GameEndedSignal>(OnGameFinished);
            _signalBus.Unsubscribe<GameMenuOpenedSignal>(OnGameFinished);
            _signalBus.Unsubscribe<GameMenuClosedSignal>(OnGameStarted);
        }

        private void OnGameStarted()
        {
            if (_gameConfig.PlatformType == PlatformTypeEnum.Yandex) YG2.GameplayStart();
            //else if (_gameConfig.PlatformType == _gameConfig.PlatformTypeEnum.CrazyGames)
                //CrazySDK.Game.GameplayStart();
        }
        private void OnGameFinished()
        {
            if (_gameConfig.PlatformType == PlatformTypeEnum.Yandex) YG2.GameplayStop();
            //else if (_gameConfig.PlatformType == _gameConfig.PlatformTypeEnum.CrazyGames)
                //CrazySDK.Game.GameplayStop();
        }
    }
}
#endif