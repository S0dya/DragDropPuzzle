using UniRx;

namespace PT.Logic.Ads
{
    public interface IBannerService
    {
        public void ShowBanner();
        public void HideBanner();
        public ReactiveProperty<bool> IsBannerActive { get; }
    }
}