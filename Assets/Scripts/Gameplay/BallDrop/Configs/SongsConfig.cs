using Gameplay.BallDrop.Datas;
using UnityEngine;

namespace Gameplay.BallDrop.Configs
{
    [CreateAssetMenu(menuName = "Configs/SongsConfig", fileName = "SongsConfig")]
    public class SongsConfig : ScriptableObject
    {
        [SerializeField] private SongData[] songDatas;
        
        public SongData[] SongDatas => songDatas;
    }
}