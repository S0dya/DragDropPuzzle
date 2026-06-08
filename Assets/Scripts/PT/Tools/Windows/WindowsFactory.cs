using System;
using System.Collections.Generic;
using NaughtyAttributes;
using PT.Logic.Dependency;
using PT.Tools.Debugging;
using PT.Tools.Factories;
using PT.Tools.Helper;
using UnityEngine;
using Zenject;

namespace PT.Tools.Windows
{
    [RequireComponent(typeof(WindowsManager))]
    public class WindowsFactory : MonoBehaviour
    {
        [SerializeField] private WindowBase[] windows;
        [SerializeField] private Transform parent;

        [Inject] private IFactoryZenject _factoryZenject;

        private readonly Dictionary<Type, IWindow> _windows = new();

        private void Awake()
        {
            FindWindows();
            
            foreach (var window in windows)
            {
                window.SetActive(false);
                
                _windows[window.GetType()] = window;
            }
        }

        [Button]
        private void FindWindows()
        {
            if (windows == null || windows.Length == 0) windows = GetComponentsInChildren<WindowBase>(true);
        }
        
        public IWindow GetWindow<T>() where T : WindowBase
        {
            if (!_windows.TryGetValue(typeof(T), out var window))
            {
                DebugManager.Log(DebugCategory.UI, $"Window {typeof(T)} not found in cache", LogType.Error);
                return null;
            }
            
            ((MonoBehaviour)window).transform.SetAsLastSibling();

            return window;
        }
    }
}