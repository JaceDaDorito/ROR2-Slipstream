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
        public GameObject projectilePrefab = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("MageNanoFlame");
        private static float damage = 3.5f;
        private static float force = 30f;
        public float charge = 0.2f;
        public float baseDuration = 0.4f;
        private float duration;
        private ChildLocator childLocator;
        private Transform head;
        public static int maxBalls = 5;
        public float balls;
        public static float[] angleOrder = {0f, 180f, 45f, 135f, 90f};
        public static float distanceFromHead = 2;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            balls = Mathf.Clamp(charge/(1/maxBalls) + 1, 1, maxBalls);
            childLocator = base.GetModelChildLocator();
            if (childLocator)
            {
                head = childLocator.FindChild("HeadCenter");
            }
            Fire();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private void Fire()
        {
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();

                if (projectilePrefab != null)
                {
                    for (int i = 0; i < balls; i++)
                    {
                        Vector3 ballPosition = new Vector3((Mathf.Cos(angleOrder[i]) * distanceFromHead) + head.position.x, (Mathf.Sin(angleOrder[i] * distanceFromHead)) + head.position.y, head.position.z);
                        FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                        {
                            projectilePrefab = this.projectilePrefab,
                            position = ballPosition,
                            rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                            owner = base.gameObject,
                            damage = this.damageStat * damage,
                            force = force,
                            crit = base.RollCrit()
                        };
                        ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                    }
                }
            }
        }
    }
}
