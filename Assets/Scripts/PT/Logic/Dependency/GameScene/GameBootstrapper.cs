using Cysharp.Threading.Tasks;
using Gameplay.DragDropPuzzle;
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
            // _container.Resolve<PuzzleGameManager>();
        }
    }
}
