#if UNITY_IOS || UNITY_ANDROID
using Cysharp.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using PT.Backend.Interfaces;
using PT.Tools.Debugging;
using UnityEngine;
#endif

namespace PT.Backend.GP
{
#if UNITY_IOS || UNITY_ANDROID
    
    public class GooglePlayAuthentificationService : IAuthentificationService
    {
        public bool IsSignedIn { get; private set; }
        public string PlayerId { get; private set; }
        public string DisplayName { get; private set; }

        public UniTask SignIn()
        {
            if (IsSignedIn) return UniTask.CompletedTask;

            if (PlayGamesPlatform.Instance == null)
            {
                DebugManager.Log(DebugCategory.Backend, "Google Play platform not initialized", LogType.Error);
                return UniTask.FromException(new System.Exception("Google Play platform not initialized"));
            }

            var tcs = new UniTaskCompletionSource();

            PlayGamesPlatform.Instance.Authenticate(status =>
            {
                if (status == SignInStatus.Success)
                {
                    IsSignedIn = true;
                    PlayerId = PlayGamesPlatform.Instance.localUser.id;
                    DisplayName = PlayGamesPlatform.Instance.localUser.userName;

                    DebugManager.Log(DebugCategory.Backend, $"Google Play signed in: {DisplayName} ({PlayerId})");

                    tcs.TrySetResult();
                }
                else
                {
                    DebugManager.Log(DebugCategory.Backend, $"Google Play sign-in failed: {status}", LogType.Error);

                    tcs.TrySetException(new System.Exception($"Google Play sign-in failed: {status}"));
                }
            });

            return tcs.Task;
        }

        public UniTask SetDisplayName(string name)
        {
            throw new System.NotImplementedException();
        }
    }
#endif
}
