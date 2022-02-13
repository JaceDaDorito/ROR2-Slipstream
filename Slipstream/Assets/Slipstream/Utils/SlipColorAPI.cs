using RoR2;
using UnityEngine;
using System.Collections.Generic;

namespace Slipstream
{
    public static class SlipColorAPI
    {
        //ty Mystic <3
        internal static void Init()
        {
            //subscribes DamageColor_FindColor to DamageColor.FindColor
            On.RoR2.DamageColor.FindColor += DamageColor_FindColor;
        }

        //ArrayList that contains new DamageColorIndices in Slipstream
        public static List<DamageColorIndex> registerColorIndex = new List<DamageColorIndex>();
        private static Color DamageColor_FindColor(On.RoR2.DamageColor.orig_FindColor orig, DamageColorIndex colorIndex)
        {
            if (registerColorIndex.Contains(colorIndex)) return DamageColor.colors[(int)colorIndex];
            return orig(colorIndex);
        }

        //Adds colors to the RoR2's damage color array.
        public static DamageColorIndex RegisterDamageColor(Color color)
        {
            int nextColorIndex = DamageColor.colors.Length;
            DamageColorIndex newDamageColorIndex = (DamageColorIndex)nextColorIndex;
            HG.ArrayUtils.ArrayAppend(ref DamageColor.colors, color);
            registerColorIndex.Add(newDamageColorIndex);
            return newDamageColorIndex;
        }
    }
}
