using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace PT.GameplayAdditional.Animations
{
    public class SpinTextLoop : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI spinText;
        [SerializeField] private bool keepStartText = true;
        [SerializeField] private string[] loopTexts;
        [SerializeField] private float loopDuration = 0.5f;

        private string _startText = "";
        private int _currentTextIndex;
        private bool _isLooping;

        private void OnDisable()
        {
            StopLoop();
        }
        private void OnDestroy()
        {
            StopLoop();
        }

        public void StartLoop()
        {
            if (_isLooping) return;

            if (_startText == "") _startText = spinText.text;

            _isLooping = true;
            LoopText().Forget();
        }

        public void StopLoop()
        {
            _isLooping = false;
            if (_startText != "") spinText.text = _startText;
        }

        private async UniTask LoopText()
        {
            while (_isLooping)
            {
                SetText(loopTexts[_currentTextIndex % loopTexts.Length]);

                _currentTextIndex++;
                await UniTask.Delay(TimeSpan.FromSeconds(loopDuration), cancellationToken: this.GetCancellationTokenOnDestroy());
            }
        }

        private void SetText(string text) => spinText.text = (keepStartText ? _startText : "") + spinText;
    }
}
