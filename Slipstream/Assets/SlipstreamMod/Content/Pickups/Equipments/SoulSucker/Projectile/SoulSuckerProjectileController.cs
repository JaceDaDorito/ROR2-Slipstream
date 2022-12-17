using System;
using RoR2.Projectile;
using Slipstream.Items;
using RoR2;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Slipstream.Projectiles
{
	[RequireComponent(typeof(ProjectileController))]
	public class SoulSuckerProjectileController : MonoBehaviour, IProjectileImpactBehavior
	{
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
            if (NetworkServer.active && alive && !impactInfo.collider?.GetComponent<HurtBox>()?.healthComponent)
            {
				ProjectileController projectileController = base.GetComponent<ProjectileController>();
				CharacterBody characterBody = projectileController.owner?.GetComponent<CharacterBody>();
				if (characterBody && characterBody.master)
				{
					GameObject trapInstance = Instantiate(soulSuckerTrapPrefab, base.transform.position, base.transform.rotation);
					trapInstance.GetComponent<TeamFilter>().teamIndex = base.GetComponent<TeamFilter>().teamIndex;
					trapInstance.GetComponent<GenericOwnership>().ownerObject = projectileController.owner;
					Deployable deployable = trapInstance.GetComponent<Deployable>();
					characterBody.master.AddDeployable(deployable, Equipments.SoulSucker.SoulSuckerTrapDeployable);
					deployable.onUndeploy = deployable.onUndeploy ?? new UnityEvent();
					deployable.onUndeploy.AddListener(new UnityAction(() => Destroy(trapInstance)));
					NetworkServer.Spawn(trapInstance);
				}
				Destroy(this);
				alive = false;
            }
		}

		[SerializeField]
		public GameObject soulSuckerTrapPrefab;
		private bool alive = true;
	}
}
