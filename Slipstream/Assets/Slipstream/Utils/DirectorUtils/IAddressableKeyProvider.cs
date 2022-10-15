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
            if (provider is IBasicAddressableKeyProvider<T> basicProvider)
            {
                ResolveBasic(basicProvider);
            }
            if (provider is IArrayAddressableKeyProvider<T> arrayProvider)
            {
                ResolveArray(arrayProvider);
            }
        }
        public static void ResolveBasic<T>(IBasicAddressableKeyProvider<T> provider)
        {
            if (!string.IsNullOrEmpty(provider.Key))
            {
                T addressable = Addressables.LoadAssetAsync<T>(provider.Key).WaitForCompletion();
                if (addressable == null)
                {
                    SlipLogger.LogW(provider + ": Addressable key [" + provider.Key + "] was provided, but returned null!");
                }
                provider.Addressable = addressable;
            }
            
        }
        public static void ResolveArray<T>(IArrayAddressableKeyProvider<T> provider)
        {
            SlipLogger.LogI("resolve addressable keys array:");
            foreach (string key in provider.Keys)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    SlipLogger.LogI("found key: " + key);
                    T addressable = Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
                    if (addressable == null)
                    {
                        SlipLogger.LogW(provider + ": Addressable key [" + key + "] was provided, but returned null!");
                    }
                    provider.AppendAddressable(addressable);
                }
            }

        }
    }
    public interface IAddressableKeyProvider<T> { }
    public interface IBasicAddressableKeyProvider<T> : IAddressableKeyProvider<T>
    {
        string Key { get; }
        T Addressable { set; }
        
    }
    public interface IArrayAddressableKeyProvider<T> : IAddressableKeyProvider<T>
    {
        string[] Keys { get; }
        void AppendAddressable(T addressable);
    }

}
