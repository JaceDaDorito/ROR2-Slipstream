using BepInEx;
using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Slipstream
{
    public static class SlipDmgColorCatalog
    {
        //should this be a normal array idk
        private static List<Color> colors = new List<Color>();
        public static List<DamageColorIndex> loadedIndices = new List<DamageColorIndex>();
        internal static void Init()
        {

        }
    }
}
