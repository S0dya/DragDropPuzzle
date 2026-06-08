using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace PT.Tools.Windows
{
    [RequireComponent(typeof(WindowsFactory))]
    public class WindowsManager : MonoBehaviour
    {
        [SerializeField] private WindowsFactory windowsFactory;
        
        private readonly Stack<IWindow> _stack = new();
        private readonly List<IWindow> _openWindows = new();
        
        public async UniTask<IWindow> Open<T>(object data = null) where T : WindowBase
        {
            var window = windowsFactory.GetWindow<T>();

            if (window == null || window.IsOpen || _openWindows.Contains(window)) return window;

            _openWindows.Add(window);
            _stack.Push(window);
            await window.OpenAsync(data);

            return window;
        }

        public async UniTask Close<T>() where T : WindowBase
        {
            var window = windowsFactory.GetWindow<T>();

            if (window == null || !window.IsOpen || !_openWindows.Contains(window)) return;
            
            _openWindows.Remove(window);
            RemoveFromStack(window);
            
            await window.CloseAsync();
        }

        public async UniTask CloseTop()
        {
            if (_stack.TryPop(out var top))
            {
                await top.CloseAsync();
                _openWindows.Remove(top);
            }
        }
        
        public async UniTask CloseAllFrom<T>() where T : WindowBase
        {
            var targetType = typeof(T);
            
            while (_stack.Count > 0)
            {
                var top = _stack.Peek();
                await CloseTop();

                if (top.GetType() == targetType) break;
            }
        }

        public async UniTask CloseAll()
        {
            while (_stack.Count > 0)
                await CloseTop();
        }

        public IWindow GetLastOpened() => _stack.Count > 0 ? _stack.Peek() : null;
        
        private void RemoveFromStack(IWindow target)
        {
            var temp = new Stack<IWindow>();
            while (_stack.Count > 0)
            {
                var top = _stack.Pop();
                if (top != target)
                    temp.Push(top);
            }
            while (temp.Count > 0)
                _stack.Push(temp.Pop());
        }
    }
}