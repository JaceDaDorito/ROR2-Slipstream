using Moonstorm;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using System;

namespace Slipstream.Items
{
    [DisabledContent]
    public class GlassEye : ItemBase
    {
        private const string token = "SLIP_ITEM_GLASSEYE_DESC";
        public override ItemDef ItemDef { get; set; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<ItemDef>("GlassEye");

        public static string section;

        [ConfigurableField(ConfigName = "Base Shield", ConfigDesc = "Shield percentage after having at least one stack.")]
        //[TokenModifier(token, StatTypes.Default, 0)]
        public static float baseShield = 0.05f;

        [ConfigurableField(ConfigName = "Base Crit", ConfigDesc = "Crit chance given when having at least one stack.")]
        //[TokenModifier(token, StatTypes.Default, 0)]
        public static float baseCrit = 5;

        [ConfigurableField(ConfigName = "Crit Dmg per Stack", ConfigDesc = "Increased crit damage per item stack.")]
        //[TokenModifier(token, StatTypes.Default, 0)]
        public static float stackCritDmg = 0.05f;

        //Color SuperCrit = new Color(0.495194f, 0.5953774f, 0.9811321f); Color for later, just dont know how to do it yet

        public override void AddBehavior(ref CharacterBody body, int stack)
        {
            //SlipLogger.LogD($"Initializing Glass Eye");
            body.AddItemBehavior<GlassEyeBehavior>(stack);
        }

        public class GlassEyeBehavior : CharacterBody.ItemBehavior, IBodyStatArgModifier, IOnIncomingDamageOtherServerReciever
        {
            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                args.baseShieldAdd += body.healthComponent.fullHealth * baseShield;
                args.critAdd += baseCrit;
            }

            public void OnIncomingDamageOther(HealthComponent victimHealthComponent, DamageInfo damageInfo)
            {
                if (NetworkServer.active)
                {
                    CharacterBody damageSource;
                    if (damageInfo.attacker != null)
                    {
                        damageSource = damageInfo.attacker.GetComponent<CharacterBody>();
                    }
                    else
                    {
                        return;
                    }

                    //Checks if shield is active and if the damage dealt is a crit (whether it be procced or a backstab)
                    //Well this crit check does not work at all. I think crit damage becomes a stat when DLC comes out so I think its best to wait
                    
                    if (damageSource != null && damageSource.healthComponent.shield > 0)
                    {
                        if (damageInfo.crit /*|| BackstabManager.IsBackstab(-(damageSource.corePosition - damageInfo.position), body)*/)
                        {
                            damageInfo.damage *= (float)(1 + (stackCritDmg * stack));
                            damageInfo.damageColorIndex = SlipDmgColorCatalog.loadedIndices[0];
                        }
                    }
                }
            }
        }
    }
}
