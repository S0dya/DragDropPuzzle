using Cysharp.Threading.Tasks;
using Gameplay.BallDrop.Balls;
using Gameplay.BallDrop.Levels.Generation;
using Gameplay.BallDrop.Music;
using Gameplay.BallDrop.UI;
using PT.Logic.Dependency.Signals;
using PT.Tools.Windows;
using UnityEngine;
using Zenject;

namespace PT.Logic.Dependency.GameScene
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Inject] private DiContainer _container;
        [Inject (Id = "Game")] private WindowsManager _windowsManager;

        private async void Start()
        {
            // await _container.Resolve<BoardManager>().Init();
            
            // _container.Resolve<SignalBus>().Fire(new GameStartedSignal());
            
            _container.Resolve<LevelGenerator>().Init();
            _container.Resolve<BallsController>().Init();
            await _container.Resolve<GameAudioController>().Init();

            await _windowsManager.Open<GameIntroWindow>();
        }
    }
}
