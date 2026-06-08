using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.BallDrop.Configs;
using Gameplay.BallDrop.Datas;
using Gameplay.BallDrop.UI.Windows.ChooseSingers;
using PT.Tools.Factories;
using PT.Tools.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.BallDrop.UI.Windows.ChooseSong
{
    public class ChooseSongWindow : MonoBehaviour
    {
        [SerializeField] private SongCardView songCardPrefab;
        [SerializeField] private Transform songCardsParent;
        [Space]
        [SerializeField] private Button closeButton;

        [Inject] private SongsConfig _songsConfig;
        [Inject] private IFactoryZenject _factoryZenject;
        [Inject(Id = "Menu")] private WindowsManager _windowsManager;

        private readonly List<SongCardView> _songCards = new();
        
        private void Awake()
        {
            foreach (var song in _songsConfig.SongDatas)
            {
                var card = _factoryZenject.InstantiateObject(songCardPrefab.gameObject, songCardsParent).GetComponent<SongCardView>();
                card.Init(song);
                card.OnPressed += OnSongChosen;
                
                _songCards.Add(card);
            }
            
            closeButton.onClick.AddListener(OnClosePressed);
        }

        private void OnSongChosen(SongData songData)
        {
            _windowsManager.Open<ChooseSingersWindow>(songData).Forget();
        }
        private void OnClosePressed()
        {
            // _windowsManager.Close<ChooseSongWindow>().Forget();
        }

        private void OnDestroy()
        {
            foreach (var card in _songCards) card.OnPressed -= OnSongChosen;
        }
    }
}