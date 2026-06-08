using System;
using Gameplay.BallDrop.Balls;
using UnityEngine;

namespace Gameplay.BallDrop.Datas
{
    [Serializable]
    public class CharacterData
    {
        [SerializeField] private CharacterNameEnum characterName;
        [SerializeField] private Sprite characterSprite;
        [SerializeField] private Sprite ballSprite;
        [Space]
        [SerializeField] private string characterNameKey;
        [SerializeField] private Color characterColor = Color.white;

        public CharacterNameEnum CharacterName => characterName;
        public Sprite CharacterSprite => characterSprite;
        public Sprite BallSprite => ballSprite;

        public string CharacterNameKey => characterNameKey;
        public Color CharacterColor => characterColor;
    }
}