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

namespace Slipstream.Buffs
{
    public class AffixSandswept : BuffBase
    {
        public override BuffDef BuffDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<BuffDef>("AffixSandswept");
        public static BuffDef buff;
        //public BuffDef grainDebuff = SlipAssets.Instance.MainAssetBundle.LoadAsset<BuffDef>("Grainy");

        private static GameObject knockbackExplosion = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("EliteSandKnockback");

        private static GameObject sandsweptMissile = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("SandsweptMissile");

        public EntityStateMachine targetStateMachine;

        public static float timeInvulnerable = 1f;

        public static float upwardForce = 5000f;

        public static float outwardForce = 5500f;

        public static int buffCount = 4;
        public static float debuffUpwardForce = 4000f;
        public static float debuffProc = 100f;

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

        #region proj
        //public static float projectileVel = 20f;
        //public static float minimumDistance = 10f;
        //public static float timeToTarget = 3f;

        //public static float missileInterval = 8f;
        //public static float maxSeekDistance = 40f;

        //public static DamageAPI.ModdedDamageType sandDamageType;

        //public static float differenceForceMultiplier = 1.5f;
        //public static float secondForce = 4700f;
        #endregion
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
            //sandDamageType = DamageAPI.ReserveDamageType();

            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            IL.EntityStates.GenericCharacterDeath.OnEnter += GenericCharacterDeath_OnEnter; //IL hook, you hook onto a method like any other hook.
            //IL.RoR2.CharacterModel.UpdateRendererMaterials += CharacterModel_UpdateRendererMaterials;
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
                //If the body just died, is Sandswept, isn't in the glass state, and didn't die to the void implosions, make body into glass state.
                if (!body.healthComponent.alive && body.HasBuff(BuffDef.buffIndex) && !body.GetComponent<AffixSandsweptBehavior>().isGlass && ((obj.damageInfo.damageType & DamageType.VoidDeath) <= 0))
                {
                    Util.CleanseBody(body, true, false, true, true, true, false);

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

                    GlassState state;
                    if(body.isFlying/* || (body.characterMotor && !body.characterMotor.isGrounded)*/)
                        deathBehaviour.deathStateMachine.SetNextState(state = new AirGlassState());
                    else
                        deathBehaviour.deathStateMachine.SetNextState(state = new GlassState());
                    state.attackerBody = obj.attackerBody; //Records the attacker who put this body into the glass state to give them money later if the body suicides

                    /*EntityStateMachine[] array = deathBehaviour.idleStateMachine;
                    for( int i = 0; i < array.Length; i++)
                    {
                        array[i].SetNextState(new Idle());
                    }*/
                       
                }
                //If the body just died, is Sandswept, and is glass OR died to void implosions, do knockback explosion.
                //I would do this check in the entity state but I wanted the entity state to be skipped if they died to a void implosion.
                else if (!body.healthComponent.alive && body.HasBuff(BuffDef.buffIndex) && (body.GetComponent<AffixSandsweptBehavior>().isGlass || ((obj.damageInfo.damageType & DamageType.VoidDeath) > 0)))
                {
                    //This grants money to the attacker anyway
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
                    hullRadius = 7.5f;
                    break;
                case HullClassification.Golem:
                    hullRadius = 15f;
                    break;
                default:
                    hullRadius = 22.5f;
                    break;
            }
            return hullRadius + (body.bestFitRadius / 2f);
        }

        public static void DisableColliders(CharacterBody body)
        {
            Collider[] collider = body.gameObject.GetComponents<Collider>();
            if (collider.Length > 0)
            {
                for (int i = 0; i < collider.Length; i++)
                    collider[i].enabled = false;

            }
        }

