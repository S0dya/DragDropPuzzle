using PT.Tools.Settings.Views;
using UnityEngine;
using UnityEngine.UI;

namespace PT.Tools.Settings.Types
{
    public class IntSpriteSettingView : SettingView
    {
        [SerializeField] private Button button;
        [Space]
        [SerializeField] private Image icon;
        [SerializeField] private Sprite[] sprites;

        private int _current;

        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                _current = (_current+1) % sprites.Length;
                OnUserChangedValue(_current);
            });
        }

        protected override void OnSettingChanged(object value)
        {
            _current = (int)value;
            icon.sprite = sprites[_current];
        }

        protected override void OnUserChangedValue(object value) => SetValue(value);
    }
}