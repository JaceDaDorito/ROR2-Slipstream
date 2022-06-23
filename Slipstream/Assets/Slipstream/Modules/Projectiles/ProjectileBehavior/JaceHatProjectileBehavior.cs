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
    /*[DisallowMultipleComponent]
    [RequireComponent(typeof(ProjectileController))]
    public class JaceHatProjectileBehavior : NetworkBehaviour, IProjectileImpactBehavior
    {
        //I intend to make this behavior modular so we can apply it on other things if we need to
        //this script is really bad i know

        private float radius;
        private float initialRadius;
        private float stopwatch;
        private ProjectileOwnerOrbiter orbiter;
        private ProjectileController projectileController;
        private CharacterBody body;
        private float maxTime;
        public bool canHitWorld;

        public void OnEnable()
        {
            if (NetworkServer.active)
            {
                projectileController = base.GetComponent<ProjectileController>();
                body = projectileController.owner.GetComponent<CharacterBody>();
                orbiter = base.GetComponent<ProjectileOwnerOrbiter>();
                initialRadius = JaceHat.initialRadius;
                radius = JaceHat.maxRadius - initialRadius;
                maxTime = 360f / orbiter.degreesPerSecond; //always results in only doing 1 orbit
            }
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active) {
                if(orbiter && projectileController)
                {
                    stopwatch += Time.fixedDeltaTime;
                    orbiter.radius = radius * (stopwatch / maxTime) + initialRadius + body.radius;
                    if (stopwatch >= maxTime)
                    {
                        stopwatch = 0f;
                        Destroy(gameObject);
                    }
                }
            }
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (!this.canHitWorld)
            {
                return;
            }
            Chat.AddMessage("<color=#20fa5a>collision</color>");
            orbiter.degreesPerSecond *= -1f;
        }
    }*/
}
