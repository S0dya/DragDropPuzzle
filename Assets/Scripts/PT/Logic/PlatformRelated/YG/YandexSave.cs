#if UNITY_WEBGL
using System;
using System.Collections.Generic;
using YG;
using PT.Logic.Save;
using UnityEngine;

namespace PT.Logic.PlatformRelated.YG
{
    public class YandexSave : ISaveService
    {
        public void Save(SaveData saveData)
        {
            YG2.saves.IntKeys = new List<string>(saveData.IntDict.Keys);
            YG2.saves.IntValues = new List<int>(saveData.IntDict.Values);

            YG2.saves.FloatKeys = new List<string>(saveData.FloatDict.Keys);
            YG2.saves.FloatValues = new List<float>(saveData.FloatDict.Values);

            YG2.saves.BoolKeys = new List<string>(saveData.BoolDict.Keys);
            YG2.saves.BoolValues = new List<bool>(saveData.BoolDict.Values);

            YG2.saves.StringKeys = new List<string>(saveData.StringDict.Keys);
            YG2.saves.StringValues = new List<string>(saveData.StringDict.Values);

            YG2.SaveProgress();
        }

        public SaveData Load()
        {
            var s = YG2.saves;
            if (s == null || s.IntKeys == null) return null;

            var intDict = new Dictionary<string, int>();
            for (int i = 0; i < s.IntKeys.Count; i++)
                intDict[s.IntKeys[i]] = s.IntValues[i];

            var floatDict = new Dictionary<string, float>();
            for (int i = 0; i < s.FloatKeys.Count; i++)
                floatDict[s.FloatKeys[i]] = s.FloatValues[i];

            var boolDict = new Dictionary<string, bool>();
            for (int i = 0; i < s.BoolKeys.Count; i++)
                boolDict[s.BoolKeys[i]] = s.BoolValues[i];

            var stringDict = new Dictionary<string, string>();
            for (int i = 0; i < s.StringKeys.Count; i++)
                stringDict[s.StringKeys[i]] = s.StringValues[i];

            return new SaveData(intDict, floatDict, boolDict, stringDict);
        }
    }
}
#endif