        public static void FireKBBlast(CharacterBody body)
        {
            SlipLogger.LogD("Fired Sandswept Glass Explosion");
            //Util.PlayAttackSpeedSound("Play_char_glass_death", body.gameObject, 2f);
            //Vector3 corePosition = RoR2.Util.GetCorePosition(body.gameObject);

            Vector3 feetPosition = body.footPosition;
            float combinedRadius = CalculateRadius(body);

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

            //All of these knockbacks were previous iterations of the knockback. Keeping them here for archival purposes.
            #region UpwardBlast
            /*
            //Bullseye search to get bodies that are grounded
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.teamMaskFilter = TeamMask.all;
            bullseyeSearch.filterByDistinctEntity = true;
            bullseyeSearch.filterByLoS = true;
            bullseyeSearch.maxDistanceFilter = combinedRadius;
            bullseyeSearch.searchOrigin = feetPosition;
            bullseyeSearch.searchDirection = body.transform.forward;
            bullseyeSearch.RefreshCandidates();

            SlipLogger.LogD("Sandswept Bullseye Search Creation");
            IEnumerable<HurtBox> list = bullseyeSearch.GetResults();
            SlipLogger.LogD("Sandswept Bullseye Search Results");
            foreach (HurtBox hurtbox in list)
            {
                CharacterBody indexBody = hurtbox.healthComponent.body;
                //if the body has a motor and is grounded or at a certain height below the core of the explosion, launch the body
                if (indexBody && indexBody.characterMotor && indexBody != body)
                {
                    Vector3 result = indexBody.corePosition - feetPosition;
                    result = result.normalized;

                    //indexBody.characterMotor.
                    PhysForceInfo physInfo = new PhysForceInfo()
                    {
                        //should hopefully scale the force needed to lift someone above the core
                        force = new Vector3(result.x * outwardForce, upwardForce, result.z * outwardForce),
                        ignoreGroundStick = true,
                        disableAirControlUntilCollision = false,
                        massIsOne = true
                    };
                    indexBody.characterMotor.ApplyForceImpulse(physInfo);
                }
            }*/
            #endregion
            //Util.CharacterSpherecast

            #region BlastAttackKB
            //RaycastHit hit;
            //bool isRaycastExplosion = Physics.Raycast(corePosition, Vector3.down, out hit, combinedRadius * 2f, LayerIndex.world.mask);

            //blastAttack that launches you into the air
            /*BlastAttack blastAttack = new BlastAttack();
            blastAttack.radius = combinedRadius;
            blastAttack.position = corePosition;
            blastAttack.attacker = body.gameObject;
            blastAttack.falloffModel = BlastAttack.FalloffModel.None;
            blastAttack.damageType = DamageType.Silent;
            blastAttack.baseForce = 0f;
            blastAttack.bonusForce.y = 4000f;
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
            blastAttack.attackerFiltering = AttackerFiltering.AlwaysHit;
            blastAttack.Fire();*/
            #endregion

            #region 2PartKB
            //Disables all colliders when fired
            /*
            Collider[] collider = body.gameObject.GetComponents<Collider>();
            if (collider.Length > 0)
            {
                for (int i = 0; i < collider.Length; i++)
                    collider[i].enabled = false;

            }


            //Bullseye search to get bodies that are grounded
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.teamMaskFilter = TeamMask.all;
            bullseyeSearch.filterByDistinctEntity = true;
            bullseyeSearch.filterByLoS = true;
            bullseyeSearch.maxDistanceFilter = combinedRadius;
            bullseyeSearch.searchOrigin = corePosition;
            bullseyeSearch.searchDirection = body.transform.forward;
            bullseyeSearch.RefreshCandidates();

            SlipLogger.LogD("Sandswept Bullseye Search Creation");
            IEnumerable<HurtBox> list = bullseyeSearch.GetResults();
            SlipLogger.LogD("Sandswept Bullseye Search Results");
            foreach (HurtBox hurtbox in list)
            {
                CharacterBody indexBody = hurtbox.healthComponent.body;
                float difference = Mathf.Max(0, corePosition.y - indexBody.corePosition.y);
                //if the body has a motor and is grounded or at a certain height below the core of the explosion, launch the body
                if (indexBody && indexBody.characterMotor && (indexBody.characterMotor.isGrounded || difference > 0) && indexBody != body)
                {
                    PhysForceInfo physInfo = new PhysForceInfo()
                    {
                        //should hopefully scale the force needed to lift someone above the core
                        force = Vector3.up * (initialConstantForce + (difference * differenceForceMultiplier)),
                        ignoreGroundStick = true,
                        disableAirControlUntilCollision = false,
                        massIsOne = true
                    };
                    indexBody.characterMotor.ApplyForceImpulse(physInfo);
                }
            }

            GameObject knockbackExplosionInstance = UnityEngine.Object.Instantiate<GameObject>(knockbackExplosion, corePosition, Quaternion.identity);
            DelayBlast delayBlast = knockbackExplosionInstance.GetComponent<DelayBlast>();
            if (delayBlast)
            {
                delayBlast.position = corePosition;
                delayBlast.attacker = body.gameObject;
                delayBlast.falloffModel = BlastAttack.FalloffModel.None;
                delayBlast.damageType = DamageType.Silent;
                delayBlast.baseForce = secondForce;
                delayBlast.radius = combinedRadius;
                delayBlast.maxTimer = 0.25f;
            }
            NetworkServer.Spawn(knockbackExplosionInstance);*/
            #endregion
        }

