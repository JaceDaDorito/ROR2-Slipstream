using System;
using UnityEngine;
using RoR2.Orbs;
using RoR2;
using Slipstream.Buffs;

namespace Slipstream.Orbs
{
	public class SandsweptDeathOrb : Orb
	{
		private const float speed = 30f;
		private CharacterBody characterBody;
		public override void Begin()
		{
			base.duration = base.distanceToTarget / speed;
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			//AffixSandDeathOrbEffect
			EffectManager.SpawnEffect(SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("AffixSandDeathOrbEffect"), effectData, true);
			HurtBox hurtBox = this.target.GetComponent<HurtBox>();
			characterBody = (hurtBox != null) ? hurtBox.healthComponent.GetComponent<CharacterBody>() : null;
		}

		public override void OnArrival()
		{
			if (characterBody.HasBuff(Grainy.buff))
			{
				characterBody.RemoveBuff(Grainy.buff);
			}
		}


	}
}
