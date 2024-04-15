using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using System.Linq;
using R2API;
using MSU.Config;
using System.Collections;
using MSU;

namespace Slipstream
{
    public class SlipConfig
    {
        public const string PREFIX = "Slip.";
        public const string MAIN = PREFIX + "Main";
        public const string ITEMS = PREFIX + "Items";
        public const string EQUIPS = PREFIX + "Equips";

        internal static ConfigFactory ConfigFactory { get; private set; }

        public static ConfigFile ConfigMain { get; private set; }
        public static ConfigFile ConfigItem { get; private set; }
        public static ConfigFile ConfigEquip { get; private set; }

        internal static IEnumerator RegisterToModSettingsManager()
        {
            var request = SlipAssets.LoadAssetAsync<Sprite>("Icon", SlipBundle.Main);

            request.StartLoad();
            while (!request.IsComplete)
                yield return null;
        }

        internal SlipConfig(BaseUnityPlugin bup)
        {
            ConfigFactory = new ConfigFactory(bup, true);
            ConfigMain = ConfigFactory.CreateConfigFile(MAIN, true);
            ConfigItem = ConfigFactory.CreateConfigFile(ITEMS, true);
            ConfigEquip = ConfigFactory.CreateConfigFile(EQUIPS, true);
        }
    }
}
