using System;
using System.Collections.Generic;
using PT.Logic.Save;
using UnityEngine;

namespace PT.Logic.PlatformRelated.Mobile
{
    public class PrefsSave : ISaveService
    {
        public void Save(SaveData data)
        {
            foreach (var intKvp in data.IntDict) PlayerPrefs.SetInt(intKvp.Key, intKvp.Value);
            foreach (var floatKvp in data.FloatDict) PlayerPrefs.SetFloat(floatKvp.Key, floatKvp.Value);
            foreach (var boolKvp in data.BoolDict) PlayerPrefs.SetInt(boolKvp.Key, boolKvp.Value ? 1 : 0);
            foreach (var strKvp in data.StringDict) PlayerPrefs.SetString(strKvp.Key, strKvp.Value);
            
            PlayerPrefs.Save();
        }
        
        public SaveData Load()
        {
            var intDict = new Dictionary<string, int>();
            var floatDict = new Dictionary<string, float>();
            var boolDict = new Dictionary<string, bool>();
            var stringDict = new Dictionary<string, string>();

            foreach (GameDataKey key in Enum.GetValues(typeof(GameDataKey)))
            {
                var value = GameDataRegistry.Get(key);
                var str = key.ToString();

                if (!PlayerPrefs.HasKey(str)) continue;

                switch (value)
                {
                    case int: intDict[str] = PlayerPrefs.GetInt(str); break;
                    case float: floatDict[str] = PlayerPrefs.GetFloat(str); break;
                    case bool: boolDict[str] = PlayerPrefs.GetInt(str) == 1; break;
                    case Enum: stringDict[str] = PlayerPrefs.GetString(str); break;
                    case string: stringDict[str] = PlayerPrefs.GetString(str); break;
                }
            }

            return new SaveData(intDict, floatDict, boolDict, stringDict);
        }
        private int[] GetArray(string key)
        {
            var intList = new List<int>();

            int i = 0;
            while (true)
            {
                if (!PlayerPrefs.HasKey(key + i.ToString())) break;

                int val = PlayerPrefs.GetInt(key + i.ToString());

                intList.Add(val);
                i++;
            }

            return intList.ToArray();
        }
    }
}
