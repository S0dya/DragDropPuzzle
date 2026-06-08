using TMPro;
using UniRx;
using UnityEngine;

namespace PT.Tools.RewWheel
{
    public class RewardWheelTextView : MonoBehaviour
    {
        [SerializeField] private RewardWheel rewardWheel;
        [Space]
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private TextMeshProUGUI multiplierText;

        private CompositeDisposable _disposables;
        
        private void Start()
        {
            if (valueText)
            {
                rewardWheel.CurrentRewardValue
                    .Subscribe(val => valueText.text = val.ToString())
                    .AddTo(this);
            }
            if (multiplierText)
            {
                rewardWheel.CurrentMultiplier
                    .Subscribe(val => multiplierText.text = $"x{val}")
                    .AddTo(this);
            }
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}