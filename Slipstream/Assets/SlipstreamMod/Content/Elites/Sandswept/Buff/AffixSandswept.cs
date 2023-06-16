using Moonstorm;
using Moonstorm.Components;
using R2API;
using RoR2;
using System.Linq;
using UnityEngine;
using EntityStates.Sandswept;
using EntityStates;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using Slipstream.Components;
using KinematicCharacterController;
using RoR2.Projectile;
using System.Collections.ObjectModel;
using Slipstream.Orbs;
using RoR2.Orbs;

namespace Slipstream.Buffs
{

    /* SANDSWEPT DOCUMENTATION BECAUSE THIS ELITE IS COMPLICATED
     * ----------------------
     * 
     * So Sandswepts have 5 ways of "dying": 4 of which involve the glass state and the last one skipping the glass state.
     * I want to add one more way of dying of which it dies to the kill console command but its not a big deal.
     * Each death will be logged via console, fires the method FireKBBlast(), and gives an orb if a valid target has the grainy debuff.
     * 
     * Each Death is as follows:
     * 
     * Damage - Only happens once the elite is in the glass state and takes another instance of damage. Also allows some aerial enemies to take fall damage and die.
     * (Found in this script under GlobalEventManager_onServerDamageDealt)
     * 
     * Collision - Only applies to aerial elites of which they fall to the ground and die when they are in the glass state. Triggered when the collider hits the ground.
     * (Found in the DieOnCollision script)
     * 
     * Raycast - Same as Collision except that its triggered when the footposition of the elite is close enough to the ground. The point of this is because some enemies
     * don't collide with the ground or take fall damage (vultures and pests).
     * (Found in the AirGlassState script under FixedUpdate)
     * 
     * Age - When the elite goes into the glass state, it has a set timer before it explodes by itself.
     * (Found in the GlassState script under FixedUpdate)
     * 
     * Void Damage - This is the one that skips the glass state entirely and directly fires the explosion. This applies to any void enemy that has void damage as well as
     * lost seers lenses.
     * (Found in this script under GlobalEventManager_onServerDamageDealt)
     * 
     * 
     */
    public class AffixSandswept : BuffBase
    {
        public override BuffDef BuffDef { get; } = SlipAssets.LoadAsset<BuffDef>("AffixSandswept", SlipBundle.Elites);
        public static BuffDef buff;
        //public BuffDef grainDebuff = SlipAssets.Instance.MainAssetBundle.LoadAsset<BuffDef>("Grainy");

        private static GameObject knockbackExplosion = SlipAssets.LoadAsset<GameObject>("EliteSandKnockback", SlipBundle.Elites);

        private static GameObject eliteKnockbackVisual = SlipAssets.LoadAsset<GameObject>("EliteSandKnockbackVisual", SlipBundle.Elites);

        public EntityStateMachine targetStateMachine;

        public static float timeInvulnerable = 1f;

        public static float upwardForce = 5000f;

        public static float outwardForce = 5500f;

        public static float freezeDuration = 30f;

        public static int buffCount = 5;
        public static int buffsApplied = 2;
        public static float debuffUpwardForce = 4000f;
        public static float debuffProc = 100f;
        
        //As you would imagine, being able to fucking cut 40% of everyone's health with Wake is... very busted.
        public static float chippedPercentage = 0.4f;
        public static float nerfedChippedPercentage = 0.2f;

        public static float chippedDuration = 60f;

        public GlassState state;

        //List of character bodies sandswept knockback doesn't effect

        private static readonly List<BodyIndex> blacklistedBodyIndices = new List<BodyIndex>();
        //public so the delayedupwardblast can get this blacklist
        public static ReadOnlyCollection<BodyIndex> BlacklistedBodyIndices = new ReadOnlyCollection<BodyIndex>(blacklistedBodyIndices);

        //Modders with custom drones should add to this blacklist with a soft dependency
        public static List<string> blacklistedBodies = new List<string>()
        {
            "DroneBackupBody",
            "Drone1Body",
            "Drone2Body",
            "EmergencyDroneBody",
            "EquipmentDroneBody",
            "FlameDroneBody",
            "MegaDroneBody",
            "DroneMissileBody",
            "EngiWalkerTurretBody",
            "RoboBallRedBuddyBody",
            "RoboBallGreenBuddyBody"
        };

