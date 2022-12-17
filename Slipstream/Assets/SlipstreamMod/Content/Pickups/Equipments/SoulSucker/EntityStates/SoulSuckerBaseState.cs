using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using EntityStates;
using RoR2.Orbs;
using System.Linq;

namespace EntityStates.SoulSucker
{
    public class SoulSuckerBaseState : BaseState
    {
		public static float grabRadius = 15f;
		public TeamIndex teamIndex;
        public GameObject owner;
		public CharacterBody ownerBody;
        public HealthComponent victim;
        public override void OnEnter()
        {
            base.OnEnter();
            teamIndex = base.GetComponent<TeamFilter>().teamIndex;
            owner = base.GetComponent<GenericOwnership>()?.ownerObject;
			ownerBody = owner?.GetComponent<CharacterBody>();
        }
		public class SoulOrb : Orb
		{
			public override void Begin()
			{
				base.duration = base.distanceToTarget / speed;
				EffectData effectData = new EffectData
				{
					scale = this.scale,
					origin = this.origin,
					genericFloat = base.duration
				};
				effectData.SetHurtBoxReference(this.target);
				GameObject effectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OrbEffects/DevilOrbEffect");
				EffectManager.SpawnEffect(effectPrefab, effectData, true);
			}

			public override void OnArrival()
			{
				if (this.target)
				{
					HealthComponent healthComponent = this.target.healthComponent;
					if (healthComponent)
					{
						DamageInfo damageInfo = new DamageInfo();
						damageInfo.damage = this.damageValue;
						damageInfo.attacker = this.attacker;
						damageInfo.inflictor = inflictor;
						damageInfo.force = Vector3.zero;
						damageInfo.crit = this.isCrit;
						damageInfo.procChainMask = this.procChainMask;
						damageInfo.procCoefficient = this.procCoefficient;
						damageInfo.position = this.target.transform.position;
						damageInfo.damageColorIndex = this.damageColorIndex;
						healthComponent.TakeDamage(damageInfo);
						GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
						GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
					}
				}
			}

			public HurtBox PickNextTarget(Vector3 position, float range, HealthComponent currentVictim)
			{
				BullseyeSearch bullseyeSearch = new BullseyeSearch();
				bullseyeSearch.searchOrigin = position;
				bullseyeSearch.searchDirection = Vector3.zero;
				bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
				bullseyeSearch.teamMaskFilter.RemoveTeam(this.teamIndex);
				bullseyeSearch.filterByLoS = false;
				bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
				bullseyeSearch.maxDistanceFilter = range;
				bullseyeSearch.RefreshCandidates();
				List<HurtBox> list = bullseyeSearch.GetResults().Where((HurtBox hurtBox) => hurtBox.healthComponent != currentVictim).ToList();
				if (list.Count <= 0)
				{
					return null;
				}
				return list[UnityEngine.Random.Range(0, list.Count)];
			}

			private const float speed = 30f;

			public float damageValue;

			public GameObject attacker;

			public GameObject inflictor;

			public TeamIndex teamIndex;

			public bool isCrit;

			public float scale;

			public ProcChainMask procChainMask;

			public float procCoefficient = 0.2f;

			public DamageColorIndex damageColorIndex;

		}
	}
}
