using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PT.Tools.Helper;
using UnityEngine;

namespace PT.Tools.Leaderboard.Game
{
    public class LeaderboardRankAnimation : MonoBehaviour
    {
        [SerializeField] private float moveTime = 0.7f;
        [SerializeField] private Ease ease = Ease.InOutCubic;

        private Tween _tween;

        public async UniTask PlayScroll(
            RectTransform content,
            RectTransform targetEntry,
            RectTransform playerAnchor,
            CancellationToken token)
        {
            Skip();

            Vector3 entryWorld = targetEntry.position;
            Vector3 anchorWorld = playerAnchor.position;

            float deltaY = entryWorld.y - anchorWorld.y;

            Vector2 startPos = content.anchoredPosition;
            Vector2 targetPos = startPos - new Vector2(0, deltaY);

            _tween = content
                .DOAnchorPos(targetPos, moveTime)
                .SetEase(ease)
                .SetTarget(this);

            try
            {
                await _tween.ToUniTask(cancellationToken: token);
            }
            catch
            {
            }
            finally
            {
                content.anchoredPosition = targetPos;
            }
        }

        public void Skip()
        {
            if (_tween != null && _tween.IsActive())
                _tween.Kill(true);
        }
    }
}