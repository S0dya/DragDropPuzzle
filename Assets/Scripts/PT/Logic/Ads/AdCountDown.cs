using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using PT.Logic.Dependency.Signals;
using Zenject;

namespace PT.Logic.Ads
{
    public class AdCountDown : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField][Min(0)] private int countDownAmount = 3;
        [SerializeField] private float countDownDuration = 1;

        [Header("UI")]
        // [SerializeField] private  uiTabHandler;
        [SerializeField] private GameObject adCountDownWindow;
        [SerializeField] private TextMeshProUGUI countDownText;
        
        [Inject] private SignalBus _signalBus;

        private bool _lastAdCountDown;

        private int _currentCount;
        private int CurrentCount 
        {
            get { return _currentCount; }
            set { _currentCount = value; countDownText.text = _currentCount.ToString(); } 
        }

        private void Awake()
        {
            _signalBus.Subscribe<AdCountdownStartSignal>(OnAdCountDownStart);
            _signalBus.Subscribe<AdClosedSignal>(OnAdClosed);
        }

        private async void OnAdCountDownStart()
        {
            adCountDownWindow.SetActive(true);

            await CountDownTask();

            _signalBus.Fire(new ShowAdSignal());
        }
        private void OnAdClosed()
        {
            if (_lastAdCountDown)
            {
                _lastAdCountDown = false;

                _signalBus.Fire(new AdCountdownStopSignal());
            }
        }

        private async UniTask CountDownTask()
        {
            CurrentCount = countDownAmount;

            while (CurrentCount > 0)
            {
                await UniTask.WaitForSeconds(countDownDuration);

                CurrentCount--;
            }

            _lastAdCountDown = true;
        }
    }
}
