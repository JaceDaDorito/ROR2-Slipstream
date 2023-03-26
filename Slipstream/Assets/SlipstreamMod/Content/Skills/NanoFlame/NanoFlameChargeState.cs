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
        public GameObject effectPrefab = SlipAssets.LoadAsset<GameObject>("MageNanoFlameChargeBolts", SlipBundle.Skills);
        private List<GameObject> instantiatedEffects = new List<GameObject>();
        private int effectIndex = 0;
        public static float[] angleOrder = { 0f, 180f, 45f, 135f, 90f };
        public static float arcRadius = 1.5f;
        public static int maxBalls = 5;

        private ChildLocator childLocator;
        private Transform head;
        private static float baseDuration = 2f;
        private float duration;
        private float minChargeDuration = 0.3f;
        public static GameObject crosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageCrosshair.prefab").WaitForCompletion();
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private uint loopSoundInstanceId;
        private float minBloomRadius = 0.1f;
        private float maxBloomRadius = 0.5f;

        

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / this.attackSpeedStat;
            //animator = GetModelAnimator();
            PlayAnimation("Gesture, Additive", "PrepWall", "PrepWall.playbackRate", this.duration);
            if (crosshairOverridePrefab)
            {
                crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(characterBody, crosshairOverridePrefab, CrosshairUtils.OverridePriority.Skill);
            }
            childLocator = base.GetModelChildLocator();
            if (childLocator)
            {
                head = childLocator.FindChild("Head");
            }
            Util.PlayAttackSpeedSound("Play_moonBrother_blueWall_slam_start", gameObject, attackSpeedStat);
            loopSoundInstanceId = this.loopSoundInstanceId = Util.PlayAttackSpeedSound("Play_lunar_wisp_attack2_chargeLoop", base.gameObject, this.attackSpeedStat);
            base.StartAimMode(duration + 2, false);
        }

        public override void OnExit()
        {
            /*for(int i = 0; i < instantiatedEffects.Count; i++)
            {
                Destroy(instantiatedEffects[i]);
            }*/
            //instantiatedEffects.Clear();
            if (crosshairOverrideRequest != null)
            {
                crosshairOverrideRequest.Dispose();
            }
            AkSoundEngine.StopPlayingID(this.loopSoundInstanceId);
            if (!this.outer.destroying)
            {
                this.PlayAnimation("Gesture, Additive", "Empty");
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

        public override void Update()
        {
            base.Update();
            base.characterBody.SetSpreadBloom(Util.Remap(this.CalcCharge(), 0f, 1f, this.minBloomRadius, this.maxBloomRadius), true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float charge = CalcCharge();
            int balls = (int)Mathf.Clamp(charge * maxBalls, 0, maxBalls - 1) + 1;
            if (balls - 1 >= effectIndex)
            {   
                Quaternion localAngle = Quaternion.AngleAxis(angleOrder[effectIndex], Vector3.forward);
                Vector3 ballPosition = ((head.rotation * localAngle) * (Vector3.right * arcRadius)) + head.position;
                instantiatedEffects.Add(UnityEngine.Object.Instantiate<GameObject>(effectPrefab, ballPosition, Quaternion.identity));
                instantiatedEffects[effectIndex].transform.parent = head;
                Util.PlayAttackSpeedSound("Play_item_use_molotov_throw", instantiatedEffects[effectIndex], characterBody.attackSpeed);
                effectIndex++;
            }
            if (base.isAuthority && ((!base.IsKeyDownAuthority() && base.fixedAge >= minChargeDuration) || base.fixedAge >= this.duration))
            {
                ThrowNanoFlame nextState = new ThrowNanoFlame();
                nextState.balls = balls;
                nextState.instantiatedEffects = instantiatedEffects;
                this.outer.SetNextState(nextState);
            }
        }
    }
}