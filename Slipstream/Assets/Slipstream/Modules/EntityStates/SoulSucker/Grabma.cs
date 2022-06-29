using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using RoR2.Orbs;
using EntityStates.Treebot.TreebotFlower;

namespace EntityStates.SoulSucker
{
    public class Grabma : SoulSuckerBaseState
    {
        public static float yankIdealDistance = 6f;

        public static float orbInterval = 0.3f;
        private float orbStopwatch;
        public override void OnEnter()
        {
            base.OnEnter();
            //closing animation 
            if (NetworkServer.active && victim && victim.body)
            {
                victim.body.AddBuff(Slipstream.SlipContent.Buffs.SoulRoot);
                victim.body.RecalculateStats();
                Vector3 a = victim.body.corePosition - base.transform.position;
                float magnitude = a.magnitude;
                Vector3 a2 = a / magnitude;
                Rigidbody rigidBody = victim.GetComponent<Rigidbody>();
                float num = rigidBody ? rigidBody.mass : 1f;
                float num2 = magnitude - yankIdealDistance;
                float num3 = TreebotFlower2Projectile.yankSuitabilityCurve.Evaluate(num);
                Vector3 vector = rigidBody ? rigidBody.velocity : Vector3.zero;
                if (HGMath.IsVectorNaN(vector))
                {
                    vector = Vector3.zero;
                }
                Vector3 a3 = -vector;
                if (num2 > 0f)
                {
                    a3 = a2 * -Trajectory.CalculateInitialYSpeedForHeight(num2, -victim.body.acceleration);
                }
                Vector3 force = a3 * (num * num3);
               
                victim.TakeDamageForce(force, false, false);
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active)
            {
                FixedUpdateServer();
            }
        }
        public void FixedUpdateServer()
        {
            if (!victim || !victim.alive)
            {
                this.outer.SetNextState(new ResetTrap());
                return;
            }
            //ApplyForce();
            orbStopwatch += Time.fixedDeltaTime;
            if (orbStopwatch >= orbInterval)
            {
                orbStopwatch -= orbInterval;
                FireSoulOrb();
            }
        }
        public void FireSoulOrb()
        {
            if (!victim.body)
            {
                return;
            }
            SoulOrb soulOrb = new SoulOrb();
            soulOrb.origin = base.transform.position;
            soulOrb.damageValue = -victim.body.regen * orbInterval;
            soulOrb.teamIndex = teamIndex;
            soulOrb.attacker = owner;
            soulOrb.inflictor = base.gameObject;
            soulOrb.damageColorIndex = DamageColorIndex.Poison;
            soulOrb.scale = 1f;
            HurtBox hurtBox = soulOrb.PickNextTarget(soulOrb.origin, 30f, victim);
            if (hurtBox)
            {
                soulOrb.target = hurtBox;
                soulOrb.isCrit = ownerBody ? ownerBody.RollCrit() : false;
                OrbManager.instance.AddOrb(soulOrb);
            }
        }
        public void ApplyForce()
        {
            if (!victim.body)
            {
                return;
            }
            CharacterMotor characterMotor = victim.body.characterMotor;
            Vector3 direction = this.transform.position - victim.body.corePosition;
            float distanceMultiplier = Mathf.Clamp01(direction.sqrMagnitude / (grabRadius * grabRadius));
            direction = direction.normalized * 50f;
            Vector3 velocity = Vector3.zero;
            float mass = 1f;
            if (characterMotor)
            {
                velocity = characterMotor.velocity;
                mass = characterMotor.mass;
            }
            else
            {
                Rigidbody rigidbody = victim.body.rigidbody;
                if (rigidbody)
                {
                    velocity = rigidbody.velocity;
                    mass = rigidbody.mass;
                }
            }
            velocity.y += Physics.gravity.y * Time.fixedDeltaTime;
            victim.TakeDamageForce((direction - velocity) * mass * distanceMultiplier, true, false);
        }
        public override void OnExit()
        {
            if (NetworkServer.active && victim && victim.body)
            {
                victim.body.RemoveBuff(Slipstream.SlipContent.Buffs.SoulRoot);
            }
            base.OnExit();
        }
    }
}
