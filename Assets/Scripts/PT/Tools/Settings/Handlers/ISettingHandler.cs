namespace PT.Tools.Settings.Handlers
{
    public interface ISettingHandler
    {
        void Load();
        void Save();
        void Apply();
    }
}