        [SystemInitializer(typeof(BodyCatalog))]
        private static void SystemInit()
        {
            foreach (string bodyName in blacklistedBodies)
            {
                BodyIndex bodyIndex = BodyCatalog.FindBodyIndex(bodyName);
                if (bodyIndex != BodyIndex.None)
                {
                    AddBodyToBlacklist(bodyIndex);
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            buff = BuffDef;

            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            //Moved death IL hook to SlipHooks because there is another condition for void death adjacent
        }

        public static void AddBodyToBlacklist(BodyIndex bodyIndex)
        {
            if (bodyIndex == BodyIndex.None)
            {
                SlipLogger.LogD($"Tried to add a master to the blacklist, but it's index is none.");
                return;
            }

            if (blacklistedBodyIndices.Contains(bodyIndex))
            {
                GameObject prefab = BodyCatalog.GetBodyPrefab(bodyIndex);
                SlipLogger.LogD($"Master PRefab {prefab} is already blacklisted.");
                return;
            }

            blacklistedBodyIndices.Add(bodyIndex);
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport obj)
        {
            CharacterBody body = obj.victimBody;
            if (body.healthComponent)
            {
                //If the body died to void damage, just kill it and fire the knockback
                if (body.HasBuff(BuffDef.buffIndex) && ((obj.damageInfo.damageType & DamageType.VoidDeath) > 0))
                {
                    SlipLogger.LogD(body + " glass statue died to Void Damage");
                    FireKBBlast(body);
                }
                //If the body just died, is Sandswept, isn't in the glass state, make body into glass state.
                else if (!body.healthComponent.alive && body.HasBuff(BuffDef.buffIndex) && !body.GetComponent<AffixSandsweptBehavior>().isGlass/* && ((obj.damageInfo.damageType & DamageType.VoidDeath) <= 0)*/)
                {
                    body.healthComponent.Networkbarrier = 0f;
                    body.healthComponent.Networkhealth = 1f;
                    body.healthComponent.Networkshield = 0f;               
                    body.MarkAllStatsDirty();

                    CharacterDeathBehavior deathBehaviour = body.GetComponent<CharacterDeathBehavior>();
                    body.GetComponent<AffixSandsweptBehavior>().isGlass = true;
                    body.GetComponent<AffixSandsweptBehavior>().lastBecameGlass = Run.FixedTimeStamp.now;
                    //deathBehaviour.deathState = new EntityStates.SerializableEntityStateType(typeof(SandsweptGlassShatter));
                    EntityStateMachine[] machineArray = body.gameObject.GetComponents<EntityStateMachine>();
                    for(int i = 0; i < machineArray.Length; i++)
                    {
                        machineArray[i].SetNextStateToMain();
                    }

                    if(body.isFlying && (body.rigidbody != null || body.characterMotor != null)/* || (body.characterMotor && !body.characterMotor.isGrounded)*/)
                    {
                        deathBehaviour.deathStateMachine.SetNextState(state = new AirGlassState());
                        state.isAirState = true;
                    }
                    else
                        deathBehaviour.deathStateMachine.SetNextState(state = new GlassState());
                    if (obj.attacker)
                        state.attackerBody = obj.attackerBody; //Records the attacker who put this body into the glass state to give them money later if the body suicides
                       
                }
                //If the body Sandswept, and is glass, do knockback explosion. Doesn't matter if the body is alive or not, if its glass its glass
                else if (body.HasBuff(BuffDef.buffIndex) && body.GetComponent<AffixSandsweptBehavior>().isGlass)
                {

                    if (body.healthComponent.alive && obj.attackerBody)
                        body.healthComponent.Suicide(obj.attackerBody?.gameObject/*, null, obj.damageInfo.damageType*/);
                    else
                        body.healthComponent.Suicide();

                    //This grants money to the attacker anyway
                    if (state.isAirState)
                    {
                        //somtimes flying enemies die to the fall damage
                        DieOnCollision doc = body.gameObject.GetComponent<DieOnCollision>();
                        doc.collided = true;
                    }
                    AffixSandswept.CreateOrb(body, obj.attackerBody);
                    SlipLogger.LogD(body + " glass statue died to Damage");
                    FireKBBlast(body);
                }
            }        
        }

        public static float CalculateRadius(CharacterBody body)
        {
            //Not sure how to make the radius feel nice without it feeling scuffed.

            float hullRadius;
            switch (body.hullClassification)
            {
                case HullClassification.Human:
                    hullRadius = 10.5f;
                    break;
                case HullClassification.Golem:
                    hullRadius = 16f;
                    break;
                default:
                    hullRadius = 22.5f;
                    break;
            }
            return hullRadius + (body.bestFitRadius / 2f);
        }


        public static void CreateOrb(CharacterBody source, CharacterBody passedTarget)
        {
            CharacterBody leader = passedTarget.master.minionOwnership?.ownerMaster?.GetBody();
            CharacterBody target = null;
            MinionOwnership.MinionGroup minionGroup = null;

            if(leader != null)
                minionGroup = MinionOwnership.MinionGroup.FindGroup(leader.master.netId);
            else
                minionGroup = MinionOwnership.MinionGroup.FindGroup(passedTarget.master.netId);

            if (leader != null && leader.HasBuff(Grainy.buff))
                target = leader;
            else if (passedTarget.HasBuff(Grainy.buff))
                target = passedTarget;
            else if(minionGroup != null)
            {
                foreach(MinionOwnership minionOwnership in minionGroup.members)
                {
                    if (minionOwnership)
                    {
                        CharacterBody minionBody = minionOwnership?.GetComponent<CharacterMaster>()?.GetBody();
                        if (minionBody && minionBody.HasBuff(Grainy.buff))
                        {
                            target = minionBody;
                        }
                    }
                }
            }

            if (target != null)
            {
                SandsweptDeathOrb sandsweptDeathOrb = new SandsweptDeathOrb();
                sandsweptDeathOrb.origin = source.corePosition;
                sandsweptDeathOrb.target = Util.FindBodyMainHurtBox(target);
                OrbManager.instance.AddOrb(sandsweptDeathOrb);
            }
        }

        public static void FireKBBlast(CharacterBody body)
        {
            SlipLogger.LogD("Fired Sandswept Glass Explosion");

            Vector3 feetPosition = body.footPosition;
            float combinedRadius = CalculateRadius(body);

            EffectManager.SpawnEffect(eliteKnockbackVisual, new EffectData
            {
                origin = feetPosition,
                scale = combinedRadius
            }, true);

            GameObject explosion = UnityEngine.Object.Instantiate(knockbackExplosion, feetPosition, Quaternion.identity);
            DelayedUpwardBlast blast = explosion.GetComponent<DelayedUpwardBlast>();
            if (blast)
            {
                blast.combinedRadius = combinedRadius;
                blast.position = feetPosition;
                blast.searchDirection = body.transform.forward;
                blast.outwardForce = outwardForce;
                blast.upwardForce = upwardForce;
                blast.time = 0.12f;
                blast.team = body.teamComponent.teamIndex;

                NetworkServer.Spawn(explosion);
            }
            else
                SlipLogger.LogE("Couldn't fire upward blast in AffixSandswept FireKBBlast");
        }

        public class AffixSandsweptBehavior : BaseBuffBodyBehavior, IOnIncomingDamageServerReceiver, IOnDamageDealtServerReceiver
        {
            [BuffDefAssociation(useOnClient = true, useOnServer = true)]
            public static BuffDef GetBuffDef() => SlipContent.Buffs.AffixSandswept;

            public Run.FixedTimeStamp lastBecameGlass;
            public bool isGlass = false;
            public void OnIncomingDamageServer(DamageInfo damageInfo)
            {
                if (isGlass && lastBecameGlass.timeSince <= timeInvulnerable)
                    damageInfo.rejected = true;
            }


            public void OnDamageDealtServer(DamageReport damageReport)
            {
                //DamageInfo damageInfo = damageReport.damageInfo;
                CharacterBody victimBody = damageReport.victimBody;
                if (victimBody && !victimBody.HasBuff(Chipped.buff) && Util.CheckRoll(damageReport.damageInfo.procCoefficient * debuffProc, damageReport.attackerBody.master))
                {
                    SlipUtils.AddStacksOfBuff(victimBody, Grainy.buff, buffsApplied);
                    int count = victimBody.GetBuffCount(Grainy.buff);
                    if (victimBody.GetBuffCount(Grainy.buff) > buffCount)
                    {
                        victimBody.AddTimedBuff(Chipped.buff, chippedDuration);
                        SlipUtils.RemoveStacksOfBuff(victimBody, Grainy.buff, count);
                        Util.PlaySound("Play_merc_sword_impact", body.gameObject);
                    }
                }
            }
        }
    }
}