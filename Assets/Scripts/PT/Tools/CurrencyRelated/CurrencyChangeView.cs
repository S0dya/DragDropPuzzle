using PT.GameplayAdditional.Animations;
using PT.Tools.Other;
using UniRx;
using UnityEngine;
using Zenject;

namespace PT.Tools.CurrencyRelated
{
    public class CurrencyChangeView : MonoBehaviour
    {
        [SerializeField] private CurrencyView currencyView;
        [SerializeField] private FloatingDeltaTexts deltaTexts;

        [Inject] private CurrencyManager _currencyManager;

        private void Awake()
        {
            if (!currencyView) currencyView = GetComponent<CurrencyView>();
            if (!deltaTexts) deltaTexts = GetComponent<FloatingDeltaTexts>();
        }
        
        private void Start()
        {
            _currencyManager.GetReactiveChange(currencyView.UsedType)
                .Subscribe(OnValueChanged)
                .AddTo(this);
        }
        
        private void OnValueChanged(ValueChange valueChange)
        {
            switch (valueChange.Type)
            {
                case ValueChangeType.Add: deltaTexts.PlayAdd(valueChange.Value); break;
                case ValueChangeType.Subtract: deltaTexts.PlaySubtract(-(valueChange.Value)); break;
            }
        }
    }
}