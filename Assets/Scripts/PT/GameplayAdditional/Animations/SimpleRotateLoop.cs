using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace PT.GameplayAdditional.Animations
{
    public class SimpleRotateLoop : MonoBehaviour
    {
        [SerializeField, Min(0.1f), MaxValue(14)] private float speed = 4f;
        [SerializeField] private bool invert;
        [SerializeField] private bool useUnscaledTime = true;

        private Tween _tween;

        private void OnEnable()
        {
            StartRotation();
        }

        private void OnDisable()
        {
            StopRotation();
        }

        private void OnDestroy()
        {
            StopRotation();
        }

        private void StartRotation()
        {
            StopRotation();

            if (!gameObject.activeInHierarchy) return;

            float direction = invert ? -1f : 1f;

            _tween = transform
                .DORotate(
                    new Vector3(0f, 0f, 360f * direction),
                    speed,
                    RotateMode.WorldAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental)
                .SetUpdate(useUnscaledTime);
        }

        private void StopRotation()
        {
            if (_tween == null) return;

            try
            {
                if (_tween.IsActive())
                {
                    if (_tween.IsPlaying()) _tween.Complete(true);
                    _tween.Kill();
                }
            }
            catch { }
            finally { _tween = null; }
        }
    }
}