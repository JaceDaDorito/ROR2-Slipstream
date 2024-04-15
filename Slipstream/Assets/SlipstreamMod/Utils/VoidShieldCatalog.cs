using UnityEngine;
using System.Collections.Generic;

namespace Slipstream.Items
{
    //this doesn't work for some reason and im too stupid to understand why
    public class VoidShieldCatalog
    {
        private static List<RoR2.ItemDef> voidShieldItems = new List<RoR2.ItemDef>();
        public void Init()
        {
            On.RoR2.HealthComponent.GetHealthBarValues += new On.RoR2.HealthComponent.hook_GetHealthBarValues(HealthComponent_GetHealthBarValues);
            On.RoR2.CharacterModel.UpdateOverlays += new On.RoR2.CharacterModel.hook_UpdateOverlays(CharacterModel_UpdateOverlays);
        }

        public static void AddVoidShieldCatalog(RoR2.ItemDef itemDef)
        {
            voidShieldItems.Add(itemDef);
        }

        private bool HasVoidShield(RoR2.CharacterBody component)
        {
            bool hasVoidShield = false;
            if (voidShieldItems.Count <= 0)
            {
                return hasVoidShield;
            }
            for(int i = 0; i < voidShieldItems.Count; i++)
            {
                if (component.inventory.GetItemCount(voidShieldItems[i]) > 0)
                    hasVoidShield = true;
            }
            return hasVoidShield;
        }

        private RoR2.HealthComponent.HealthBarValues HealthComponent_GetHealthBarValues(On.RoR2.HealthComponent.orig_GetHealthBarValues orig, RoR2.HealthComponent self)
        {
            //Material cachedMaterial = null;
            RoR2.HealthComponent.HealthBarValues values = orig(self);
            if (self.body.inventory)
            {
                if (HasVoidShield(self.body))
                {
                    values.hasVoidShields = true;
                }
            }
            return values;
        }

        private void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, RoR2.CharacterModel self)
        {
            Material cachedMaterial = null;
            if (!self.body || !self.body.inventory)
            {
                orig(self);
                return;
            }
            if (HasVoidShield(self.body))
            {
                cachedMaterial = RoR2.CharacterModel.energyShieldMaterial;
                RoR2.CharacterModel.energyShieldMaterial = RoR2.CharacterModel.voidShieldMaterial;
            }
            orig(self);
            if (cachedMaterial)
            {
                RoR2.CharacterModel.energyShieldMaterial = cachedMaterial;
            }
            return;
        }
    }
}
