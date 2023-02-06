using Moonstorm;
using R2API.ScriptableObjects;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using RoR2.Skills;
using RoR2.Projectile;
using RoR2.UI;
using RoR2;
using UnityEngine.AddressableAssets;
using HG;
using Slipstream;

namespace EntityStates.Mage.Weapon {
    public class NanoFlameChargeState : BaseSkillState
    {
        private Animator animator;
        public GameObject projectilePrefab = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("MageNanoFlame");
        private static float baseDuration = 1.5f;
        private float duration;
        private float minChargeDuration = 0.3f;
        public static GameObject crosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageCrosshair.prefab").WaitForCompletion();
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / this.attackSpeedStat;
            animator = GetModelAnimator();
            if (crosshairOverridePrefab)
            {
                crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(characterBody, crosshairOverridePrefab, CrosshairUtils.OverridePriority.Skill);
            }
            base.StartAimMode(duration + 2, false);
        }

        public override void OnExit()
        {
            if (crosshairOverrideRequest != null)
            {
                crosshairOverrideRequest.Dispose();
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private float CalcCharge()
        {
            return Mathf.Clamp01(fixedAge/duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float charge = CalcCharge();
            if (base.isAuthority && ((!base.IsKeyDownAuthority() && base.fixedAge >= minChargeDuration) || base.fixedAge >= this.duration))
            {
                ThrowNanoFlame nextState = new ThrowNanoFlame();
                nextState.charge = charge;
                this.outer.SetNextState(nextState);
            }
        }
    }
}