using PT.GameplayAdditional.Animations;
using PT.Tools.Other;
using UniRx;
using UnityEngine;
using Zenject;

namespace PT.Tools.Score
{
    public class ScoreChangeView : MonoBehaviour
    {
        [SerializeField] private ScoreView scoreView;
        [SerializeField] private FloatingDeltaTexts deltaTexts;

        [Inject] private ScoreManager _scoreManager;

        private void Awake()
        {
            if (!scoreView) scoreView = GetComponent<ScoreView>();
            if (!deltaTexts) deltaTexts = GetComponent<FloatingDeltaTexts>();
        }

        private void OnEnable()
        {
            _scoreManager.CurrentScoreChange
                .Skip(1)  
                .Subscribe(OnScoreChangeAnimated)
                .AddTo(this);
        }

        private void OnScoreChangeAnimated(ValueChange valueChange)
        {
            switch (valueChange.Type)
            {
                case ValueChangeType.Add: deltaTexts.PlayAdd(valueChange.Value); break;
                case ValueChangeType.Subtract: deltaTexts.PlaySubtract(-valueChange.Value); break;
            }
        }
    }
}