using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace PT.Tools.Score
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        [Inject] private ScoreManager _scoreManager;

        private void OnEnable()
        {
            _scoreManager.CurrentScoreReactive
                .Subscribe(OnScoreChanged)
                .AddTo(this);
        }

        private void OnScoreChanged(int value)
        {
            text.text = value.ToString();
        }
    }
}