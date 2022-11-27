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

namespace Slipstream.Buffs
{
    public class AffixSandswept : BuffBase
    {
        public override BuffDef BuffDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<BuffDef>("AffixSandswept");

        public EntityStateMachine targetStateMachine;

        

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
                if (!body.healthComponent.alive && body.HasBuff(BuffDef.buffIndex) && !body.GetComponent<AffixSandsweptBehavior>().isGlass)
                {
                    body.healthComponent.Networkbarrier = 0f;
                    body.healthComponent.Networkhealth = 1f;
                    body.healthComponent.Networkshield = 0f;               
                    body.MarkAllStatsDirty();

                    CharacterDeathBehavior deathBehaviour = body.GetComponent<CharacterDeathBehavior>();
                    body.GetComponent<AffixSandsweptBehavior>().isGlass = true;
                    body.GetComponent<AffixSandsweptBehavior>().lastBecameGlass = Run.FixedTimeStamp.now;
                    //deathBehaviour.deathState = new EntityStates.SerializableEntityStateType(typeof(SandsweptGlassShatter));
                    deathBehaviour.deathStateMachine.SetNextState(new GlassState());
                    EntityStateMachine[] array = deathBehaviour.idleStateMachine;
                    for( int i = 0; i < array.Length; i++)
                    {
                        array[i].SetNextState(new Idle());
                    }
                }
            }        
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
                if (isGlass && lastBecameGlass.timeSince <= 0.5f)
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