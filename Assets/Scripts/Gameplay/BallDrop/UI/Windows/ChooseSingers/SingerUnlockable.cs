using PT.GameplayAdditional.UI.CurrencyRelated;
using PT.Logic.Save;

namespace Gameplay.BallDrop.UI.Windows.ChooseSingers
{
    public class SingerUnlockable : UnlockableItem
    {
        protected override GameDataKey UnlockStateKey => GameDataKey.UnlockedSingers;
        
        private int _singerIndex;
        protected override int ItemIndex => _singerIndex;
        
        public void SetSingerIndex(int index)
        {
            _singerIndex = index;

            RefreshUnlockState();
        }
    }
}
