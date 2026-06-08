using System;
using System.Linq;
using Gameplay.BallDrop.Configs;
using Gameplay.BallDrop.Datas;
using PT.GameplayAdditional.UI.CurrencyRelated;
using PT.Logic.Save;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.BallDrop.UI.Windows.ChooseSong
{
    public class SongCardView : MonoBehaviour
    {
        [SerializeField] private SongView songView;
        [SerializeField] private Transform singersParent;
        [SerializeField] private Image singerPrefab;
        [SerializeField] private Button chooseSongButton;
        [SerializeField] private SongUnlockable songUnlockable;

        public event Action<SongData> OnPressed;
        
        [Inject] private CharacterConfig _characterConfig;
        
        private int _songIndex;
        
        public void Init(SongData songData)
        {
            _songIndex = songData.SongIndex;
            songView.Init(songData);
            
            var currentCharacterDatas = _characterConfig.CharacterDatas.Where(cd => songData.Singers.Contains(cd.CharacterName));
            
            foreach (var characterData in currentCharacterDatas)
            {
                var singerImage = Instantiate(singerPrefab, singersParent);
                singerImage.sprite = characterData.BallSprite;
            }
            
            chooseSongButton.onClick.RemoveAllListeners();
            chooseSongButton.onClick.AddListener(() => OnPressed?.Invoke(songData));
            
            songUnlockable.SetSongIndex(_songIndex);
        }
    }
}