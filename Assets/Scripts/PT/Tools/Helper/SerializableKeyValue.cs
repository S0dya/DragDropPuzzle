using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PT.Tools.Debugging;
using UnityEngine;

namespace PT.Tools.Helper
{
    [Serializable]
    public class SerializableKeyValue<TKey, TValue>
    {
        [Header("Array")]
        [JsonProperty] [SerializeField] private List<TKey> keys = new();
        [JsonProperty] [SerializeField] private List<TValue> values = new();
        [Space]
        [Header("Or Pair")]
        [SerializeField] private KeyValueClass<TKey, TValue>[] keyValueClasses;

        [Serializable]
        class KeyValueClass<TK, TV>
        {
            [JsonProperty] [SerializeField] private TK key;
            [JsonProperty] [SerializeField] private TV value;
            
            public TK Key => key;
            public TV Value => value;
        }
        
        public List<TKey> Keys => keys;
        public List<TValue> Values => values;
        
        private Dictionary<TKey, TValue> _dictionary;
        public Dictionary<TKey, TValue> Dictionary 
        {
            get
            {
                if (_dictionary == null)
                {
                    _dictionary = new Dictionary<TKey, TValue>();

                    if (keys.Count >= 0 && keys.Count > keyValueClasses.Length)
                    {
                        if (keys.Count != values.Count) throw new Exception("Keys and Values lists must have the same number of elements.");
                        
                        for (int i = 0; i < keys.Count; i++) _dictionary[keys[i]] = values[i];
                    }
                    else if (keyValueClasses != null && keyValueClasses.Length > 0)
                    {
                        for (int i = 0; i < keyValueClasses.Length; i++)
                        {
                            _dictionary[keyValueClasses[i].Key] = keyValueClasses[i].Value;
                        }
                    }
                    else DebugManager.Log(DebugCategory.Misc, $"Empty SerializableKeyValue<{typeof(TKey).Name}, {typeof(TValue).Name}>", LogType.Warning);
                    
                }
            
                return _dictionary;
            }
        }

        public SerializableKeyValue(){}
        public SerializableKeyValue(List<TKey> key, List<TValue> value)
        {
            keys = key;
            values = value;
        }
    }
}