using Moonstorm;
using UnityEngine.AddressableAssets;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Slipstream.Scenes
{
    public static class IAddressableKeyProviderExt
    {
        public static void Resolve<T>(this IAddressableKeyProvider<T> provider)
        {
            if(provider is IAddressableKeyArrayProvider<T> arrayProvider)
            {
                foreach (string key in arrayProvider.Key)
                {
                    ResolveSingle(arrayProvider, key);
                }
                return;
            }
            ResolveSingle(provider, provider.Key);
        }
        public static void ResolveSingle<T>(IAddressableKeyProvider<T> provider, string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                T addressable = Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
                if (addressable == null)
                {
                    SlipLogger.LogW(provider + ": Addressable key [" + key + "] was provided, but returned null!");
                }
                provider.Addressable = addressable;
            }
            
        }
    }
    public interface IAddressableKeyProvider<T>
    {
        string Key { get; }
        T Addressable { set; }
        
    }
    public interface IAddressableKeyArrayProvider<T> : IAddressableKeyProvider<T>
    {
        new string[] Key { get; }
    }

}
