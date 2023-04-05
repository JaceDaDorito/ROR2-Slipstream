using Moonstorm;
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

namespace EntityStates.Mage.Weapon
{
    public class ThrowNanoFlame : BaseState
    {
        private const string token = "SLIP_SKILL_NANOFLAME_DESC";

        [ConfigurableField(ConfigName = "Damage Coeff", ConfigDesc = "Damage Coefficient of each fireball.", ConfigSection = "NanoFlame")]
        [TokenModifier(token, StatTypes.MultiplyByN, 0, "100")]
        private static float damage = 3.5f;

        public GameObject projectilePrefab = SlipAssets.LoadAsset<GameObject>("MageNanoFlame", SlipBundle.Skills);
        public GameObject effectStartPrefab = SlipAssets.LoadAsset<GameObject>("MageNanoFlameStartEffect", SlipBundle.Skills);
        public List<GameObject> instantiatedEffects = new List<GameObject>();
        private GameObject muzzleFlashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MuzzleflashMageFire.prefab").WaitForCompletion();
        private static float force = 30f;
        private static float convergeDistance = 60f;
        //public float charge;
        public float baseDuration = 0.4f;
        private float duration;
        private ChildLocator childLocator;
        private Transform head;
        //public static int maxBalls = NanoFlameChargeState.maxBalls;
        public float balls;
        public static float[] angleOrder = NanoFlameChargeState.angleOrder;
        public static float arcRadius = NanoFlameChargeState.arcRadius;
        private int projectileIndex = 0;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            //balls = (int)Mathf.Clamp(charge * (float)maxBalls - 1, 1f, maxBalls - 1);
            this.PlayAnimation("Gesture, Additive", "FireWall");
            Util.PlayAttackSpeedSound("Play_lemurianBruiser_m2_end", gameObject, characterBody.attackSpeed);
            if (muzzleFlashEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(muzzleFlashEffectPrefab, gameObject, "MuzzleLeft", false);
                EffectManager.SimpleMuzzleFlash(muzzleFlashEffectPrefab, gameObject, "MuzzleRight", false);
            }
            childLocator = base.GetModelChildLocator();
            if (childLocator)
            {
                head = childLocator.FindChild("Head");
            }
            //Fire();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            float formula = Mathf.Clamp(Mathf.Clamp01(this.fixedAge / duration) * balls, 0, balls - 1);
            if (formula >= projectileIndex)
            {
                float difference = formula - projectileIndex;
                for (int i = 0; i < (int)(difference) + 1; i++)
                {
                    Destroy(instantiatedEffects[projectileIndex]);
                    if (base.isAuthority)
                    {
                        Fire(projectileIndex);
                        projectileIndex++;
                    }
                }

            }

            if (base.isAuthority)
            {
                if (fixedAge >= duration)
                {
                    outer.SetNextStateToMain();
                }
            }


        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private void Fire(int i)
        {
            Ray aimRay = base.GetAimRay();
            //RaycastHit hit;
            //bool raycast = Physics.Raycast(aimRay.origin, aimRay.direction, out hit, 30f);
            //Util.PlayAttackSpeedSound("Play_magmaWorm_impact", base.gameObject, this.attackSpeedStat);

            if (projectilePrefab != null)
            {
                //var projectilePosition = Quaternion.LookRotation(head.forward);
                /*Vector3 arcAngles = new Vector3((Mathf.Cos(angleOrder[i]) * distanceFromHead) + head.position.x,
                                                 Mathf.Sin(angleOrder[i] * distanceFromHead) + head.position.y,
                                                 head.position.z);
                Vector3 ballPosition = arcAngles * characterBody.transform.rotation;*/

                Quaternion localAngle = Quaternion.AngleAxis(angleOrder[i], Vector3.forward);
                Vector3 ballPosition = ((head.rotation * localAngle) * (Vector3.right * arcRadius)) + head.position;

                Ray resultRay;
                resultRay = new Ray(ballPosition, (aimRay.origin + aimRay.direction * convergeDistance) - ballPosition);
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    projectilePrefab = this.projectilePrefab,
                    position = ballPosition,
                    rotation = Util.QuaternionSafeLookRotation(resultRay.direction)/*raycast? Util.QuaternionSafeLookRotation(resultRay.direction): Util.QuaternionSafeLookRotation(aimRay.direction)*/,
                    owner = base.gameObject,
                    damage = this.damageStat * damage,
                    force = force,
                    crit = base.RollCrit()
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);

                EffectManager.SpawnEffect(effectStartPrefab, new EffectData
                {
                    origin = ballPosition
                }, true);
            }
        }
    }
}