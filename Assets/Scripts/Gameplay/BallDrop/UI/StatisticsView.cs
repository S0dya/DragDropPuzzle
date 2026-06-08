using Gameplay.BallDrop.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.BallDrop.UI
{
    public class StatisticsView : MonoBehaviour
    {
        [SerializeField] private Image characterSpriteImage;
        [SerializeField] private Image percentFillImage;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI percentText;

        public void SetData(CharacterData characterData, float percent)
        {
            characterSpriteImage.sprite = characterData.BallSprite;
            characterNameText.text = characterData.CharacterNameKey;
            percentFillImage.color = characterData.CharacterColor;
            percentFillImage.fillAmount = Mathf.Clamp01(percent);
            percentText.text = $"{percent:P1}%";
        }
    }
}
