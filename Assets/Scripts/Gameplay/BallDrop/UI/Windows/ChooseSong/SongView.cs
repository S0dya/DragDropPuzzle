using Gameplay.BallDrop.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.BallDrop.UI.Windows.ChooseSong
{
    public class SongView : MonoBehaviour
    {
        [SerializeField] private Image coverImage;
        [SerializeField] private TextMeshProUGUI songNameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        
        public void Init(SongData songData)
        {
            coverImage.sprite = songData.CoverImage;
            songNameText.text = songData.Name;
            songNameText.text = songData.Name;
        }
    }
}