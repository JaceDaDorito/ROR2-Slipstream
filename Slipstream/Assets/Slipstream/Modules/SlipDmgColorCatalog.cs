using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using System.Linq;
using R2API;
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
            AddColors();
            LoadColors();
        }

        //Any new colors should be added here
        private static void AddColors()
        {
            colors.Add(new Color(0.682353f, 0.8431373f, 0.9882353f)); //Glass Eye supercrit color. loadedIndices[0]
        }

        //Passes colors into the ColorAPI to apprehend RoR2's color catalog as well as create indexes to be used.
        private static void LoadColors()
        {
            foreach (var a in colors)
            {
                loadedIndices.Add(SlipColorAPI.RegisterDamageColor(a));
            }
        }
    }
}
