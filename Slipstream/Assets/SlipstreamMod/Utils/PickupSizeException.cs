using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace Slipstream.Items
{
    public class PickupSizeException : MonoBehaviour
    {
        [Header("Usually reserve this component if the pickup is animated.")]

        [Tooltip("If multiplier mode is on, the scale value will multiply with the unmodified pickup scale. If it isn't on, the scale value will replace the unmodified pickup scale.")]
        public bool multiplierMode = true;

        [Tooltip("A value representing the scale of the pickup in game depending on the multiplier mode.")]
        public float scale = 1.5f;
    }

    public class PickupSizeExceptionIL
    {
        public void Init()
        {
            IL.RoR2.PickupDisplay.RebuildModel += PickupDisplay_RebuildModel;
        }

        private void PickupDisplay_RebuildModel(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            bool found = c.TryGotoNext(
                MoveType.After,
                /*x => x.MatchLdarg(0),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<PickupDisplay>(nameof(PickupDisplay.modelScale)),
                x => x.MatchLdsfld<PickupDisplay>(nameof(PickupDisplay.idealVolume)),
                x => x.MatchLdcR4(out _),
                x => x.MatchCallOrCallvirt(typeof(UnityEngine.Mathf).GetMethod("Pow")),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(out _),
                x => x.MatchCallOrCallvirt(typeof(UnityEngine.Mathf).GetMethod("Pow")),*/
                x => x.MatchDiv(),
                x => x.MatchMul(),
                x => x.MatchStfld<PickupDisplay>(nameof(PickupDisplay.modelScale)));
            if (found)
            {
#if DEBUG
                SlipLogger.LogI($"IL found in PickupDisplay_RebuildModel");
#endif

                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<PickupDisplay>>((pd) => {
                    PickupSizeException pickupSizeException = pd.modelObject.GetComponent<PickupSizeException>();
                    if (pickupSizeException && pickupSizeException.multiplierMode) pd.modelScale *= pickupSizeException.scale;
                    if (pickupSizeException && !pickupSizeException.multiplierMode) pd.modelScale = pickupSizeException.scale;
                });
            }
            else
                SlipLogger.LogW($"Cound't find Rebuid Model location. Can't inject Pickup Exception");
        }
    }
}
