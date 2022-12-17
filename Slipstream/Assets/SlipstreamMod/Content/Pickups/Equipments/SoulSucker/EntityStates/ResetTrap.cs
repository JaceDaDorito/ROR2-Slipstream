using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using EntityStates;

namespace EntityStates.SoulSucker
{
    public class ResetTrap : SoulSuckerBaseState
    {
		public override void OnEnter()
		{
			base.OnEnter();
			//play re-opening animation
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		public static float duration = 1f;

	}
}
