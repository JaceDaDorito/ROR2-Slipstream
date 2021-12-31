using Moonstorm;
using Slipstream.Buffs;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using System;

namespace Slipstream.Items
{
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
        public static float stackCritDmg = 0.2f;

        public override void AddBehavior(ref CharacterBody body, int stack)
        {
            //SlipLogger.LogD($"Initializing Jace Hat");
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

                    //Checks if shield is active and if the damage dealt is a crit (whether it be procced or a backstab), sorry if this if statement is a mess
                    if (damageSource != null && damageSource.healthComponent.shield > 0 && (damageInfo.crit || BackstabManager.IsBackstab(-(damageSource.corePosition - damageInfo.position), body)))
                    {
                        damageInfo.damage *= (float)(1 + (stackCritDmg * stack));
                    }
                }
            }
        }
    }
}
