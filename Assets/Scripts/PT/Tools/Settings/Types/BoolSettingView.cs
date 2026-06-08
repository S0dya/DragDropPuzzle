using PT.Tools.Settings.Views;
using UnityEngine;
using UnityEngine.UI;

namespace PT.Tools.Settings.Types
{
    public class BoolSettingView : SettingView
    {
        [SerializeField] private Button button;
        [Space]
        [SerializeField] private Image icon;
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;

        private bool _current;

        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                _current = !_current;
                OnUserChangedValue(_current);
            });
        }

        protected override void OnSettingChanged(object value)
        {
            _current = (bool)value;
            icon.sprite = _current ? onSprite : offSprite;
        }

        protected override void OnUserChangedValue(object value) => SetValue(value);
    }
}