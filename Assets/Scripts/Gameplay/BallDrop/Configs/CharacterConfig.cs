using Gameplay.BallDrop.Balls;
using Gameplay.BallDrop.Datas;
using PT.Tools.Helper;
using UnityEngine;

namespace Gameplay.BallDrop.Configs
{
    [CreateAssetMenu(menuName = "Configs/CharacterConfig", fileName = "CharacterConfig")]
    public class CharacterConfig : ScriptableObject
    {
        [SerializeField] private CharacterData[] characterDatas;
        
        public CharacterData[] CharacterDatas => characterDatas;
    }
}