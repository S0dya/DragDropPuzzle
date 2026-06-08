using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.GameplayAdditional.Progression
{
    public class StageView : MonoBehaviour
    {
        [SerializeField] private Image progressFill;
        [SerializeField] private TextMeshProUGUI percentText;
        [SerializeField] private Image currentStageIcon;
        [SerializeField] private Image nextStageIcon;
        [SerializeField] private RectTransform percentTextContainer;

        [Inject] private StageProvider _stageProvider;

        private RectTransform _textRect;
        private float _barWidth;

        private void Awake()
        {
            _textRect = percentText.GetComponent<RectTransform>();

            if (percentTextContainer == null)
                percentTextContainer = progressFill.transform.parent as RectTransform;
        }

        private void Start()
        {
            Refresh();
        }

        private void Refresh()
        {
            var current = _stageProvider.GetCurrentStageInfo();
            var next = _stageProvider.GetNextStageInfo();
            float progress = _stageProvider.GetStageProgress();

            currentStageIcon.sprite = current.Icon;
            nextStageIcon.sprite = next.Icon;
            progressFill.fillAmount = progress;
            percentText.text = $"{Mathf.RoundToInt(progress * 100)}%";

            UpdatePercentTextPosition(progress);
        }

        private void UpdatePercentTextPosition(float progress)
        {
            if (percentTextContainer == null) return;

            _barWidth = percentTextContainer.rect.width;

            float xPos = Mathf.Lerp(0, _barWidth, progress);
            float safeMargin = 20f; 
            xPos = Mathf.Clamp(xPos - _barWidth * 0.5f, -_barWidth * 0.5f + safeMargin, _barWidth * 0.5f - safeMargin);

            var anchoredPos = _textRect.anchoredPosition;
            anchoredPos.x = xPos;
            _textRect.anchoredPosition = anchoredPos;
        }
    }
}