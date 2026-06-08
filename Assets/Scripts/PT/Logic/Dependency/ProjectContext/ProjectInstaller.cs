using Gameplay.Session;
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
            
            Container.DeclareSignal<NewLowestBallSignal>();
            Container.DeclareSignal<BallReachedFinishSignal>();
            Container.DeclareSignal<GameIntroTimerPassedSignal>();
            Container.DeclareSignal<GameSongEndingSignal>();
            
            Container.Bind<GameSessionData>().AsSingle();
        }
    }
}
