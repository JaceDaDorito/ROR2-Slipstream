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

namespace Slipstream.Buffs
{
    public class AffixSandswept : BuffBase
    {
        public override BuffDef BuffDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<BuffDef>("AffixSandswept");

        private static GameObject knockbackExplosion = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("EliteSandKnockback");

        public EntityStateMachine targetStateMachine;

        public static float timeInvulnerable = 1f;

        public static float upwardForce = 40f;
        public static float outwardForce = 40f;
        //public static float differenceForceMultiplier = 1.5f;
        //public static float secondForce = 4700f;

        public override void Initialize()
        {
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            IL.EntityStates.GenericCharacterDeath.OnEnter += GenericCharacterDeath_OnEnter; //IL hook, you hook onto a method like any other hook.
            //IL.RoR2.CharacterModel.UpdateRendererMaterials += CharacterModel_UpdateRendererMaterials;
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport obj)
        {
            CharacterBody body = obj.victimBody;
            if (body.healthComponent)
            {
                //If the body just died, is Sandswept, isn't in the glass state, and didn't die to the void implosions, make body into glass state.
                if (!body.healthComponent.alive && body.HasBuff(BuffDef.buffIndex) && !body.GetComponent<AffixSandsweptBehavior>().isGlass && ((obj.damageInfo.damageType & DamageType.VoidDeath) <= 0))
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

                    if(body.isFlying || (body.characterMotor && !body.characterMotor.isGrounded))
                        deathBehaviour.deathStateMachine.SetNextState(new AirGlassState());
                    else
                        deathBehaviour.deathStateMachine.SetNextState(new GlassState());

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

        public static void FireKBBlast(CharacterBody body)
        {
            SlipLogger.LogD("Fired Sandswept Glass Explosion");
            //Util.PlayAttackSpeedSound("Play_char_glass_death", body.gameObject, 2f);
            //Vector3 corePosition = RoR2.Util.GetCorePosition(body.gameObject);

            Collider[] collider = body.gameObject.GetComponents<Collider>();
            if (collider.Length > 0)
            {
                for (int i = 0; i < collider.Length; i++)
                    collider[i].enabled = false;

            }

            if (NetworkServer.active)
            {

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
                    blast.time = 0.1f;

                    NetworkServer.Spawn(explosion);
                }
                else
                    SlipLogger.LogE("Couldn't fire upward blast in AffixSandswept FireKBBlast");
            }

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

            //Both of these knockbacks were previous iterations of the knockback. Keeping them here for archival purposes.
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

        private void CharacterModel_UpdateRendererMaterials(ILContext il)
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

        }

        public class AffixSandsweptBehavior : BaseBuffBodyBehavior, IOnIncomingDamageOtherServerReciever, IOnIncomingDamageServerReceiver, IBodyStatArgModifier
        {
            [BuffDefAssociation(useOnClient = true, useOnServer = true)]
            public static BuffDef GetBuffDef() => SlipContent.Buffs.AffixSandswept;

            public Run.FixedTimeStamp lastBecameGlass;
            public bool isGlass = false;

            
            public void OnIncomingDamageOther(HealthComponent victimHealthComponent, DamageInfo damageInfo)
            {
                damageInfo.damageType |= DamageType.Stun1s;
            }

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
        }
    }
}