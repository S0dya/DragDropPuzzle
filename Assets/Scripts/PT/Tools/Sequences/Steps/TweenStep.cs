using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace PT.Tools.Sequences.Steps
{
    [Serializable]
    public class TweenStep : SequenceStep
    {
        public enum TweenType
        {
            Scale,
            Move,
            MoveDistance,
            Rotate,
            Fade
        }

        [SerializeField] private TweenType type;
        [SerializeField] private Vector3 targetVector;
        [SerializeField] private Transform targetTransform;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private Ease ease = Ease.OutQuad;

        private Tween _tween;

        public override async UniTask PlayStep(SequenceInfo sequenceInfo, CancellationToken token)
        {
            _tween?.Kill();
            _tween = null;
        
            var transf = sequenceInfo.TargetTransform;
            
            switch (type)
            {
                case TweenType.Scale:
                    _tween = transf.DOScale(targetVector, duration).SetEase(ease);
                    break;
                case TweenType.Move:
                    _tween = transf.DOMove(targetTransform.position, duration).SetEase(ease);
                    break;
                case TweenType.MoveDistance:
                    _tween = transf.DOLocalMove(targetVector, duration).SetEase(ease);
                    break;
                case TweenType.Rotate:
                    _tween = transf.DOLocalRotate(targetVector, duration).SetEase(ease);
                    break;
                case TweenType.Fade:
                    var cg = sequenceInfo.TargetTransform.GetComponent<CanvasGroup>() 
                             ?? sequenceInfo.TargetTransform.gameObject.AddComponent<CanvasGroup>();
                    _tween = cg.DOFade(targetVector.x, duration).SetEase(ease);
                    break;
            }
            
            using (token.Register(() => SkipStep(sequenceInfo)))
            {
                await _tween.AsyncWaitForCompletion();
            }
        }
        
        public override void SkipStep(SequenceInfo sequenceInfo)
        {
            _tween?.Complete(true);
            _tween?.Kill();
            _tween = null;
            
            var transf = sequenceInfo.TargetTransform;
            
            switch (type)
            {
                case TweenType.Scale:
                    transf.localScale = targetVector;
                    break;
                case TweenType.Move:
                    if (targetTransform) transf.position = targetTransform.position;
                    break;
                case TweenType.MoveDistance:
                    transf.localPosition = targetVector;
                    break;
                case TweenType.Rotate:
                    transf.localEulerAngles = targetVector;
                    break;
                case TweenType.Fade:
                    var cg = transf.GetComponent<CanvasGroup>() 
                             ?? transf.gameObject.AddComponent<CanvasGroup>();
                    cg.alpha = targetVector.x;
                    break;
            }
        }
        
        //myb safer version
        //
        // public override async UniTask PlayStep(SequenceInfo sequenceInfo, CancellationToken token)
        // {
        //     SafeKillTween();
        //
        //     var transf = sequenceInfo?.TargetTransform;
        //     if (!transf || !transf.gameObject.activeInHierarchy)
        //         return;
        //
        //     try
        //     {
        //         switch (type)
        //         {
        //             case TweenType.Scale:
        //                 _tween = transf.DOScale(targetVector, duration).SetEase(ease);
        //                 break;
        //
        //             case TweenType.Move:
        //                 if (targetTransform && targetTransform)
        //                     _tween = transf.DOMove(targetTransform.position, duration).SetEase(ease);
        //                 break;
        //
        //             case TweenType.MoveDistance:
        //                 _tween = transf.DOLocalMove(targetVector, duration).SetEase(ease);
        //                 break;
        //
        //             case TweenType.Rotate:
        //                 _tween = transf.DOLocalRotate(targetVector, duration).SetEase(ease);
        //                 break;
        //
        //             case TweenType.Fade:
        //                 var cg = transf.TryGetComponent(out CanvasGroup cgComp)
        //                     ? cgComp
        //                     : transf.gameObject.AddComponent<CanvasGroup>();
        //                 _tween = cg.DOFade(targetVector.x, duration).SetEase(ease);
        //                 break;
        //         }
        //
        //         if (_tween == null || !_tween.active) return;
        //
        //         using (token.Register(() => SafeSkip(sequenceInfo)))
        //             await _tween.AsyncWaitForCompletion();
        //     }
        //     catch (Exception e)
        //     {
        //         SafeKillTween();
        //     }
        // }
        //
        // public override void SkipStep(SequenceInfo sequenceInfo)
        // {
        //     SafeSkip(sequenceInfo);
        // }
        //
        // private void SafeSkip(SequenceInfo info)
        // {
        //     SafeKillTween();
        //
        //     var transf = info?.TargetTransform;
        //     if (!transf || !transf.gameObject.activeInHierarchy)
        //         return;
        //
        //     try
        //     {
        //         switch (type)
        //         {
        //             case TweenType.Scale:
        //                 transf.localScale = targetVector;
        //                 break;
        //
        //             case TweenType.Move:
        //                 if (targetTransform)
        //                     transf.position = targetTransform.position;
        //                 break;
        //
        //             case TweenType.MoveDistance:
        //                 transf.localPosition = targetVector;
        //                 break;
        //
        //             case TweenType.Rotate:
        //                 transf.localEulerAngles = targetVector;
        //                 break;
        //
        //             case TweenType.Fade:
        //                 var cg = transf.TryGetComponent(out CanvasGroup cgComp)
        //                     ? cgComp
        //                     : transf.gameObject.AddComponent<CanvasGroup>();
        //                 cg.alpha = targetVector.x;
        //                 break;
        //         }
        //     }
        //     catch (Exception e) { }
        // }
        //
        // private void SafeKillTween()
        // {
        //     try
        //     {
        //         if (_tween != null && _tween.IsActive())
        //         {
        //             if (_tween.IsPlaying()) _tween.Complete(true);
        //             _tween.Kill();
        //         }
        //     }
        //     catch { }
        //     finally
        //     {
        //         _tween = null;
        //     }
        // }
    }
}