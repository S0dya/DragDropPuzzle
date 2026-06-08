using PT.GameplayAdditional.UI.CurrencyRelated;
using PT.Logic.Save;

namespace Gameplay.BallDrop.UI.Windows.ChooseSong
{
    public class SongUnlockable : UnlockableItem
    {
        protected override GameDataKey UnlockStateKey => GameDataKey.UnlockedSongs;
        
        private int _songIndex;
        protected override int ItemIndex => _songIndex;
        
        public void SetSongIndex(int index)
        {
            _songIndex = index;
            
            RefreshUnlockState();
        }
    }
}
