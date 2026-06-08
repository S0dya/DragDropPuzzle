using System;

namespace PT.GameplayAdditional.Models
{
    public class Armor
    {
        private int _value;

        public int Value
        {
            get => _value; private set
            {
                if (value == _value)
                {
                    return;
                }

                _value = value;
            }
        }
        public int InitialValue { get; private set; }

        public bool HasArmor => Value > 0;

        public event Action<int> DamageTaken = delegate { };

        public Armor(int value)
        {
            Value = InitialValue = value;
        }

        public void TakeDamage(int damage)
        {
            Value -= damage;

            if (Value < 0)
                Value = 0;

            DamageTaken(damage);
        }
    }
}