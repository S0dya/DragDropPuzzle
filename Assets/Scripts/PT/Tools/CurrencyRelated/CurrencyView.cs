using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace PT.Tools.CurrencyRelated
{
    public class CurrencyView : MonoBehaviour
    {
        [SerializeField] private CurrencyType type;
        [SerializeField] private TextMeshProUGUI text;

        internal CurrencyType UsedType => type;
        
        [Inject] private CurrencyManager _currencyManager;

        private void Start()
        {
            _currencyManager.GetReactiveValue(type)
                .Subscribe(OnNewValue)
                .AddTo(this);
        }
        
        private void OnNewValue(int value)
        {
            text.text = value.ToString();
        }
    }
}