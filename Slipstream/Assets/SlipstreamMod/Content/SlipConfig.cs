using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using System.Linq;
using R2API;
using Moonstorm.Loaders;

namespace Slipstream
{
    public class SlipConfig : ConfigLoader<SlipConfig>
    {
        //Adds config options that applies to the entire mod rather than a particular thing. For example, disabling ALL of our items.

        public const string items = "Slip.Items";
        public const string equips = "Slip.Equips";

        public override BaseUnityPlugin MainClass { get; } = SlipMain.instance;

        public override bool CreateSubFolder => true;

        public static ConfigFile itemConfig;
        public static ConfigFile equipsConfig;

        internal static ConfigEntry<bool> enableItems;
        internal static ConfigEntry<bool> enableEquipments;

        public void Init()
        {
            itemConfig = CreateConfigFile(items, true);
            equipsConfig = CreateConfigFile(equips, true);

            //SetConfigs();
        }

        /*internal static void SetConfigs()
        {

        }*/
    }
}
