using System.Collections.Generic;

namespace YG
{
    [System.Serializable]
    public partial class SavesYG
    {
        public int idSave;
        
        public List<string> IntKeys = new();
        public List<int> IntValues = new();

        public List<string> FloatKeys = new();
        public List<float> FloatValues = new();

        public List<string> BoolKeys = new();
        public List<bool> BoolValues = new();

        public List<string> StringKeys = new();
        public List<string> StringValues = new();
    }
}
