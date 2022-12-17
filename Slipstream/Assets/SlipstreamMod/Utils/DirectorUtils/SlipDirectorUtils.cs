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
using System.Linq;

namespace Slipstream.Scenes
{
    public static class SlipDirectorUtils
    {
        [SystemInitializer(new Type[] { })]
        public static void Init()
        {
            SlipDccs[] allSlipDccs = SlipAssets.LoadAllAssetsOfType<SlipDccs>();
            foreach (SlipDccs slipDccs in allSlipDccs)
            {
                slipDccs.ResolveAddressableCategories();
            }
            SlipFamilyDccs[] allSlipFamilyDccs = SlipAssets.LoadAllAssetsOfType<SlipFamilyDccs>();
            foreach (SlipFamilyDccs slipFamilyDccs in allSlipFamilyDccs)
            {
                slipFamilyDccs.ResolveAddressableCategories();
            }
            SlipDccsPool[] allSlipDccsPools = SlipAssets.LoadAllAssetsOfType<SlipDccsPool>();
            foreach (SlipDccsPool slipDccsPool in allSlipDccsPools)
            {
                slipDccsPool.ResolveAddressableCategories();
            }
        }
        public static void Resolve<T>(this IAddressableKeyProvider<T> provider)
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
    }
}
