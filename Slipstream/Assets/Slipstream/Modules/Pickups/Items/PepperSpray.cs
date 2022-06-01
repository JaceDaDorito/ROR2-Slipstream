using Moonstorm;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;

namespace Slipstream.Items
{
    public class PepperSpray: ItemBase
    {
        //Ok so this isn't the most ideal item to reference off of as a beginner but don't be intimidated by the sheer girth of the code here.

        private const string token = "SLIP_ITEM_PEPPERSPRAY_DESC";
        public override ItemDef ItemDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<ItemDef>("PepperSpray");

        public static string section;

        //Establishes the config fields to allow easy changes in values in certain calculations and such.

        [ConfigurableField(ConfigName = "Base Shield", ConfigDesc = "Shield percentage after having at least one stack.")]
        [TokenModifier(token, StatTypes.Percentage, 0)]
        public static float baseShield = 0.06f;

        [ConfigurableField(ConfigName = "Shield Threshold", ConfigDesc = "Percentage of total shield in order to trigger the effect.")]
        [TokenModifier(token, StatTypes.Percentage, 1)]
        public static float threshold = 0.5f;

        [ConfigurableField(ConfigName = "Base Radius", ConfigDesc = "Initial radius of the stun effect.")]
        [TokenModifier(token, StatTypes.Default, 2)]
        public static float baseRadius = 13.0f;

        [ConfigurableField(ConfigName = "Radius Increase", ConfigDesc = "Amount of increased stun radius per stack.")]
        [TokenModifier(token, StatTypes.Default, 3)]
        public static float radiusPerStack = 6.0f;

        [ConfigurableField(ConfigName = "Speed Increase", ConfigDesc = "Movement speed increase when Pepper Speed is active.")]
        [TokenModifier(token, StatTypes.Percentage, 4)]
        public static float speedIncrease = 0.6f;

        [ConfigurableField(ConfigName = "Max Speed Duration", ConfigDesc = "The time on your buff if your entire healthbar is shield + Base Speed Duration Constant.")]
        [TokenModifier(token, StatTypes.Default, 5)]
        public static float maxBuffTime = 20.0f;

        //[ConfigurableField(ConfigName = "Base Speed Duration Constant", ConfigDesc = "Initial amount of speed with one stack.")]
        //[TokenModifier(token, StatTypes.Default, 5)]
        public static float buffTimeConstant = 1.0f;

        //public static string explosionSoundString = "Fart";

        public class PepperSprayBehavior : BaseItemBodyBehavior, IBodyStatArgModifier
        {
            [ItemDefAssociation(useOnClient = true, useOnServer = true)]
            public static ItemDef GetItemDef() => SlipContent.Items.PepperSpray;

            private bool shouldTrigger = false;

            //This just adds an initial shield when you have atleast one stack.
            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                args.baseShieldAdd += body.healthComponent.fullHealth * baseShield;
            }

            //The trigger should only happen once until you recharge, not after everytime you get hit below the threshold
            private void FixedUpdate()
            {
                if (NetworkServer.active)
                {
                    //Checks if the body is at full shield. shouldTrigger is just a switch to make sure that the effect doesn't trigger more than once below the shield threshold.
                    if (body.healthComponent.shield == body.healthComponent.fullShield && !shouldTrigger)
                        shouldTrigger = true;

                    //Checks if the body is lower than the shield threshold percentage.
                    if (body.healthComponent.shield < body.healthComponent.fullShield * threshold && shouldTrigger)
                    {
                        shouldTrigger = false;
                        FireStunSpray();
                        //not sure what addtimedbuffauthority is
                        body.AddTimedBuffAuthority(SlipContent.Buffs.PepperSpeed.buffIndex, maxBuffTime * (body.healthComponent.fullShield/(body.healthComponent.fullShield + body.healthComponent.fullHealth)) + buffTimeConstant);
                        //Util.PlaySound(explosionSoundString, gameObject);
                        Util.PlaySound(EntityStates.Bison.PrepCharge.enterSoundString, gameObject);
                    }
                }
            }

            private void FireStunSpray()
            {
                //Establishes a gameobject for the explosion
                Vector3 corePosition = Util.GetCorePosition(body.gameObject);
                float radius = body.radius + baseRadius + radiusPerStack * (stack - 1f);
                GameObject hitBoxStun = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GenericDelayBlast"), corePosition, Quaternion.identity);
                hitBoxStun.transform.localScale = new Vector3(radius, radius, radius);
                DelayBlast sprayAttack = hitBoxStun.GetComponent<DelayBlast>();

                //Fills characteristics of the explosion such as the damage type stunning
                sprayAttack.position = corePosition;
                sprayAttack.radius = radius;
                sprayAttack.attacker = body.gameObject;
                sprayAttack.falloffModel = BlastAttack.FalloffModel.None;
                sprayAttack.maxTimer = 0f;
                sprayAttack.damageType = DamageType.Stun1s;
                sprayAttack.explosionEffect = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("PepperSprayExplosion");
                //sprayAttack.delayEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/AffixWhiteExplosion");
                hitBoxStun.GetComponent<TeamFilter>().teamIndex = TeamComponent.GetObjectTeam(body.gameObject);

                //Fires the explosion
                NetworkServer.Spawn(hitBoxStun);
            }
        }
    }
}
