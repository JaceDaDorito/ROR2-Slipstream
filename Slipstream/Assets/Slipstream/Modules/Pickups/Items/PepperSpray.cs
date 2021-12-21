using Moonstorm;
using Slipstream.Buffs;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using System;

namespace Slipstream.Items
{
    public class PepperSpray: ItemBase
    {
        private const string token = "SLIP_ITEM_PEPPERSPRAY_DESC";
        public override ItemDef ItemDef { get; set; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<ItemDef>("PepperSpray");

        public static string section;

        [ConfigurableField(ConfigName = "Base Shield", ConfigDesc = "Shield percentage after having at least one stack.")]
        //[TokenModifier(token, StatTypes.Default, 0)]
        public static float baseShield = 0.05f;

        [ConfigurableField(ConfigName = "Shield Threshold", ConfigDesc = "Percentage of total shield in order to trigger the effect.")]
        public static float threshold = 0.5f;

        [ConfigurableField(ConfigName = "Base Radius", ConfigDesc = "Initial radius of the stun effect.")]
        public static float baseRadius = 30.0f;

        [ConfigurableField(ConfigName = "Radius Increase", ConfigDesc = "Amount of increased stun radius per stack.")]
        public static float radiusPerStack = 3.0f;

        [ConfigurableField(ConfigName = "Speed Increase", ConfigDesc = "Movement speed increase when Pepper Speed is active")]
        public static float speedIncrease = 0.5f;

        [ConfigurableField(ConfigName = "Base Speed Duration", ConfigDesc = "Initial speed duration with theoretically no shield (which is impossible but you get the point).")]
        public static float baseDuration = 1.0f;

        [ConfigurableField(ConfigName = "Speed Duration Multiplier", ConfigDesc = "Increased Pepper Speed duration per shield")]
        public static float durationMultiplier = 0.5f;

        public override void AddBehavior(ref CharacterBody body, int stack)
        {
            SlipLogger.LogD($"Initializing Test Item");
            body.AddItemBehavior<PepperSprayBehavior>(stack);

        }
        public class PepperSprayBehavior : CharacterBody.ItemBehavior, IBodyStatArgModifier
        {
            private bool shouldTrigger = false;
            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                args.baseShieldAdd += body.healthComponent.fullHealth * baseShield;
            }

            //The trigger should only happen once until you recharge, not after everytime you get hit below the threshold
            private void FixedUpdate()
            {
                if (NetworkServer.active)
                {
                    //Checks if the body is at full shield.
                    if (body.healthComponent.shield == body.healthComponent.fullShield && !shouldTrigger)
                        shouldTrigger = true;

                    if (body.healthComponent.shield < body.healthComponent.fullShield * threshold && shouldTrigger)
                    {
                        shouldTrigger = false;
                        FireStunSpray();
                        //AddBuff();
                        body.AddTimedBuff(PepperSpeed.buff, baseDuration + durationMultiplier * body.healthComponent.fullShield);
                        Util.PlaySound(EntityStates.Bison.PrepCharge.enterSoundString, gameObject);
                    }
                }
            }

            private void AddBuff()
            {

            }

            private void FireStunSpray()
            {
                Vector3 corePosition = Util.GetCorePosition(body.gameObject);
                float radius = baseRadius + radiusPerStack * (stack - 1f);
                GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GenericDelayBlast"), corePosition, Quaternion.identity);
                gameObject2.transform.localScale = new Vector3(radius, radius, radius);
                DelayBlast sprayAttack = gameObject2.GetComponent<DelayBlast>();

                sprayAttack.position = corePosition;
                sprayAttack.radius = radius;
                sprayAttack.attacker = body.gameObject;
                sprayAttack.falloffModel = BlastAttack.FalloffModel.None;
                sprayAttack.maxTimer = 0f;
                sprayAttack.damageType = DamageType.Stun1s;
                sprayAttack.explosionEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/AffixWhiteExplosion");
                //sprayAttack.delayEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/AffixWhiteExplosion");
                gameObject2.GetComponent<TeamFilter>().teamIndex = TeamComponent.GetObjectTeam(body.gameObject);

                NetworkServer.Spawn(gameObject2);
            }
        }
    }
}
