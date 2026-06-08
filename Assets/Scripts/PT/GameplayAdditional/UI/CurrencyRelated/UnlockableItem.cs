using System;
using PT.Logic.Ads;
using PT.Logic.Save;
using PT.Tools.CurrencyRelated;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.GameplayAdditional.UI.CurrencyRelated
{
    public abstract class UnlockableItem : MonoBehaviour
    {
        [SerializeField] private GameObject mainObj;
        [Space]
        [SerializeField] private CurrencyType relatedCurrencyType;
        [SerializeField] private int relatedCurrencyAmount;
        [Space]
        [SerializeField] private GameObject moneyUnlockingObj;
        [SerializeField] private Button[] moneyUnlockingButtons;
        [Space]
        [SerializeField] private GameObject adUnlockingObj;
        [SerializeField] private Button[] adUnlockingButtons;
        [Space]
        [SerializeField] private TextMeshProUGUI[] moneyTexts;

        public event Action OnUnlocked;
        
        [Inject] private CurrencyManager _currencyManager;
        [Inject] private AdsManager _adsManager;
        
        protected abstract GameDataKey UnlockStateKey { get; }
        protected abstract int ItemIndex { get; }
        
        protected virtual void Start()
        {
            _currencyManager.GetReactiveValue(relatedCurrencyType)
                .Subscribe(OnCurrencyChanged)
                .AddTo(this);
            
            foreach (var moneyText in moneyTexts) moneyText.text = relatedCurrencyAmount.ToString();
            
            foreach (var moneyButton in moneyUnlockingButtons) moneyButton.onClick.AddListener(OnMoneyUnlockingButtonClick);
            foreach (var adButton in adUnlockingButtons) adButton.onClick.AddListener(OnAdUnlockingButtonClick);
        }

        private void OnEnable()
        {
            RefreshUnlockState();
        }

        public void SetUnlocked(bool unlocked)
        {
            mainObj.SetActive(!unlocked);
        }
        
        protected virtual void RefreshUnlockState()
        {
            bool isUnlocked = GetUnlockState();
            SetUnlocked(isUnlocked);
        }
        
        protected virtual bool GetUnlockState()
        {
            int unlockInt = (int)GameDataRegistry.Get(UnlockStateKey);
            string unlockString = unlockInt.ToString();

            if (ItemIndex >= unlockString.Length) return false;

            return unlockString[ItemIndex] == '1';
        }

        protected virtual void SetUnlockState(bool unlocked)
        {
            int unlockInt = (int)GameDataRegistry.Get(UnlockStateKey);
            string unlockString = unlockInt.ToString();

            while (unlockString.Length <= ItemIndex)
            {
                unlockString += "0";
            }

            char[] chars = unlockString.ToCharArray();
            chars[ItemIndex] = unlocked ? '1' : '0';
            unlockString = new string(chars);

            unlockInt = int.Parse(unlockString);
            GameDataRegistry.Set(UnlockStateKey, unlockInt);
        }
        
        protected virtual void OnCurrencyChanged(int amount)
        {
            moneyUnlockingObj.SetActive(amount >= relatedCurrencyAmount);
            adUnlockingObj.SetActive(amount < relatedCurrencyAmount);
        }
        
        protected virtual void OnMoneyUnlockingButtonClick()
        {
            if (_currencyManager.TrySpend(relatedCurrencyType, relatedCurrencyAmount))
            {
                SetUnlockState(true);
                OnUnlocked?.Invoke();
                SetUnlocked(true);
            }
        }
        
        protected virtual void OnAdUnlockingButtonClick()
        {
            _adsManager.ShowRewardAd(() =>
            {
                SetUnlockState(true);
                OnUnlocked?.Invoke();
                SetUnlocked(true);
            });
        }
    }
}