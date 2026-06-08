using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using PT.Backend.Types;
using PT.Logic.PersistentScene;
using PT.Tools.Debugging;
using PT.Tools.Windows;
using PT.UI.Windows.Payloads;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.Tools.Leaderboard.Game
{
    public class LeaderboardDynamicView : WindowBase
    {
        [Header("Refs")]
        [SerializeField] private LeaderboardViewInitializer initializer;
        [SerializeField] private RectTransform content;
        [SerializeField] private RectTransform playerAnchor;
        [SerializeField] private LeaderboardRankAnimation animation;
        [SerializeField] private Button nextButton;
        [SerializeField] private RectTransform blankObjectTransform;

        [Inject] private LeaderboardManager _leaderboardManager;
        [Inject] private LoadingManager _loadingManager;

        private CancellationTokenSource _cts;
        private bool _animationFinished;

        private void Awake()
        {
            nextButton.onClick.AddListener(OnNext);
        }

        protected override async UniTask OnOpen()
        {
            var result = await _leaderboardManager.TryGetAroundPlayerEntries();

            if (!result.success)
            {
                DebugManager.Log(DebugCategory.UI, $"Leaderboard entries fetch failed, performing next action", LogType.Warning);
                PerformNextAction();
                return;
            }

            await PlayAnimation(result.snapshot.Entries, result.snapshot.PlayerEntry);
        }

        private async UniTask PlayAnimation(IReadOnlyList<LeaderboardEntry> entries, LeaderboardEntry playerEntry)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            _animationFinished = false;

            initializer.SpawnEntries(entries);

            blankObjectTransform.SetParent(content, false);

            var playerView = initializer.SpawnPlayer(playerEntry, false);
            playerView.transform.SetParent(playerAnchor, false);

            LayoutRebuilder.ForceRebuildLayoutImmediate(content);

            int targetIndex = entries
                .Select((e, i) => (e, i))
                .First(x => x.e.Rank == playerEntry.Rank + 1).i;

            blankObjectTransform.SetSiblingIndex(targetIndex);

            LayoutRebuilder.ForceRebuildLayoutImmediate(content);

            SnapContentToBottom();

            await animation.PlayScroll(content, blankObjectTransform, playerAnchor, _cts.Token);

            _animationFinished = true;
        }

        private void OnNext()
        {
            if (!_animationFinished)
            {
                SkipAnimation();
                return;
            }

            PerformNextAction();
        }

        private void PerformNextAction()
        {
            if (Payload is LeaderboardDynamicPayload payload)
            {
                payload.OnNext.Invoke();
                return;
            }

            DebugManager.Log(DebugCategory.UI, "Leaderboard payload missing", LogType.Error);

            _loadingManager.LoadSteps(new()
            {
                _loadingManager.GetSceneUnloadingStep(SceneNameEnum.Game),
                _loadingManager.GetSceneLoadingStep(SceneNameEnum.Menu)
            }).Forget();
        }

        private void SnapContentToBottom()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);

            float contentHeight = content.rect.height;
            float viewportHeight = ((RectTransform)content.parent).rect.height;

            float bottomY = Mathf.Max(0, contentHeight - viewportHeight);

            content.anchoredPosition = new Vector2(
                content.anchoredPosition.x,
                bottomY
            );
        }
        
        private void SkipAnimation()
        {
            _cts?.Cancel();
            animation.Skip();
            _animationFinished = true;
        }
    }
}