using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using Slipstream;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Slipstream.Modules
{
    class TestItem : ItemBase
    {
        public ItemDef itemDef { get; set; } = ContentPackProvider.contentPack.itemDefs.Find("TestItem");
        public override ItemDef ItemDef => itemDef;

        public override string ItemName => "Test Item Name";

        public override string ItemPickupDesc => "Test Item Desc";

        public override string ItemFullDescription => "Test Item Full Desc";

        public override string ItemLore => "Test Item Lore";

        public static GameObject ItemBodyModelPrefab;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            //CreateLang();
        }
        public void CreateConfig(ConfigFile config)
        {
            
        }

        public override void Hooks()
        {
            throw new NotImplementedException();
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            throw new NotImplementedException();
            //ItemBodyModelPrefab = ItemDef.pickupModelPrefab;
            //var itemDisplay = ItemBodyModelPrefab.AddComponent<ItemDisplay>();
        }

    }
}
