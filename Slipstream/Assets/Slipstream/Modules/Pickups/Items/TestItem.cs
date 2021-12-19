using Moonstorm;
using RoR2;
using UnityEngine;
using R2API;
using System;

namespace Slipstream.Items
{
    /*public class TestItem : ItemBase
    {
        private const string token = "TESTITEM_DESCRIPTION";
        public override ItemDef ItemDef { get; set; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<ItemDef>("TestItem");

        public static string section;

        [ConfigurableField(ConfigName = "Gaming", ConfigDesc = "Yeah it works.")]
        [TokenModifier(token, StatTypes.Default, 0)]
        public static float baseGamer = 1.0f;

        public override void AddBehavior(ref CharacterBody body, int stack)
        {
            SlipLogger.LogD($"Initializing Test Item");
            body.AddItemBehavior<TestItemBehavior>(stack);

        }
        public class TestItemBehavior : CharacterBody.ItemBehavior, IBodyStatArgModifier
        {
            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                args.baseShieldAdd += baseGamer * 5;
            }
        }
    }*/
}
