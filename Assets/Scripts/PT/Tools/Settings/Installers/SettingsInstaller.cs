using PT.Tools.Settings.Configs;
using PT.Tools.Settings.Handlers;
using UnityEngine;
using Zenject;

namespace PT.Tools.Settings.Installers
{
    public class SettingsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SettingsManager>().FromComponentInHierarchy().AsSingle();
            
            Container.BindInterfacesAndSelfTo<SoundSettingHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<VibroSettingHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<LanguageSettingHandler>().AsSingle();
        }
    }
}