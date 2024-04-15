using MSU;
using EntityStates;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using Slipstream.Items;


namespace Slipstream
{
    public class SlipHooks
    {
        public void Init()
        {
            new PickupSizeExceptionIL().Init();

            IL.EntityStates.GenericCharacterDeath.OnEnter += GenericCharacterDeath_OnEnter;
        }

        private void GenericCharacterDeath_OnEnter(MonoMod.Cil.ILContext il)
        {
            //Hook to prevent death animations

            ILLabel returnLabel = null; 
            ILCursor c = new ILCursor(il);
            bool found = c.TryGotoNext(
                MoveType.After,         
                x => x.MatchLdarg(0),
                x => x.MatchCall<GenericCharacterDeath>("get_isVoidDeath"),
                x => x.MatchBrtrue(out returnLabel));
            if (found)
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<GenericCharacterDeath, bool>>((gdc) => gdc.characterBody && 
                (gdc.characterBody.HasBuff(Slipstream.Buffs.AffixSandswept.buff) || //If the body is Sandswept
                gdc.characterBody.GetItemCount(Slipstream.Items.BlackHealth.item) > 0)); //If the body has Blackhealth (the item a minion gets when the owner gets Coalition)
                c.Emit(OpCodes.Brtrue, returnLabel);
            }
            else
                SlipLog.Warning($"Couldn't find Void Death location.");

        }
    }
}
