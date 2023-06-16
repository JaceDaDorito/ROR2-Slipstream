using BepInEx;
using BepInEx.Configuration;
using HG.Reflection;
using Slipstream.Modules;
using Moonstorm;
using Slipstream.Items;
using Slipstream.Scenes;
using R2API;
using R2API.ContentManagement;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using R2API.Utils;
using RoR2.ExpansionManagement;
using RoR2;

#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618
[module: UnverifiableCode]
[assembly: HG.Reflection.SearchableAttribute.OptIn]


namespace Slipstream
{
    //Not all modules are implemented but they will be. If you are working on something make sure to reference off of Lost In Transit.

    //Dependencies
    [BepInDependency("com.bepis.r2api.content_management")]
    [BepInDependency("com.bepis.r2api.dot")]
    [BepInDependency("com.bepis.r2api.damagetype")]
    [BepInDependency("com.bepis.r2api.deployable")]
    [BepInDependency("com.bepis.r2api.networking")]
    [BepInDependency("com.bepis.r2api.prefab")]
    [BepInDependency("com.bepis.r2api.recalculatestats")]
    [BepInDependency("com.bepis.r2api.sound")]
    [BepInDependency("com.TeamMoonstorm.MoonstormSharedUtils", BepInDependency.DependencyFlags.HardDependency)]

    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(GUID, MODNAME, VERSION)]
    //[R2APISubmoduleDependency(nameof(DotAPI), nameof(DamageAPI), nameof(PrefabAPI), nameof(DeployableAPI), nameof(SoundAPI))]
    public class SlipMain : BaseUnityPlugin
    {
        internal const string GUID = "com.TeamSlipstream.Slipstream";
        internal const string MODNAME = "Slipstream";
        internal const string VERSION = "0.0.1";

        public static SlipMain instance;

        public static PluginInfo pluginInfo;

        public static ConfigFile config;

        public static bool DEBUG = false;

        public void Awake()
        {
            //Establishes an instance of the mod, config, and console logger.
            instance = this;
            pluginInfo = Info;
            config = Config;
            SlipLogger.logger = Logger;

            StartupInitialization();

            //Allows organized configurable fields of public static fields.
            Moonstorm.ConfigSystem.AddMod(this);

            //Slipstream now loads alongside the game "and will properly show percentage increase in the loading screen" -Neb
        }

        private void StartupInitialization()
        {
            new SlipAssets().Init();
            new SlipConfig().Init();
            new SlipContent().Init();
            new SlipLanguage().Init();
            new SlipHooks().Init();
            new SlipCriticalShield().Init();
            new VoidShieldCatalog().Init();
            new PickupSizeExceptionIL().Init();
            GenerateLockIcon();
        }

        public static ExpansionDef slipexpansion;
        public static void GenerateLockIcon()
        {
            //This code runs on startup of the mod to load the lock icon and set the disabled icon to the lock icon
            //Loads SOTV to reference 
            slipexpansion = SlipAssets.LoadAsset<ExpansionDef>("SlipExpansionDef", SlipBundle.Main);
            ExpansionDef expansionDef = LegacyResourcesAPI.Load<ExpansionDef>("ExpansionDefs/DLC1");

            slipexpansion.disabledIconSprite = expansionDef.disabledIconSprite;
        }

    }
}