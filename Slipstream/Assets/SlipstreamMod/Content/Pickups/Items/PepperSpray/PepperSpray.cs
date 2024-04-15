using MSU;
using MSU.Config;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using TMPro;
using UnityEngine.UI;
using RoR2.ContentManagement;
using System.Collections;

namespace Slipstream.Items
{
    public class PepperSpray: SlipItem
    {
        //Probably look at GlassEye.cs for your first reference of an item

        private const string TOKEN = "SLIP_ITEM_PEPPERSPRAY_DESC";
        public override RoR2.ItemDef ItemDef => _itemDef;
        private ItemDef _itemDef;
        public override NullableRef<GameObject> ItemDisplayPrefab => _itemDisplayPrefab;
        private GameObject _itemDisplayPrefab;

        //public static string section;

        //Establishes the config fields to allow easy changes in values in certain calculations and such.

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Base Shield", ConfigDescOverride = "Shield percentage after having at least one stack.", ConfigSectionOverride = "PepperSpray")]
        [FormatToken(SlipConfig.ITEMS, FormatTokenAttribute.OperationTypeEnum.MultiplyByN, 100)]
        public static float baseShield = 0.08f;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Shield Threshold", ConfigDescOverride = "Percentage of total shield in order to trigger the effect.", ConfigSectionOverride = "PepperSpray")]
        [FormatToken(SlipConfig.ITEMS, FormatTokenAttribute.OperationTypeEnum.MultiplyByN, 100)]
        public static float threshold = SlipCriticalShield.threshold;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Base Radius", ConfigDescOverride = "Initial radius of the stun effect.", ConfigSectionOverride = "PepperSpray")]
        public static float baseRadius = 13.0f;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Radius Increase", ConfigDescOverride = "Amount of increased stun radius per stack.", ConfigSectionOverride = "PepperSpray")]
        public static float radiusPerStack = 6.0f;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Speed Increase", ConfigDescOverride = "Movement speed increase when Pepper Speed is active.", ConfigSectionOverride = "PepperSpray")]
        [FormatToken(SlipConfig.ITEMS, FormatTokenAttribute.OperationTypeEnum.MultiplyByN, 100)]
        public static float speedIncrease = 0.6f;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Max Speed Duration", ConfigDescOverride = "The time on your buff if your entire healthbar is shield + Base Speed Duration Constant.", ConfigSectionOverride = "PepperSpray")]
        public static float maxBuffTime = 20.0f;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Base Speed Duration Constant", ConfigDescOverride = "Initial amount of speed with one stack.", ConfigSectionOverride = "PepperSpray")]
        public static float buffTimeConstant = 1.0f;

        public override void Initialize()
        {
            Slipstream.Items.SlipCriticalShield.critShieldItems.Add(ItemDef);
        }

        public override bool IsAvailable(ContentPack contentPack)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator LoadContentAsync()
        {
            throw new NotImplementedException();
        }

        public class PepperSprayBehavior : BaseItemBodyBehavior, IBodyStatArgModifier, SlipCriticalShield.ICriticalShield
        {
            [ItemDefAssociation(useOnClient = true, useOnServer = true)]
            public static RoR2.ItemDef GetItemDef() => SlipContent.Items.PepperSpray;

            public static GameObject explosionEffect = SlipAssets.LoadAsset<GameObject>("PepperSprayExplosion", SlipBundle.Items);

            private Image image;
            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                args.baseShieldAdd += body.healthComponent.fullHealth * baseShield;
            }
            public void Trigger()
            {
                if (NetworkServer.active)
                {
                    FireStunSpray();
                    body.AddTimedBuff(SlipContent.Buffs.PepperSpeed.buffIndex, maxBuffTime * (body.healthComponent.fullShield / (body.healthComponent.fullShield + body.healthComponent.fullHealth)) + buffTimeConstant);
                }
                RoR2.Util.PlaySound("Play_PepperSpray_SFX", gameObject);
            }

            private void FireStunSpray()
            {
                //Establishes a gameobject for the explosion
                Vector3 corePosition = RoR2.Util.GetCorePosition(body.gameObject);
                float radius = body.radius + baseRadius + radiusPerStack * (stack - 1f);
                GameObject hitBoxStun = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/GenericDelayBlast"), corePosition, Quaternion.identity);
                hitBoxStun.transform.localScale = new Vector3(radius, radius, radius);
                RoR2.DelayBlast sprayAttack = hitBoxStun.GetComponent<RoR2.DelayBlast>();

                //Fills characteristics of the explosion such as the damage type stunning
                sprayAttack.position = corePosition;
                sprayAttack.radius = radius;
                sprayAttack.attacker = body.gameObject;
                sprayAttack.falloffModel = RoR2.BlastAttack.FalloffModel.None;
                sprayAttack.maxTimer = 0f;
                sprayAttack.damageType = DamageType.Stun1s;
                sprayAttack.explosionEffect = explosionEffect;
                //sprayAttack.delayEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/AffixWhiteExplosion");
                hitBoxStun.GetComponent<RoR2.TeamFilter>().teamIndex = RoR2.TeamComponent.GetObjectTeam(body.gameObject);

                //Fires the explosion
                NetworkServer.Spawn(hitBoxStun);
            }

            
        }
    }
}
