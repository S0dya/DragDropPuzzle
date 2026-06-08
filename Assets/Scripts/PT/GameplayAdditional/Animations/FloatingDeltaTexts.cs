using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using PT.Tools.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace PT.GameplayAdditional.Animations
{
    public class FloatingDeltaTexts : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private TextMeshProUGUI textPrefab;
        [SerializeField] private Transform textsParent;
        [SerializeField] private RectTransform targetRect;
        [Header("Colors")]
        [SerializeField] private Color addColor = Color.green;
        [SerializeField] private Color subtractColor = Color.red;
        [Header("Motion")]
        [SerializeField] private float spawnBelowTarget = 40f;
        [SerializeField] private float popupScale = 1.2f;
        [SerializeField] private float popupTime = 0.12f;
        [SerializeField] private float moveTime = 0.35f;
        [Header("Random")]
        [SerializeField][MinMaxSlider(-50, 50)] private Vector2 randomOffsetX = new(-25f, 25f);
        [SerializeField][MinMaxSlider(0.1f, 2f)] private Vector2 randomScale = new(0.9f, 1.1f);
        [SerializeField][MinMaxSlider(0.3f, 1f)] private Vector2 randomAlpha = new(0.6f, 0.8f);
        [Header("Stacking")]
        [SerializeField] private float offsetXPerActive = 12f;
        [SerializeField] private float maxOffsetMultiplier = 3f;
        
        private ObjectPool<TextMeshProUGUI> _pool;
        private readonly List<Sequence> _sequences = new();
        private readonly List<TextMeshProUGUI> _activeLabels = new();
        private int _sortingIndex;

        private void Awake()
        {
            _pool = new ObjectPool<TextMeshProUGUI>(
                () => { return Instantiate(textPrefab, textsParent); },
                t => { t.gameObject.SetActive(true); },
                t => { t.gameObject.SetActive(false); },
                defaultCapacity: 8, maxSize: 32);
        }

        public void PlayAdd(int value)
        {
            Play($"+{value}", addColor);
        }
        public void PlaySubtract(int value)
        {
            Play($"-{value}", subtractColor);
        }

        private void Play(string text, Color color)
        {
            if (!isActiveAndEnabled) return;
            
            var label = _pool.Get();
            var rect = label.rectTransform;

            rect.SetParent(textsParent);
            rect.SetAsLastSibling();

            int activeCount = _sequences.Count;
            float spreadMultiplier = Mathf.Clamp(1f + activeCount * 0.15f, 1f, maxOffsetMultiplier);

            float minX = randomOffsetX.x * spreadMultiplier;
            float maxX = randomOffsetX.y * spreadMultiplier;

            var startPos = targetRect.anchoredPosition;
            startPos.y -= spawnBelowTarget;
            startPos.x += Random.Range(minX, maxX);

            rect.anchoredPosition = startPos;
            rect.localScale = Vector3.zero;

            label.text = text;
            label.color = color;
            label.alpha = randomAlpha.GetRandomValue();

            float scaleRand = randomScale.GetRandomValue();

            var seq = DOTween.Sequence();
            _sequences.Add(seq);

            seq.Append(rect.DOScale(popupScale * scaleRand, popupTime).SetEase(Ease.OutBack));
            seq.Append(rect.DOScale(1f, popupTime * 0.6f).SetEase(Ease.OutQuad));
            seq.Join(rect.DOAnchorPos(targetRect.anchoredPosition, moveTime).SetEase(Ease.OutCubic));
            seq.Append(label.DOFade(0f, moveTime).SetEase(Ease.OutQuad));

            seq.OnComplete(() =>
            {
                if (label == null) return;
                
                _activeLabels.Remove(label);
                _sequences.Remove(seq);
                _pool.Release(label);
            });

            seq.SetTarget(label);
        }
        
        private void OnDisable()
        {
            KillAll();
        }
        private void OnDestroy()
        {
            KillAll();
        }
        
        private void KillAll()
        {
            foreach (var s in _sequences) s?.Kill(false);

            _sequences.Clear();

            foreach (var label in _activeLabels)
            {
                if (label) _pool.Release(label);
            }

            _activeLabels.Clear();
        }
    }
}