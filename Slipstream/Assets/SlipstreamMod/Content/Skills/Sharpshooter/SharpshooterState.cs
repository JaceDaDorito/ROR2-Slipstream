using MSU;
using R2API.ScriptableObjects;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RoR2.Skills;
using RoR2.Projectile;
using RoR2;
using UnityEngine.AddressableAssets;
using HG;
using Slipstream;

namespace EntityStates.Commando.CommandoWeapon
{
    public class SharpshooterState : BaseSkillState, SteppedSkillDef.IStepSetter
    {
        private const string TOKEN = "SLIP_SKILL_SHARPSHOOTER_DESC";
        public static GameObject muzzleEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/MuzzleflashBandit2.prefab").WaitForCompletion();

        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/HitsparkBandit.prefab").WaitForCompletion();

        public static GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/TracerBanditPistol.prefab").WaitForCompletion();

        public static float damageCoefficient = 2.8f;

        public static float force = 400;

        public static float baseDuration = 0.5f;

        public static string firePistolSoundString = "Play_bandit2_R_fire";

        public static float recoilAmplitude = 2.5f;

        public static float spreadBloomValue = 0.3f;

        public static float commandoBoostBuffCoefficient = 0.4f;

        private int pistol;

        private Ray aimRay;

        private float duration;

        void SteppedSkillDef.IStepSetter.SetStep(int i)
        {
            pistol = i;
        }

        private void FireBullet(string targetMuzzle)
        {
            Util.PlaySound(firePistolSoundString, base.gameObject);
            if (muzzleEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(muzzleEffectPrefab, base.gameObject, targetMuzzle, false);
            }

            base.AddRecoil(-0.8f * recoilAmplitude, -1.6f * recoilAmplitude, -0.3f * recoilAmplitude, 0.3f * recoilAmplitude);

            if (base.isAuthority)
            {
                new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = base.characterBody.spreadBloomAngle,
                    damage = damageCoefficient * damageStat,
                    force = force,
                    tracerEffectPrefab = tracerEffectPrefab,
                    muzzleName = targetMuzzle,
                    hitEffectPrefab = hitEffectPrefab,
                    isCrit = Util.CheckRoll(critStat, base.characterBody.master),
                    radius = 0.1f,
                    smartCollision = true
                }.Fire();
            }

            base.characterBody.AddSpreadBloom(spreadBloomValue);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 3f, false);
            if (pistol % 2 == 0)
            {
                PlayAnimation("Gesture Additive, Left", "FirePistol, Left");
                FireBullet("MuzzleLeft");
                return;
            }
            PlayAnimation("Gesture Additive, Right", "FirePistol, Right");
            FireBullet("MuzzleRight");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge < duration || !base.isAuthority)
            {
                return;
            }
            if (base.activatorSkillSlot.stock <= 0)
            {
                outer.SetNextState(new ReloadPistols());
                return;
            }
            outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}