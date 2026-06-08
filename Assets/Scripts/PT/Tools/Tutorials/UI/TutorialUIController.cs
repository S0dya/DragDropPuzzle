using Cysharp.Threading.Tasks;
using DG.Tweening;
using PT.Logic.Configs;
using PT.Logic.ProjectContext;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.Tools.Tutorials.UI
{
    public class TutorialUIController : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private TutorialPointer pointer; 
        [SerializeField] private TutorialInputBlocker blocker;
        [Space]
        [Header("Modal")]
        [SerializeField] private GameObject modalRoot;
        [SerializeField] private TextMeshProUGUI modalTitle;
        [SerializeField] private TextMeshProUGUI modalBody;
        [Space]
        [Header("Highlight")]
        [SerializeField] private RectTransform highlightRect; 
        [SerializeField] private CanvasGroup highlightCanvasGroup;
        [SerializeField] private Vector2 defaultHighlightSize = new(120f, 120f);
        [Space]
        [Header("Invalid feedback")]
        [SerializeField] private RectTransform invalidShakeTarget;
        [SerializeField] private float invalidShakeDuration = 0.35f;
        [SerializeField] private float invalidShakeStrength = 12f;

        [Inject] private LanguageManager _languageManager;
         
        private Vector2 _invalidStartPos;
        
        private void Awake()
        {
            _invalidStartPos = invalidShakeTarget.anchoredPosition;
            
            if (!canvas) canvas = GetComponentInParent<Canvas>();

            modalRoot.SetActive(false);
            highlightCanvasGroup.alpha = 0f;
            
                pointer.Hide();
                blocker.Unblock(); 
        }

        public void HideAllUI()
        {
            HideModal();
            HideHighlight();
            HidePointer();
        }
        
        public void ShowText(LocalizationKeyEnum tutorialTitle, LocalizationKeyEnum tutorialDesc)
        {
            if (modalTitle) modalTitle.text = _languageManager.GetLocalizedString(tutorialTitle);
            if (modalBody) modalBody.text = _languageManager.GetLocalizedString(tutorialDesc);

            modalRoot.SetActive(true);
        }
        public void HideModal()
        {
            modalRoot.SetActive(false);
        }
        
        public void HighlightAt(Vector2 screenPosition, Vector2? sizeOverride = null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPosition,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out var localPos
            );

            highlightRect.DOKill();
            highlightRect.anchoredPosition = localPos;
            highlightRect.sizeDelta = sizeOverride ?? defaultHighlightSize;

            highlightCanvasGroup.DOKill();
            highlightCanvasGroup.DOFade(1f, 0.25f).SetUpdate(true); //myb rewrite into settings from config
        }
        public void HideHighlight()
        {
            highlightCanvasGroup.DOKill();
            highlightCanvasGroup.DOFade(0f, 0.15f).SetUpdate(true);
        }

        public void ShowPointer(Vector2 screenPos)
        {
            if (pointer != null) pointer.ShowAt(screenPos);
        }
        public void HidePointer()
        {
            if (pointer != null) pointer.Hide();
        }
        
        public void BlockInput()
        {
            if (blocker != null) blocker.Block();
        }
        public void UnblockInput()
        {
            if (blocker != null) blocker.Unblock();
        }
        
        public void PulseInvalid()
        {
            invalidShakeTarget.DOKill(true);
            invalidShakeTarget.anchoredPosition = _invalidStartPos;

            invalidShakeTarget.DOShakeAnchorPos(
                    duration: invalidShakeDuration,
                    strength: new Vector2(invalidShakeStrength, 0f),
                    vibrato: 15,
                    randomness: 0,
                    snapping: false,
                    fadeOut: true
                ).SetUpdate(true)
                .OnComplete(() => invalidShakeTarget.anchoredPosition = _invalidStartPos);
        }
    }
}