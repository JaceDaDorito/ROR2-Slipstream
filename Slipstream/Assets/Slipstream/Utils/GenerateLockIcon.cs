using Moonstorm;
using UnityEngine.AddressableAssets;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using On.RoR2;
using On.RoR2.UI;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using RoR2.ExpansionManagement;

namespace Slipstream.Utils
{
    public class ExpansionUtils
    {
        public static ExpansionDef slipexpansion = SlipAssets.Instance.MainAssetBundle.LoadAsset<ExpansionDef>("SlipExpansionDef");
        public void Init()
        {
            //This code runs on startup of the mod to load the lock icon and set the disabled icon to the lock icon
            //Loads SOTV to reference 
            ExpansionDef expansionDef = LegacyResourcesAPI.Load<ExpansionDef>("ExpansionDefs/DLC1");

            slipexpansion.disabledIconSprite = expansionDef.disabledIconSprite;
        }
    }
}