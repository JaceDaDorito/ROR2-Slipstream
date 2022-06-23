using Moonstorm;
using UnityEngine.AddressableAssets;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using On.RoR2;
using On.RoR2.UI;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Slipstream.Items
{
    //ill finish this later
    public class VoidShieldCatalog
    {
        private static Material cachedVoidMaterial;
        private static Material cachedEnergyMaterial;
        private static List<RoR2.ItemDef> voidShieldItems = new List<RoR2.ItemDef>();
        public void Init()
        {
            cachedVoidMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC/MissileVoid/materEnergyShieldVoid.mat").WaitForCompletion();
            cachedEnergyMaterial = LegacyResourcesAPI.Load<Material>("Materials/matEnergyShield");
            On.RoR2.HealthComponent.GetHealthBarValues += new On.RoR2.HealthComponent.hook_GetHealthBarValues(HealthComponent_GetHealthBarValues);
            On.RoR2.CharacterModel.UpdateOverlays += new On.RoR2.CharacterModel.hook_UpdateOverlays(CharacterModel_UpdateOverlays);
        }

        public void AddVoidShieldCatalog(RoR2.ItemDef itemDef)
        {
            voidShieldItems.Add(itemDef);
        }

        private bool HasVoidShield(RoR2.CharacterBody component)
        {
            bool hasVoidShield = false;
            if (voidShieldItems.Count <= 0)
                return hasVoidShield;
            for(int i = 0; i < voidShieldItems.Count; i++)
            {
                if (component.inventory.GetItemCount(voidShieldItems[i]) <= 0)
                    hasVoidShield = true;
            }
            return hasVoidShield;
        }

        private RoR2.HealthComponent.HealthBarValues HealthComponent_GetHealthBarValues(On.RoR2.HealthComponent.orig_GetHealthBarValues orig, RoR2.HealthComponent self)
        {
            //Material cachedMaterial = null;
            BrineSwarm.BrineSwarmBehavior component = null;
            RoR2.HealthComponent.HealthBarValues values = orig(self);
            if (self)
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
            orig(self);
            if (self.body)
            {
                /*if(self.body.inventory != null && !(self.body.inventory.GetItemCount(RoR2.DLC1Content.Items.MissileVoid) > 1))
                {

                }*/

            }
        }
    }
}
