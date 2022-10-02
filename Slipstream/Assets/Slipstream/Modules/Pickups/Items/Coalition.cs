using Moonstorm;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using On.RoR2.UI;
using TMPro;
using UnityEngine.UI;


namespace Slipstream.Items
{
    public class Coalition : ItemBase
    {
        private const string token = "SLIP_ITEM_COALITION_DESC";
        public override ItemDef ItemDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<ItemDef>("Coalition");

        [ConfigurableField(ConfigName = "Allies are immunte to void death", ConfigDesc = "Prevents instant void explosion deaths (like the Void Reaver explosion) for allies.", ConfigSection = "Coalition")]
        //[TokenModifier(token, StatTypes.Default, 0)]
        public static bool preventsVoidDeath = true;

        [ConfigurableField(ConfigName = "Initial health threshold/Hyperbolic Scaling", ConfigDesc = "Initial health threshold percentage at one stack and hyperbolic staling", ConfigSection = "Coalition")]
        [TokenModifier(token, StatTypes.Percentage, 0)]
        public static float amplificationPercentage = 0.25f;

        [ConfigurableField(ConfigName = "Armor increase per stack", ConfigDesc = "Armor increase applied to allies per stack", ConfigSection = "Coalition")]
        [TokenModifier(token, StatTypes.Default, 1)]
        public static float armorIncrease = 100f;

        public class CoalitionBehavior : BaseItemBodyBehavior
        {
            [ItemDefAssociation(useOnClient = false, useOnServer = true)]
            public static ItemDef GetItemDef() => SlipContent.Items.Coalition;

            GameObject destroyEffectPrefab;

            private void Start()
            {
                destroyEffectPrefab = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("CoalitionPreDetonation");

                //Adds Blackhealth item to all your allies
                CharacterMaster master = body.master;
                if (master)
                {
                    foreach (CharacterMaster characterMaster in CharacterMaster.instancesList)
                    {
                        if (characterMaster.minionOwnership && characterMaster.minionOwnership.ownerMaster == master)
                        {
                            characterMaster.inventory.GiveItem(SlipContent.Items.BlackHealth);
                        }
                    }
                }
            }

            private void OnDestroy()
            {
                //When you get rid of Coalition, get rid of Blackhealth from all allies
                CharacterMaster master = body.master;
                if (master)
                {
                    foreach (CharacterMaster characterMaster in CharacterMaster.instancesList)
                    {
                        if (characterMaster.minionOwnership && characterMaster.minionOwnership.ownerMaster == master)
                        {
                            characterMaster.inventory.ResetItem(SlipContent.Items.BlackHealth);
                        }
                    }
                }
            }

            private void OnEnable()
            {
                MasterSummon.onServerMasterSummonGlobal += MasterSummon_onServerMasterSummonGlobal;
                On.RoR2.HealthComponent.UpdateLastHitTime += HealthComponent_UpdateLastHitTime;
            }

            private void OnDisable()
            {
                MasterSummon.onServerMasterSummonGlobal -= MasterSummon_onServerMasterSummonGlobal;
                On.RoR2.HealthComponent.UpdateLastHitTime -= HealthComponent_UpdateLastHitTime;
            }
            private void HealthComponent_UpdateLastHitTime(On.RoR2.HealthComponent.orig_UpdateLastHitTime orig, HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker)
            {
                //call orig to make sure that Elixr is activated first. This only matters at 1 stack though.
                orig(self, damageValue, damagePosition, damageIsSilent, attacker);

                //Kill all allies when player is below the current threshold.
                float threshold = Moonstorm.MSUtil.InverseHyperbolicScaling(amplificationPercentage, amplificationPercentage, 1, stack);
                if (body == self.body && (body.healthComponent.health + body.healthComponent.shield)/body.healthComponent.fullCombinedHealth < threshold) //Character body check and ratio of curent health compared to the threshold
                {
                    CharacterMaster master = body.master;
                    foreach (CharacterMaster characterMaster in CharacterMaster.instancesList)
                    {
                        if (characterMaster.minionOwnership && characterMaster.minionOwnership.ownerMaster == master)
                        {
                            CharacterBody allybody = characterMaster.GetBody();
                            EffectData effectData = new EffectData { origin = allybody.corePosition, scale = allybody.radius};

                            EffectManager.SpawnEffect(destroyEffectPrefab, effectData, true);
                            characterMaster.GetBody()?.healthComponent.Suicide();
                        }
                    }
                }
            }

            private void MasterSummon_onServerMasterSummonGlobal(MasterSummon.MasterSummonReport report)
            {
                //If allies are gained while having Coalition, give them Blackhealth
                if (body.master && body.master == report.leaderMasterInstance)
                {
                    CharacterMaster summonMasterInstance = report.summonMasterInstance;
                    if (summonMasterInstance)
                    {
                        summonMasterInstance.inventory.GiveItem(SlipContent.Items.BlackHealth);
                    }
                }
            }

        }
    }
}