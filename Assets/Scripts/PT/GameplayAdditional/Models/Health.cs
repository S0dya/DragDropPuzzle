using System;
using UniRx;
using UnityEngine;

namespace PT.GameplayAdditional.Models
{
    public class Health
    {
        public int InitialValue
        {
            get => initialValue;
            private set
            {
                initialValue = value;
                InitialValueReactive.Value = value;
            }
        }

        private int _value;
        private int initialValue;

        public int Value
        {
            get => _value;
            private set
            {
                if (value == _value)
                {
                    return;
                }

                _value = value;
                ValueReactive.Value = value;
            }
        }

        public ReactiveProperty<int> InitialValueReactive { get; private set; } = new();

        public ReactiveProperty<int> ValueReactive { get; private set; } = new();

        public float NormalizedValue => (float)Value / InitialValue;
        public bool IsNotFull => Value < InitialValue;

        public event Action<int, GameObject> DamageTaken = delegate { };
        public event Action<int> Healed = delegate { };
        public event Action<Health> ValueBelowMinimum = delegate { };

        public Health(int value, int initialValue)
        {
            InitialValue = initialValue;
            Value = value;
        }

        public void Reset()
        {
            Value = InitialValue;
        }

        public void TakeDamage(int damageAmout, GameObject damageDealer)
        {
            if (damageAmout < 0)
            {
                return;
            }

            if (Value == 0)
            {
                return;
            }

            if (Value - damageAmout <= 0)
            {
                damageAmout = Value;
                Value = 0;
            }
            else
            {
                Value -= damageAmout;
            }

            DamageTaken(damageAmout, damageDealer);

            if (Value <= 0) ValueBelowMinimum(this);
        }

        public void Heal(int healAmount)
        {
            if (healAmount < 0)
            {
                return;
            }

            Value += healAmount;

            if (Value > InitialValue)
                Value = InitialValue;

            Healed(healAmount);
        }

        public void ChangeMaxHealth(int shift) => SetMaxHealth(InitialValue + shift);

        public void SetMaxHealth(int value)
        {
            InitialValue = value;
            Value = Mathf.Min(Value, InitialValue);
        }

        public void SetHealth(int value)
        {
            Value = value;
            InitialValue = Mathf.Max(InitialValue, value);
        }
    }
}