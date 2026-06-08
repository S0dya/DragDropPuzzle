using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PT.Tools.Debugging;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace PT.Tools.Addressables
{
    public class AddressablesAssetsService : IAssetProvider, IAssetResolver
    {
        private readonly Dictionary<AssetKey, AsyncOperationHandle> _handles = new();

        public async UniTask Load<T>(AssetKey key) where T : Object
        {
            if (_handles.ContainsKey(key)) return;
            
            DebugManager.Log(DebugCategory.Addressables, $"Loading {key.ToString()} ...");
            
            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(key.ToString());
            await handle.ToUniTask();
            
            _handles[key] = handle;
            
            if (handle.Status != AsyncOperationStatus.Succeeded)
                DebugManager.Log(DebugCategory.Addressables, $"Failed loading {key}", LogType.Error);
        }

        public void Release(AssetKey key)
        {
            if (_handles.TryGetValue(key, out var handle))
            {
                DebugManager.Log(DebugCategory.Addressables, $"Releasing addressable: {key}");
                
                UnityEngine.AddressableAssets.Addressables.Release(handle);
                _handles.Remove(key);
            }
            else
            {
                DebugManager.Log(DebugCategory.Addressables, $"Trying to release unknown addressable: {key}", LogType.Error);
            }
        }

        public T Get<T>(AssetKey key) where T : Object
        {
            if (!_handles.TryGetValue(key, out var handle))
                throw new Exception($"Asset {key} not loaded");

            return handle.Result as T;
        }
        
        public IReadOnlyDictionary<AssetKey, AsyncOperationHandle> GetLoadedHandles() => _handles;
    }
}