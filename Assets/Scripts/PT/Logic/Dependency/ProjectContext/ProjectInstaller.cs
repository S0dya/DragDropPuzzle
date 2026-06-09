using Gameplay.DragDropPuzzle;
using Gameplay.DragDropPuzzle.Data;
using PT.GameplayAdditional.Progression;
using PT.Logic.Dependency.Signals;
using Zenject;

namespace PT.Logic.Dependency.ProjectContext
{
    public class ProjectInstaller : BaseProjectInstaller
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            
            Container.Bind<StageProvider>().AsSingle();
            
            Container.DeclareSignal<GameAddCoinsSignal>();
            Container.DeclareSignal<GameSpendCoinsSignal>();
            
            Container.Bind<GameSessionData>().AsSingle();
        }
    }
}
