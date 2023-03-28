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

namespace Slipstream.Items
{
    public class BrineSwarm : VoidItemBase
    {
        private const string token = "SLIP_ITEM_BRINESWARM_DESC";
        public override RoR2.ItemDef ItemDef { get; } = SlipAssets.LoadAsset<RoR2.ItemDef>("BrineSwarm", SlipBundle.Items);
        

        [ConfigurableField(ConfigName = "Base Shield", ConfigDesc = "Shield percentage after having at least one stack.", ConfigSection = "BrineSwarm")]
        [TokenModifier(token, StatTypes.MultiplyByN, 0, "100")]
        public static float baseShield = 0.08f;

        [TokenModifier(token, StatTypes.MultiplyByN, 1, "100")]
        public static float threshold = SlipCriticalShield.threshold;

        [ConfigurableField(ConfigName = "Damage Increase", ConfigDesc = "Damage increase when Brine Buff is active.", ConfigSection = "BrineSwarm")]
        [TokenModifier(token, StatTypes.MultiplyByN, 2, "100")]
        public static float damageIncrease = 0.2f;

        [ConfigurableField(ConfigName = "Armor Increase", ConfigDesc = "Damage increase when Brine Buff is active.", ConfigSection = "BrineSwarm")]
        [TokenModifier(token, StatTypes.Default, 3)]
        public static float armorIncrease = 100f;

        [ConfigurableField(ConfigName = "Max Buff Duration", ConfigDesc = "The time on your buff if your entire healthbar is shield + Base Buff Duration Constant.", ConfigSection = "BrineSwarm")]
        [TokenModifier(token, StatTypes.Default, 4)]
        public static float maxBuffTime = 20.0f;

        [ConfigurableField(ConfigName = "Base Buff Duration Constant", ConfigDesc = "Initial amount of speed with one stack.", ConfigSection = "BrineSwarm")]
        //[TokenModifier(token, StatTypes.Default, 5)]
        public static float buffTimeConstant = 1.0f;

        public override void Initialize()
        {
            base.Initialize();
            VoidShieldCatalog.AddVoidShieldCatalog(ItemDef);
            SlipCriticalShield.critShieldItems.Add(ItemDef);
        }

        public override IEnumerable<RoR2.ItemDef> LoadItemsToInfect()
        {
            return new RoR2.ItemDef[1] { SlipAssets.LoadAsset<RoR2.ItemDef>("PepperSpray", SlipBundle.Items) };
        }

        public class BrineSwarmBehavior : BaseItemBodyBehavior, IBodyStatArgModifier, SlipCriticalShield.ICriticalShield
        {
            [ItemDefAssociation(useOnClient = true, useOnServer = true)]

            public static RoR2.ItemDef GetItemDef() => SlipContent.Items.BrineSwarm;

            /*public void Awake()
            {
                body.RecalculateStats();
            }*/

            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                args.baseShieldAdd += body.healthComponent.fullHealth * baseShield;
            }

            public void Trigger()
            {
                if (NetworkServer.active)
                {
                    //obviously the calculation should be different, also didnt balance the values at all
                    body.AddTimedBuff(SlipContent.Buffs.BrineBuff.buffIndex, maxBuffTime * (body.healthComponent.fullShield / (body.healthComponent.fullShield + body.healthComponent.fullHealth)) + buffTimeConstant);
                }
                //Util.PlaySound(explosionSoundString, gameObject);
                RoR2.Util.PlaySound(EntityStates.BrotherMonster.WeaponSlam.attackSoundString, gameObject);
            }
        }
    }
}
