using System;
using RoR2.Projectile;
using RoR2;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Slipstream.Projectiles
{
    [RequireComponent(typeof(ProjectileController))]
    public class OrbitProjectile : NetworkBehaviour, IProjectileImpactBehavior
    {
        //havent fully implemented this class yet
        //very unfinished code

        public float angularVelocity = 40f;
        public float maxTime = 1f; //temp
        public float maxRadius = 1f;
        public float maxRadiusOrbits = 1f;
        public float charge;

        public GameObject impactSpark;
        public bool canHitCharacters;
        public bool canHitWorld;
        private ProjectileController projectileController;
        private Transform ownerTransform;

        [SyncVar]
        private OrbitProjectile.OrbitState orbitState;

        //private Transform ownerTransform;
        private ProjectileDamage projectileDamage;
        private Rigidbody rigidbody;
        private float stopwatch;
        private float fireAge;
        private float fireFrequency;
        //public UnityEvent onFlyBack;
        private bool setScale;

        public enum OrbitState
        {
            FlyOut,
            MaxOrbit
            //FlyBack
        }

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            projectileController = GetComponent<ProjectileController>();
            projectileDamage = GetComponent<ProjectileDamage>();
            if (projectileController && projectileController.owner)
            {
                ownerTransform = projectileController.owner.transform;
            }
        }

        private void Start()
        {
            float num = charge * 7f;
            if (num < 1f)
                num = 1f;
            Vector3 localScale = new Vector3(num * transform.localScale.x, num * transform.localScale.y, num * transform.localScale.z);
            transform.localScale = localScale;
            gameObject.GetComponent<ProjectileController>().ghost.transform.localScale = localScale;
            GetComponent<ProjectileDotZone>().damageCoefficient *= num;
        }
        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (!canHitWorld)
                return;
            EffectManager.SimpleImpactEffect(impactSpark, impactInfo.estimatedPointOfImpact, -transform.forward, true);
            Destroy(gameObject);
        }

        public void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                if (!setScale)
                    setScale = true;
                if (!projectileController.owner)
                {
                    Destroy(base.gameObject);
                    return;
                }
                //float tanVelocity = angularVelocity * maxRadius;
                switch (this.orbitState)
                {
                    case OrbitState.FlyOut: //Placeholder code
                        if (NetworkServer.active)
                        {
                            float travelSpeed = maxRadius / maxTime;
                            rigidbody.velocity = travelSpeed * transform.right;
                            stopwatch += Time.fixedDeltaTime;
                            if (this.stopwatch >= maxTime)
                            {
                                stopwatch = 0f;
                                travelSpeed = 0f;
                                NetworkorbitState = OrbitProjectile.OrbitState.MaxOrbit;
                                return;
                            }
                        }
                        break;
                    case OrbitState.MaxOrbit: //not done
                        stopwatch += Time.fixedDeltaTime;
                        float tanVelocity = angularVelocity * maxRadius; //tangent velocity
                        Destroy(gameObject);
                        break;
                }
            }
        }


        //I have no idea what any of this does. I copied off of RoR2.Projectile.BoomerangProjectile
        private void UNetVersion()
        {
        }

        public OrbitProjectile.OrbitState NetworkorbitState
        {
            get
            {
                return this.orbitState;
            }
            [param: In]
            set
            {
                ulong newValueAsUlong = (ulong)((long)value);
                ulong fieldValueAsUlong = (ulong)((long)this.orbitState);
                base.SetSyncVarEnum<OrbitProjectile.OrbitState>(value, newValueAsUlong, ref this.orbitState, fieldValueAsUlong, 1U);
            }
        }

        public override bool OnSerialize(NetworkWriter writer, bool forceAll)
        {
            if (forceAll)
            {
                writer.Write((int)orbitState);
                return true;
            }
            bool flag = false;
            if((base.syncVarDirtyBits & 1U) != 0U)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write((int)this.orbitState);
            }
            if (!flag)
            {
                writer.WritePackedUInt32(base.syncVarDirtyBits);
            }
            return flag;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                this.orbitState = (OrbitProjectile.OrbitState)reader.ReadInt32();
                return;
            }
            int num = (int)reader.ReadPackedUInt32();
            if((num & 1) != 0)
            {
                this.orbitState = (OrbitProjectile.OrbitState)reader.ReadInt32();
            }
        }

        public override void PreStartClient()
        {
        }
    }
}
