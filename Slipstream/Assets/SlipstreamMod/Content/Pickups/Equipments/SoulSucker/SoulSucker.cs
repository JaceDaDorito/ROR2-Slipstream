using MSU;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using RoR2.Orbs;
using System.Collections.Generic;
using System.Linq;

namespace Slipstream.Equipments
{
	[DisabledContent]
	public class SoulSucker : EquipmentBase
    {
		
        public override EquipmentDef EquipmentDef { get; } = SlipAssets.LoadAsset<EquipmentDef>("SoulSucker", SlipBundle.Equipments);

        public static DeployableSlot SoulSuckerTrapDeployable;
        public static GameObject ProjectilePrefab;
        public override void Initialize()
        {
            base.Initialize();
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
			ProjectilePrefab = SlipAssets.LoadAsset<GameObject>("SoulSuckerProjectile.prefab", SlipBundle.Equipments);
			//ProjectilePrefab.GetComponent<ProjectileController>().ghostPrefab = Addressables.LoadAssetAsync<GameObject>("83c50e38bf2b31c4fb10d9b657f5f780").WaitForCompletion();
            SoulSuckerTrapDeployable = DeployableAPI.RegisterDeployableSlot((CharacterMaster self, int multiplier) => 1);
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
			orig(self);
            if (self.HasBuff(SlipContent.Buffs.SoulRoot))
            {
				float scalar = self.healthComponent ? self.healthComponent.fullCombinedHealth / self.baseMaxHealth : 1f;
				self.regen = -20f * scalar;
				self.moveSpeed = 0;
				self.acceleration = 0;
            }
        }

        public override bool FireAction(RoR2.EquipmentSlot slot)
        {
			CharacterBody characterBody = slot.characterBody;
			CharacterMaster characterMaster = characterBody?.master;
			if (!characterMaster)
			{
				return false;
			}
			Ray aimRay = slot.GetAimRay();
			Quaternion rotation = Quaternion.LookRotation(aimRay.direction);
			FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
			{
				projectilePrefab = ProjectilePrefab,
				crit = characterBody.RollCrit(),
				damage = 0f,
				damageColorIndex = DamageColorIndex.Item,
				force = 0f,
				owner = slot.gameObject,
				position = aimRay.origin,
				rotation = rotation
			};
			ProjectileManager.instance.FireProjectile(fireProjectileInfo);
			return true;
		}
		
	}
}
