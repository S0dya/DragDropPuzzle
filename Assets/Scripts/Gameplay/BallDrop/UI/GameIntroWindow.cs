using Cysharp.Threading.Tasks;
using PT.GameplayAdditional.Input;
using PT.Logic.Dependency.Signals;
using PT.Tools.Windows;
using TMPro;
using UnityEngine;
using Zenject;
using DG.Tweening;
using PT.Tools.Helper;

namespace Gameplay.BallDrop.UI
{
    public class GameIntroWindow : WindowBase
    {
        [SerializeField] private Transform ballsSpawnHolderObj;
        [Space]
        [SerializeField] private GameObject timerObj;
        [SerializeField] private TextMeshProUGUI timerText;
        [Space]
        [SerializeField] private int timerDuration = 3;

        [Inject (Id = "Game")] private WindowsManager _windowsManager;
        [Inject] private InputManager _inputManager;
        [Inject] private SignalBus _signalBus;

        private Sequence _timerSequence;

        protected override async UniTask OnOpen()
        {
            await base.OnOpen();

            PlayTimer();
        }
        
        private void TimerFinished()
        {
            timerObj.SetActive(false);
            ballsSpawnHolderObj.SetActive(false);
            
            _signalBus.Fire(new GameStartedSignal());
        }

        private void PlayTimer()
        {
            if (_timerSequence != null && _timerSequence.IsActive())
            {
                _timerSequence.Kill();
                _timerSequence = null;
            }

            timerObj.SetActive(true);
            timerText.text = timerDuration.ToString();
            timerText.transform.localScale = Vector3.one;

            _timerSequence = DOTween.Sequence();

            for (int i = timerDuration; i > 0; i--)
            {
                int display = i; 
                _timerSequence.AppendCallback(() => { if (timerText != null) timerText.text = display.ToString(); });

                _timerSequence.Append(timerText.transform.DOScale(1.4f, 0.25f).SetEase(Ease.OutBack));
                _timerSequence.Append(timerText.transform.DOScale(1f, 0.15f));

                _timerSequence.AppendInterval(Mathf.Max(0f, 1f - (0.25f + 0.15f)));
            }

            _timerSequence.AppendCallback(() => { if (timerText != null) timerText.text = "0"; });
            _timerSequence.Append(timerText.transform.DOScale(1.6f, 0.18f).SetEase(Ease.OutBack));
            _timerSequence.AppendInterval(0.2f);
            
            _timerSequence.AppendCallback(TimerFinished);

            _timerSequence.Play();
        }
    }
}