        private void GenericCharacterDeath_OnEnter(MonoMod.Cil.ILContext il)
        {
            ILLabel returnLabel = null; //make sure to declare a label and give it a value
            ILCursor c = new ILCursor(il); //make a new cursor
            bool found = c.TryGotoNext( //These lines search for an approppriate place to put the cursor. So "Match(whatever)" is matching to where we are injecting code.
                MoveType.After,         //Move.After basically tells the cursor to go after the lines.
                x => x.MatchLdarg(0),
                x => x.MatchCall<GenericCharacterDeath>("get_isVoidDeath"),
                x => x.MatchBrtrue(out returnLabel));
            if (found) //If theres a match, inject our own code.
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<GenericCharacterDeath, bool>>((gdc) => gdc.characterBody && gdc.characterBody.HasBuff(BuffDef));
                c.Emit(OpCodes.Brtrue, returnLabel);
            }
            else
                SlipLogger.LogW($"Couldn't find Void Death location. Can't inject Sandswept death IL code");

        }

        /*private void CharacterModel_UpdateRendererMaterials(ILContext il)
        {
            ILLabel returnLabel = null;
            ILCursor c = new ILCursor(il);
            bool found = c.TryGotoNext(
                MoveType.After,
                x => x.MatchLdarg(0),
                x => x.MatchCall<CharacterModel>("IsGummyClone"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdsfld(nameof(CharacterModel), nameof(CharacterModel.gummyCloneMaterial)),
                x => x.MatchStloc(0),
                x => x.MatchBr(out returnLabel));

        }*/

        public class AffixSandsweptBehavior : BaseBuffBodyBehavior/*, IOnIncomingDamageOtherServerReciever*/, IOnIncomingDamageServerReceiver, IBodyStatArgModifier, IOnDamageDealtServerReceiver
        {
            [BuffDefAssociation(useOnClient = true, useOnServer = true)]
            public static BuffDef GetBuffDef() => SlipContent.Buffs.AffixSandswept;

            public Run.FixedTimeStamp lastBecameGlass;
            public bool isGlass = false;

            #region oldProjectile
            //private float timer = 0f;
            //private bool cachedFire = false;

            //BullseyeSearch bullseyeSearch = new BullseyeSearch();

            /*public void OnEnable()
            {
                bullseyeSearch.teamMaskFilter = TeamMask.GetEnemyTeams(body.teamComponent.teamIndex);
                bullseyeSearch.filterByDistinctEntity = true;
                bullseyeSearch.filterByLoS = true;
                bullseyeSearch.maxDistanceFilter = maxSeekDistance;
                
            }*/

            /*public void FixedUpdate()
            {
                if (!isGlass)
                {
                    timer += Time.deltaTime;
                    if(timer >= missileInterval || cachedFire)
                    {
                        cachedFire = true;
                        bullseyeSearch.searchOrigin = body.corePosition;
                        bullseyeSearch.searchDirection = body.transform.forward;
                        bullseyeSearch.RefreshCandidates();
                        HurtBox hurtbox = bullseyeSearch.GetResults().First<HurtBox>();

                        if (hurtbox && hurtbox.healthComponent.body)
                        {
                            cachedFire = false;
                            timer = 0f;
                            FireSandMotar(hurtbox.healthComponent.body);
                        }                    
                    }
                }
            }*/

            /*public void FireSandMotar(CharacterBody target)
            {
                GameObject projectilePrefab = sandsweptMissile;
                DamageAPI.ModdedDamageTypeHolderComponent holder = projectilePrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
                holder.Add(sandDamageType);
                bool isCrit = Util.CheckRoll(body.crit, body.master);

                Ray ray = new Ray(body.corePosition, Vector3.up);
                Vector3 targetPos = target.corePosition;
                Vector3 vector = targetPos - ray.origin;
                Vector2 a = new Vector2(vector.x, vector.z);
                float distanceMagnitude = a.magnitude;
                Vector3 vector2 = a /distanceMagnitude;

                float y = Trajectory.CalculateInitialYSpeed(timeToTarget, vector.y);
                float num = distanceMagnitude / timeToTarget;
                Vector3 direction = new Vector3(vector2.x * num, y, vector2.y * num);
                distanceMagnitude = direction.magnitude;
                ray.direction = direction;

                ProjectileManager.instance.FireProjectile(projectilePrefab, ray.origin, Quaternion.identity, body.gameObject, body.damage, 0f, isCrit, DamageColorIndex.Item, null, distanceMagnitude);
            }*/


            /*public void OnIncomingDamageOther(HealthComponent victimHealthComponent, DamageInfo damageInfo)
            {
                damageInfo.damageType |= DamageType.Stun1s;
            }

            public void OnDamageDealtServer(DamageReport damageReport)
            {
                DamageInfo damageInfo = damageReport.damageInfo;
                CharacterBody victimBody = damageReport.victimBody;
                if (damageInfo.HasModdedDamageType(sandDamageType) && !victimBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.Mechanical))
                {
                    KinematicCharacterMotor indexKCC = victimBody.GetComponent<KinematicCharacterMotor>();
                    if (indexKCC)
                        indexKCC.ForceUnground();

                    PhysForceInfo physInfo = new PhysForceInfo()
                    {
                        force = Vector3.up * projUpwardForce,
                        ignoreGroundStick = true,
                        disableAirControlUntilCollision = false,
                        massIsOne = false//MassIsOne just means if mass will have a factor in the force. I set it to false because I want the mass to contribute.
                    };
                    victimBody.characterMotor.ApplyForceImpulse(physInfo);
                }
            }*/


            #endregion
            public void OnIncomingDamageServer(DamageInfo damageInfo)
            {
                if (isGlass && lastBecameGlass.timeSince <= timeInvulnerable)
                    damageInfo.rejected = true;
            }

            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                if (isGlass)
                    args.baseCurseAdd += body.healthComponent.combinedHealth;
            }

            public void OnDamageDealtServer(DamageReport damageReport)
            {
                //DamageInfo damageInfo = damageReport.damageInfo;
                CharacterBody victimBody = damageReport.victimBody;
                if (victimBody && Util.CheckRoll(damageReport.damageInfo.procCoefficient * debuffProc, damageReport.attackerBody.master))
                {
                    victimBody.AddBuff(Grainy.buff);
                    int count = victimBody.GetBuffCount(Grainy.buff);
                    if (victimBody.GetBuffCount(Grainy.buff) >= buffCount)
                    {
                        GenericUtils.RemoveStacksOfBuff(victimBody, Grainy.buff, count);
                        Explode(victimBody);
                    }
                }
            }

            public void Explode(CharacterBody victim)
            {
                KinematicCharacterMotor indexKCC = victim.GetComponent<KinematicCharacterMotor>();
                if (indexKCC)
                    indexKCC.ForceUnground();

                if (!BlacklistedBodyIndices.Contains(victim.bodyIndex))
                {
                    PhysForceInfo physInfo = new PhysForceInfo()
                    {
                        force = Vector3.up * debuffUpwardForce,
                        ignoreGroundStick = true,
                        disableAirControlUntilCollision = false,
                        massIsOne = false //MassIsOne just means if mass will have a factor in the force. I set it to false because I want the mass to contribute.
                    };
                    victim.characterMotor.ApplyForceImpulse(physInfo);
                }
                
            }
        }
    }
}