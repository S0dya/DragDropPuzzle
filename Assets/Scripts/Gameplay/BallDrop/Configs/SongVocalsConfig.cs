using System;
using System.Linq;
using Gameplay.BallDrop.Balls;
using PT.Logic.Configs;
using PT.Tools.Addressables;
using PT.Tools.Helper;
using UnityEngine;

namespace Gameplay.BallDrop.Configs
{
    [CreateAssetMenu(menuName = "Configs/SongVocalsConfig", fileName = "SongVocalsConfig")]
    public class SongVocalsConfig : ScriptableObject
    {
        [SerializeField] private SerializableKeyValue<SoundEventEnum, CharacterVocalsKvp[]> songVocals;
        
        public AssetKey GetVocal(SoundEventEnum soundEvent, CharacterNameEnum characterName) 
            => songVocals.Dictionary[soundEvent].FirstOrDefault(x => x.Character == characterName).Asset; //myb optimize
    }

    [Serializable]
    class CharacterVocalsKvp
    {
        [SerializeField] private CharacterNameEnum character;
        [SerializeField] private AssetKey asset;

        public CharacterNameEnum Character => character;
        public AssetKey Asset => asset;
    }
}