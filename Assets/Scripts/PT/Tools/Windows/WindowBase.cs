using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PT.Tools.Windows
{
    public interface IWindow
    {
        bool IsOpen { get; }

        UniTask OpenAsync(object data = null);
        UniTask CloseAsync();
    }
    
    public abstract class WindowBase : MonoBehaviour, IWindow
    {
        public bool IsOpen { get; private set; }

        protected object Payload;

        public async UniTask OpenAsync(object payload = null)
        {
            gameObject.SetActive(true);
            IsOpen = true;
            Payload = payload;
            
            await OnOpen();
        }

        public async UniTask CloseAsync()
        {
            await OnClose();
            
            IsOpen = false;
            gameObject.SetActive(false);
        }
        
        protected virtual void OnInit() {}
        protected virtual async UniTask OnOpen() => await UniTask.CompletedTask;
        protected virtual async UniTask OnClose() => await UniTask.CompletedTask;
    }
}