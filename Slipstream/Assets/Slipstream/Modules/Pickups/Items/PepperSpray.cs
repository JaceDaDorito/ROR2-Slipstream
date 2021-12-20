using Moonstorm;
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

        [ConfigurableField(ConfigName = "Base Shield", ConfigDesc = "Initial shield gain when you have at least one Pepper Spray.")]
        //[TokenModifier(token, StatTypes.Default, 0)]
        public static float baseShield = 5.0f;

        [ConfigurableField(ConfigName = "Shield Threshold", ConfigDesc = "Percentage of total shield in order to trigger the effect.")]
        public static float threshold = 0.5f;

        public override void AddBehavior(ref CharacterBody body, int stack)
        {
            SlipLogger.LogD($"Initializing Test Item");
            body.AddItemBehavior<PepperSprayBehavior>(stack);

        }
        public class PepperSprayBehavior : CharacterBody.ItemBehavior, IBodyStatArgModifier, IOnTakeDamageServerReceiver
        {
            private bool shouldTrigger = false;
            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                args.baseShieldAdd += baseShield;
            }

            //The trigger should only happen once until you recharge, not after everytime you get hit below the threshold
            private void FixedUpdate()
            {
                if (NetworkServer.active)
                {
                    //Checks if the body is at full shield.
                    if (body.healthComponent.shield == body.healthComponent.fullShield && !shouldTrigger)
                        shouldTrigger = true;
                }
            }

            public void OnTakeDamageServer(DamageReport damageReport)
            {
                //Checks if the body went below the threshhold
                if(body.healthComponent.shield < body.healthComponent.fullShield * threshold && shouldTrigger)
                {
                    Chat.AddMessage("Trigger");
                    shouldTrigger = false;
                }

            }


        }
    }
}
