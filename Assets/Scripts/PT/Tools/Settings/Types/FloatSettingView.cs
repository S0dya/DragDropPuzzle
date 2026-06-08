using PT.Tools.Settings.Views;
using UnityEngine;
using UnityEngine.UI;

namespace PT.Tools.Settings.Types
{
    public class FloatSettingView : SettingView
    {
        [SerializeField] private Slider slider;

        private bool _changingFromCode;

        public override void Init()
        {
            base.Init();
            slider.onValueChanged.AddListener(OnSliderChanged);
        }

        private void OnSliderChanged(float value)
        {
            if (_changingFromCode) return;
            OnUserChangedValue(value);
        }

        protected override void OnSettingChanged(object value)
        {
            _changingFromCode = true;
            
            slider.SetValueWithoutNotify((float)value);
            
            _changingFromCode = false;
        }

        protected override void OnUserChangedValue(object value) => SetValue(value);
    }
}