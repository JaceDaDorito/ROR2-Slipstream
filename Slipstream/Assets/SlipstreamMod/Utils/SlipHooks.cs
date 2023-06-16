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


namespace Slipstream
{
    //A class that has hooks that deal with multiple pieces of content
    public class SlipHooks
    {
        public void Init()
        {
            IL.EntityStates.GenericCharacterDeath.OnEnter += GenericCharacterDeath_OnEnter;
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
                c.EmitDelegate<Func<GenericCharacterDeath, bool>>((gdc) => gdc.characterBody && 
                (gdc.characterBody.HasBuff(Slipstream.Buffs.AffixSandswept.buff) || //If the body is Sandswept
                gdc.characterBody.GetItemCount(Slipstream.Items.BlackHealth.item) > 0)); //If the body has Blackhealth (the item a minion gets when the owner gets Coalition)
                c.Emit(OpCodes.Brtrue, returnLabel);
            }
            else
                SlipLogger.LogW($"Couldn't find Void Death location.");

        }
    }
}
