using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using System.Linq;
using R2API;

namespace Slipstream
{
    public static class SlipConfig
    {
        internal static ConfigEntry<bool> EnableItems;
        internal static ConfigEntry<bool> EnableEquipments;
        internal static void Init(ConfigFile config)
        {
            EnableItems = config.Bind<bool>("Slipstream :: Pickups", "Enable Items", true, "Affects if Slipstream's items will be enabled.");
            EnableEquipments = config.Bind<bool>("Slipstream :: Pickups", "Enable Equipments", true, "Affects if Slipstream's equipments will be enabled.");
        }
    }
}
