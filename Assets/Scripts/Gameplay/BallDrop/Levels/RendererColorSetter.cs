using Gameplay.Session;
using UnityEngine;
using Zenject;

namespace Gameplay.BallDrop.Levels
{
    public class RendererColorSetter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr;

        [Inject] private GameSessionData _gameSessionData;
        
        private void Start()
        {
            sr.color = _gameSessionData.SongData.BgColor;

            enabled = false;
        }
    }
}