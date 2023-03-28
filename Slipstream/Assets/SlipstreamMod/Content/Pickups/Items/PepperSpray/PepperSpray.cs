using Moonstorm;
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

namespace Slipstream.Items
{
    public class PepperSpray: ItemBase
    {
        //Probably look at GlassEye.cs for your first reference of an item

        private const string token = "SLIP_ITEM_PEPPERSPRAY_DESC";
        public override RoR2.ItemDef ItemDef { get; } = SlipAssets.LoadAsset<RoR2.ItemDef>("PepperSpray", SlipBundle.Items);

        //public static string section;

        //Establishes the config fields to allow easy changes in values in certain calculations and such.

        [ConfigurableField(ConfigName = "Base Shield", ConfigDesc = "Shield percentage after having at least one stack.", ConfigSection = "PepperSpray")]
        [TokenModifier(token, StatTypes.MultiplyByN, 0, "100")]
        public static float baseShield = 0.08f;

        //[ConfigurableField(ConfigName = "Shield Threshold", ConfigDesc = "Percentage of total shield in order to trigger the effect.", ConfigSection = "PepperSpray")]
        [TokenModifier(token, StatTypes.MultiplyByN, 1, "100")]
        public static float threshold = SlipCriticalShield.threshold;

        [ConfigurableField(ConfigName = "Base Radius", ConfigDesc = "Initial radius of the stun effect.", ConfigSection = "PepperSpray")]
        [TokenModifier(token, StatTypes.Default, 2)]
        public static float baseRadius = 13.0f;

        [ConfigurableField(ConfigName = "Radius Increase", ConfigDesc = "Amount of increased stun radius per stack.", ConfigSection = "PepperSpray")]
        [TokenModifier(token, StatTypes.Default, 3)]
        public static float radiusPerStack = 6.0f;

        [ConfigurableField(ConfigName = "Speed Increase", ConfigDesc = "Movement speed increase when Pepper Speed is active.", ConfigSection = "PepperSpray")]
        [TokenModifier(token, StatTypes.MultiplyByN, 4, "100")]
        public static float speedIncrease = 0.6f;

        [ConfigurableField(ConfigName = "Max Speed Duration", ConfigDesc = "The time on your buff if your entire healthbar is shield + Base Speed Duration Constant.", ConfigSection = "PepperSpray")]
        [TokenModifier(token, StatTypes.Default, 5)]
        public static float maxBuffTime = 20.0f;

        [ConfigurableField(ConfigName = "Base Speed Duration Constant", ConfigDesc = "Initial amount of speed with one stack.", ConfigSection = "PepperSpray")]
        //[TokenModifier(token, StatTypes.Default, 5)]
        public static float buffTimeConstant = 1.0f;

        //public static string explosionSoundString = "Fart";

        public override void Initialize()
        {
            Slipstream.Items.SlipCriticalShield.critShieldItems.Add(ItemDef);
        }

        public class PepperSprayBehavior : BaseItemBodyBehavior, IBodyStatArgModifier, SlipCriticalShield.ICriticalShield
        {
            [ItemDefAssociation(useOnClient = true, useOnServer = true)]
            public static RoR2.ItemDef GetItemDef() => SlipContent.Items.PepperSpray;

            private Image image;

            //private bool shouldTrigger = false;

            //This just adds an initial shield when you have atleast one stack.
            /*public void Awake()
            {
                body.RecalculateStats();
            }*/
            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                args.baseShieldAdd += body.healthComponent.fullHealth * baseShield;
            }

            //The trigger should only happen once until you recharge, not after everytime you get hit below the threshold
            public void Trigger()
            {
                if (NetworkServer.active)
                {
                    FireStunSpray();
                    body.AddTimedBuff(SlipContent.Buffs.PepperSpeed.buffIndex, maxBuffTime * (body.healthComponent.fullShield / (body.healthComponent.fullShield + body.healthComponent.fullHealth)) + buffTimeConstant);
                }
                //Util.PlaySound(explosionSoundString, gameObject);
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
                sprayAttack.explosionEffect = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("PepperSprayExplosion");
                //sprayAttack.delayEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/AffixWhiteExplosion");
                hitBoxStun.GetComponent<RoR2.TeamFilter>().teamIndex = RoR2.TeamComponent.GetObjectTeam(body.gameObject);

                //Fires the explosion
                NetworkServer.Spawn(hitBoxStun);
            }

            
        }
    }
}
