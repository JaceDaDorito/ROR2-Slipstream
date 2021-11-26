using BepInEx.Configuration;
using RoR2;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;


namespace Assets.Slipstream.Modules
{
    public abstract class ItemBase
    {
        public abstract ItemDef ItemDef { get; }
        public abstract string ItemName { get; }
        public abstract string ItemPickupDesc { get; }
        public abstract string ItemFullDescription { get; }
        public abstract string ItemLore { get; }
        protected void SetLangs()
        {
            LanguageAPI.Add(ItemDef.nameToken, ItemName);
            LanguageAPI.Add(ItemDef.pickupToken, ItemPickupDesc);
            LanguageAPI.Add(ItemDef.descriptionToken, ItemFullDescription);
            LanguageAPI.Add(ItemDef.loreToken, ItemLore);
        }
        public abstract void Init(ConfigFile config);
        public abstract ItemDisplayRuleDict CreateItemDisplayRules();

        public abstract void Hooks();
       
        public int GetCount(CharacterBody body)
        {
            if (!body || !body.inventory) { return 0; }

            return body.inventory.GetItemCount(ItemDef);
        }

        public int GetCount(CharacterMaster master)
        {
            if (!master || !master.inventory) { return 0; }

            return master.inventory.GetItemCount(ItemDef);
        }
    }
}
