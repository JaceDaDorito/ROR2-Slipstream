using Moonstorm;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace Slipstream.Items
{
    public class TestItem : ItemBase
    {
        private const string token = "TESTITEM_DESC";
        public override ItemDef ItemDef { get; set; } = Assets.SlipAssets.LoadAsset<ItemDef>("TestItem");

        public static string section;

        [ConfigurableField(ConfigName = "Gaming", ConfigDesc = "Yeah it works.")]
        [TokenModifier(token, StatTypes.Default, 0)]
        public static float baseGamer = 1.0f;

        public class TestItemBehavior : CharacterBody.ItemBehavior
        {

        }
    }
}
