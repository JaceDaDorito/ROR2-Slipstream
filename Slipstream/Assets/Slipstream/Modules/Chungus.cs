/*using R2API;
using RoR2;
using UnityEngine;

namespace Slipstream.Items
{
    public class Chungus //I hate this mod
    {
        public ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();

        public void Init()
        {
            ChungusItem();
            AddLanguageTokens();
            Hooks();
        }

        private void ChungusItem()
        {
            itemDef.name = "Chungus";
            itemDef.tier = ItemTier.Tier3;
            itemDef.pickupModelPrefab = SlipstreamPlugin.AssetBundle.LoadAsset<GameObject>("Assets/Import/Cube/Cube.prefab");
            itemDef.pickupIconSprite = SlipstreamPlugin.AssetBundle.LoadAsset<Sprite>("Assets/Import/Cube/Cube_icon.png");
            itemDef.nameToken = "CHUNGUS_NAME";
            itemDef.pickupToken = "CHUNGUS_PICKUP";
            itemDef.descriptionToken = "CHUNGUS_DESC";
            itemDef.loreToken = "CHUNGUS_LORE";
            itemDef.tags = new ItemTag[] { ItemTag.Healing, ItemTag.EquipmentRelated };

            //TODO: MAKE THIS SHIT ACTUALLY WORK
            var itemDisplayRules = new ItemDisplayRule[1];
            itemDisplayRules[0].followerPrefab = SlipstreamPlugin.AssetBundle.LoadAsset<GameObject>("Assets/Import/Cube/Cube.prefab");
            itemDisplayRules[0].childName = "Chest";
            itemDisplayRules[0].localScale = new Vector3(2f, 2f, 2f);

            SlipstreamPlugin.ModItemDefs.Add(itemDef);
        }

        private void AddLanguageTokens()
        {
            LanguageAPI.Add("CHUNGUS_NAME", "Charged Fungus");
            LanguageAPI.Add("CHUNGUS_PICKUP", "Heal over time when your equipment is fully charged.");
            LanguageAPI.Add("CHUNGUS_DESC", "Heal for <style=cIsHealing>2.5%</style> <style=cStack>(+2.5% per stack)</style> of your maximum health/second");
            LanguageAPI.Add("CHUNGUS_LORE", "Big Big Chungus Big Chungus Big Chungus (ooh) Big Big Chungus Big Chungus Big Chungus Big Big Chungus Big Chungus Big Chungus Big Big Chungus Big Chungus Big Chungus Big Big Chungus Big Chungus Big Chungus Big Big Chungus Big Chungus Big Chungus Big Big Chungus Big Chungus Big Chungus Big Big Chungus Big Chungus Big Chungus" +
                "Chungus, Big Chungus Chungus, Big Chungus Chungus, Big Chungus Chungus, Big Chungus Big Big Chungus Big Chungus Big Chungus Big Big Chungus Big Chungus Big Chungus Big Big Chungus Big Chungus Big Chungus Big Big Chungus Big Chungus Big Chungus" +
                "Chungus, Big Chungus Chungus, Big Chungus Chungus, Big Chungus Chungus, Big Chungus Big Big Chungus");
        }

        private float timer = 0;

        private void Hooks()
        {
            On.RoR2.CharacterBody.FixedUpdate += (On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self) =>
            {
                if (self.inventory != null)
                {
                    if (self.inventory.GetItemCount(itemDef) > 0)
                    {
                        timer += Time.deltaTime;
                        if (timer >= 1)
                        {
                            System.Console.WriteLine("3");
                            if (self.inventory.currentEquipmentState.charges > 0)
                            {
                                self.healthComponent.Heal(self.healthComponent.fullHealth * 0.025f * self.inventory.GetItemCount(itemDef), default);
                            }
                            timer = 0;
                        }
                    }
                }
                orig(self);
            };
        }
    }
}*/