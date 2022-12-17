using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

//credit to Nebby and GrooveSalad

namespace Slipstream
{
    public class SlipstreamEliteRamps : SlipstreamContentRunBehaviour
    {
        public struct SlipstreamEliteRamp
        {
            public Texture rampTexture;
            public EliteDef eliteDef;
        }
        public static List<SlipstreamEliteRamp> eliteRamps = new List<SlipstreamEliteRamp>();
        public static bool hookActive;
        /*[SystemInitializer(typeof(EliteCatalog))]
        public static void Initialize()
        {
            if (eliteRamps.Count != 0 && !hookActive)
            {
                IL.RoR2.CharacterModel.UpdateMaterials += CopyMSU;
                hookActive = true;
            }
        }*/
        private void OnEnable()
        {
            if (SetHook(ref hookActive, eliteRamps.Count != 0))
            {
                IL.RoR2.CharacterModel.UpdateMaterials += CopyMSU;
            }
        }
        private void OnDisable()
        {
            if (UnsetHook(ref hookActive))
            {
                IL.RoR2.CharacterModel.UpdateMaterials -= CopyMSU;
            }
        }

        private static void CopyMSU(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                MoveType.After,
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<CharacterModel>("propertyStorage"),
                x => x.MatchLdsfld(typeof(CommonShaderProperties), "_EliteIndex")
            );
            c.GotoNext(
                MoveType.After,
                x => x.MatchCallOrCallvirt<MaterialPropertyBlock>("SetFloat")
            );
            c.Emit(OpCodes.Ldarg, 0);
            c.EmitDelegate<Action<CharacterModel>>((model) =>
            {
                for (int i = 0; i < eliteRamps.Count; i++)
                {
                    SlipstreamEliteRamp ramp = eliteRamps[i];
                    if (model.myEliteIndex == ramp.eliteDef.eliteIndex)
                    {
                        model.propertyStorage.SetTexture(EliteRampPropertyID, ramp.rampTexture);
                        return;
                    }
                    if (model.propertyStorage.GetTexture(EliteRampPropertyID) == ramp.rampTexture)
                    {
                        model.propertyStorage.Clear();
                    }
                }
            });
        }
        public static int EliteRampPropertyID
        {
            get
            {
                return Shader.PropertyToID("_EliteRamp");
            }
        }
    }
}
