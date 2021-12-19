using Moonstorm;
using RoR2;
using UnityEngine;
using R2API;
using System;

namespace Slipstream.Items
{
    public class PepperSpray : ItemBase
    {
        private const string token = "SLIP_ITEM_PEPPERSPRAY_DESCRIPTION";
        public override ItemDef ItemDef { get; set; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<ItemDef>("PepperSpray");

        public static string section;

        [ConfigurableField(ConfigName = "", ConfigDesc = "")]
        [TokenModifier(token, StatTypes.Default, 0)]

        public static float baseRadius = 1.0f;
        public static float radiusIncrease = 1.0f;
        public static float buffDuration = 1.0f;
        public static float moveSpeed = 1.0f;

        public override void AddBehavior(ref CharacterBody body, int stack)
        {
            body.AddItemBehavior<PepperSprayBehavior>(stack);
        }

        public class PepperSprayBehavior : CharacterBody.ItemBehavior, IOnIncomingDamageServerReceiver
        {
            private bool shouldTrigger;
            public void Awake()
            {
                //To make sure that players are at full shield when they pick up the item.
                if (body.healthComponent.shield == body.healthComponent.fullShield)
                    shouldTrigger = true;
                else
                    shouldTrigger = false;
            }

            public void OnIncomingDamageServer(DamageInfo damageInfo)
            {
                throw new NotImplementedException();
            }

        }
    }
}
