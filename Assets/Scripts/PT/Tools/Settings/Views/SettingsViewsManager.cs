using System.Collections.Generic;
using PT.Tools.Settings.Configs;
using UnityEngine;
using Zenject;

namespace PT.Tools.Settings.Views
{
    public class SettingsViewsManager : MonoBehaviour
    {
        [SerializeField] private List<SettingView> views;

        [Inject] private SettingsInfosConfig _settingsInfosConfig;

        private void Start()
        {
            foreach (var view in views) if (view.gameObject.activeSelf) view.Init();
        }
    }
}