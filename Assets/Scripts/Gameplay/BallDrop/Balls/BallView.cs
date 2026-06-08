using System;
using Gameplay.BallDrop.Datas;
using UnityEngine;

namespace Gameplay.BallDrop.Balls
{
    [Serializable]
    public class BallView
    {
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private TrailRenderer tr;
        
        public void SetInfo(CharacterData characterData)
        {
            sr.sprite = characterData.BallSprite;
        }

        public void ChangeSize(float size)
        {
            tr.startWidth = size;
        }
    }
}