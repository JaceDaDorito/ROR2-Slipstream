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
    public class SlipDirectorUtilsHandler
    {
        public void Init()
        {
            SlipDccs[] allSlipDccs = SlipAssets.LoadAllAssetsOfType<SlipDccs>();
            foreach (SlipDccs slipDccs in allSlipDccs)
            {
                slipDccs.ResolveAddressableCategories();
            }
            SlipDccsPool[] allSlipDccsPools = SlipAssets.LoadAllAssetsOfType<SlipDccsPool>();
            foreach (SlipDccsPool slipDccsPool in allSlipDccsPools)
            {
                slipDccsPool.ResolveAddressableCategories();
            }
        }

    }
}
