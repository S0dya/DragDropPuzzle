using Gameplay.BallDrop.Balls;
using Gameplay.BallDrop.Datas;
using Gameplay.BallDrop.Levels.Types;
using PT.Logic.Configs;
using UnityEngine;

namespace Gameplay.Session
{
    public class GameSessionData
    {
        public int SelectedLevelIndex { get; private set; }
        public LevelLengthType LevelLengthType { get; private set; }
        public SongData SongData { get; private set; }
        public CharacterData[] CharacterDatas { get; private set; }
        
        public void SetInfo(int selectedLevelIndex, SongData songData, CharacterData[] currentCharacterDatas)
        {
            SelectedLevelIndex = selectedLevelIndex;
            SongData = songData;
            CharacterDatas = currentCharacterDatas;
        }
    }
}