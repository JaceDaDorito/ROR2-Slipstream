using MSU;
using MSU.Config;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using RoR2.ContentManagement;
using System.Collections;

namespace Slipstream.Items
{
    //Reminder to implement the new critdmg stat instead of this
    public class GlassEye : SlipItem
    {
        private const string TOKEN = "SLIP_ITEM_GLASSEYE_DESC";
        public override ItemDef ItemDef { get; } = SlipAssets.LoadAsset<ItemDef>("GlassEye", SlipBundle.Items);

        private ItemDef _itemDef;

        public override NullableRef<GameObject> ItemDisplayPrefab => throw new NotImplementedException();

        private GameObject _itemDisplayPrefab;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Base Shield", ConfigDescOverride = "Shield percentage after having at least one stack.", ConfigSectionOverride = "GlassEye")]
        [FormatToken(TOKEN, FormatTokenAttribute.OperationTypeEnum.MultiplyByN, 100)]
        public static float baseShield = 0.06f;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Base Crit", ConfigDescOverride = "Crit chance given when having at least one stack.", ConfigSectionOverride = "GlassEye")]
        public static float baseCrit = 5;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Initial Crit Dmg", ConfigDescOverride = "Initial crit dmg on first stack.", ConfigSectionOverride = "GlassEye")]
        [FormatToken(TOKEN, FormatTokenAttribute.OperationTypeEnum.MultiplyByN, 100)]
        public static float initialCritDmg = 0.2f;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Crit Dmg per Stack", ConfigDescOverride = "Increased crit damage per item stack.", ConfigSectionOverride = "GlassEye")]
        [FormatToken(TOKEN, FormatTokenAttribute.OperationTypeEnum.MultiplyByN, 100)]
        public static float stackCritDmg = 0.1f;

        public override void Initialize()
        {
        }

        public override bool IsAvailable(ContentPack contentPack)
        {
            return true;
        }

        public override IEnumerator LoadContentAsync()
        {
            var request = SlipAssets.LoadAssetAsync<AssetCollection>("acChargedFungus", SlipBundle.Items);

            request.StartLoad();
            while (!request.IsComplete)
                yield return null;

            var collection = request.Asset;

            _itemDef = collection.FindAsset<ItemDef>("ChargedFungus");
            _itemDisplayPrefab = collection.FindAsset<GameObject>("DisplayGlassEye");
            yield break;
        }

        public class GlassEyeBehavior : BaseItemBodyBehavior, IBodyStatArgModifier
        {
            [ItemDefAssociation(useOnClient = true, useOnServer = true)]
            public static ItemDef GetItemDef() => SlipContent.Items.GlassEye;

            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                args.baseShieldAdd += body.healthComponent.fullHealth * baseShield;
                args.critAdd += baseCrit;

                //lmao, thanks SoTV
                if(body.healthComponent.shield > 0)
                {
                    args.critDamageMultAdd += (float)(stackCritDmg * (stack - 1) + initialCritDmg);
                }
            }
        }
    }
}
