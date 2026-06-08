using System;
using Gameplay.BallDrop.Balls;
using PT.Logic.Configs;
using UnityEngine;

namespace Gameplay.BallDrop.Datas
{
    [Serializable]
    public class SongData
    {
        [SerializeField] private SoundEventEnum musicName;
        [SerializeField] private Sprite coverImage;
        [SerializeField] private string name;
        [SerializeField][TextArea] private string description;
        [SerializeField] private CharacterNameEnum[] singers;
        [SerializeField] private int songIndex;
        [Space]
        [SerializeField] private Color bgColor;
        // [SerializeField] private BackgroundType bgType;
        
        public SoundEventEnum MusicName => musicName;
        public Sprite CoverImage => coverImage;
        public string Name => name;
        public string Description => description;
        public CharacterNameEnum[] Singers => singers;
        public int SongIndex => songIndex;
        
        public Color BgColor => bgColor;
        // public BackgroundType BgType => bgType;
    }
}