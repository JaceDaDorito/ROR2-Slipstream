using Moonstorm;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using System;

namespace Slipstream.Items
{
    public class PepperSpray : ItemBase
    {
        private const string token = "SLIP_ITEM_PEPPERSPRAY_DESC";
        public override ItemDef ItemDef { get; set; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<ItemDef>("Slip_PepperSpray");

        public static string section;

        [ConfigurableField(ConfigName = "Shield Trigger Threshold", ConfigDesc = "Determines what percentage of your total shield you should be below in order to trigger the Pepper Spray effect.")]
        //[TokenModifier(token, StatTypes.Default, 0)]
        public static float threshold = 0.5f;

        [ConfigurableField(ConfigName = "Base Shield", ConfigDesc = "Base shield amount when you have Pepper Spray in your inventory.")]
        public static float baseShield = 5.0f;

        //public static float baseRadius = 1.0f;
        //public static float radiusIncrease = 1.0f;
        //public static float buffDuration = 1.0f;
        //public static float moveSpeed = 1.0f;

        public override void AddBehavior(ref CharacterBody body, int stack)
        {
            body.AddItemBehavior<PepperSprayBehavior>(stack);
        }

        public class PepperSprayBehavior : CharacterBody.ItemBehavior//,  IOnTakeDamageServerReceiver
        {
            private bool shouldTrigger = false;
            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                args.baseShieldAdd += baseShield;
            }

            private void FixedUpdate()
            {
                if (NetworkServer.active)
                {
                    //Will check every frame if player is at full shield, once it is at full it will only do it once.

                    if (body.healthComponent.shield == body.healthComponent.fullShield && !shouldTrigger)
                        shouldTrigger = true;
                }
            }
            public void OnTakeDamageServer(DamageReport damageReport)
            {
                if (body.healthComponent.shield < body.healthComponent.fullShield * threshold && shouldTrigger)
                {
                    Chat.AddMessage("triggered");
                }
            }

            
        }
    }
}
