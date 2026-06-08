using PT.Tools.Settings.Views;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

namespace PT.Tools.Settings.Types
{
    public class IntSliderSettingView : SettingView
    {
        [SerializeField] private Slider slider;

        private bool _changingFromCode;
        private int _current;

        private void Awake()
        {
            slider.onValueChanged.AddListener(val =>
            {
                if (_changingFromCode) return;
                OnUserChangedValue((int)val);
            });
        }

        protected override void OnSettingChanged(object value)
        {
            _changingFromCode = true;
            
            _current = (int)value;
            slider.SetValueWithoutNotify(_current);
            
            _changingFromCode = false;
        }

        protected override void OnUserChangedValue(object value) => SetValue(value);
    }
}