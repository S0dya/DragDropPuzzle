    using System;
    using System.Collections.Generic;
    using PT.Logic.Save;
    using PT.Tools.Other;
using UniRx;

namespace PT.Tools.CurrencyRelated
{
    public enum CurrencyType
    {
        Gold,
    }
    
    public class CurrencyManager : IDisposable
    {
        private Dictionary<CurrencyType, ReactiveProperty<int>> _balancesValues = new();
        private Dictionary<CurrencyType, Subject<ValueChange>> _balancesChanges = new();
        
        private Dictionary<CurrencyType, GameDataKey> _savingKeys = new()
        {
            { CurrencyType.Gold, GameDataKey.Gold },
        };
        
        public void Init()
        {
            foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
            {
                int value = (int)GameDataRegistry.Get(_savingKeys[type]);
                var reactive = new ReactiveProperty<int>(value);
                
                reactive.Subscribe(val => { GameDataRegistry.Set(_savingKeys[type], val); });

                _balancesValues[type] = reactive;
                _balancesChanges[type] = new ();
            }
        }
        public void Dispose()
        {
            foreach (var kvp in _balancesValues)
            {
                GameDataRegistry.Set(_savingKeys[kvp.Key], kvp.Value.Value);
            }
        }

        public ReactiveProperty<int> GetReactiveValue(CurrencyType type) => _balancesValues[type];
        public Subject<ValueChange> GetReactiveChange(CurrencyType type) => _balancesChanges[type];
        public int Get(CurrencyType type) => _balancesValues[type].Value;
        
        public void Set(CurrencyType type, int value)
        {
            _balancesValues[type].Value = value;
        }

        public void Add(CurrencyType type, int amount)
        {
            _balancesChanges[type].OnNext(new (amount, ValueChangeType.Add));
            _balancesValues[type].Value += amount;
        }

        public bool TrySpend(CurrencyType type, int amount)
        {
            if (_balancesValues[type].Value < amount) return false;
            
            _balancesChanges[type].OnNext(new (amount, ValueChangeType.Subtract));;
            _balancesValues[type].Value -= amount;

            return true;
        }
    }
}