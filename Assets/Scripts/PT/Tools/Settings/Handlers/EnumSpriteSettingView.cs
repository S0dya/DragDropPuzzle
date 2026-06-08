using System;
using PT.Tools.Settings.Views;
using UnityEngine;
using UnityEngine.UI;

namespace PT.Tools.Settings.Handlers
{
    // add if needed later
    // public class EnumSpriteSettingView<TEnum> : SettingView where TEnum : Enum
    // {
    //     [SerializeField] private Button button;
    //     [SerializeField] private Image icon;
    //     [SerializeField] private Sprite[] sprites;
    //
    //     private TEnum _current;
    //
    //     private void Awake()
    //     {
    //         button.onClick.AddListener(() =>
    //         {
    //             int next = (Convert.ToInt32(_current) + 1) % sprites.Length;
    //             _current = (TEnum)Enum.ToObject(typeof(TEnum), next);
    //             OnUserChangedValue(_current);
    //         });
    //     }
    //
    //     protected override void OnSettingChanged(object value)
    //     {
    //         if (value is TEnum e)
    //             _current = e;
    //         else
    //             _current = (TEnum)Enum.ToObject(typeof(TEnum), (int)value);
    //
    //         icon.sprite = sprites[Convert.ToInt32(_current)];
    //     }
    //
    //     protected override void OnUserChangedValue(object value) => SetValue(value);
    // }
}