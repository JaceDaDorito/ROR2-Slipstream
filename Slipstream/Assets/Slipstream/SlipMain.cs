using BepInEx;
using BepInEx.Configuration;
using Moonstorm;
using R2API;
using R2API.Utils;
using System.Linq;
using System.Security;
using System.Security.Permissions;

namespace Slipstream
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.TeamMoonstorm.MoonstormSharedUtils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.TheMysticSword.AspectAbilities", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(ItemDropAPI), nameof(LanguageAPI), nameof(ResourcesAPI), nameof(PrefabAPI), nameof(BuffAPI), nameof(LoadoutAPI), nameof(ProjectileAPI), nameof(RecalculateStatsAPI))]
    public class SlipMain : BaseUnityPlugin
    {
        //Literally copying LIT setup I'm so sorry. I'm studying the code I swear don't get mad at me Neb.

        internal const string ModGuid = "com.TeamSlipstream.Slipstream";
        internal const string ModName = "Slipstream";
        internal const string ModVer = "0.0.1";

        public static string MODPREFIX = "@Slipstream:";
        public string identifier => "com.TeamSlipstream.Slipstream";

        public static SlipMain instance;

        public static PluginInfo pluginInfo;

        public static ConfigFile config;

        public static bool DEBUG = false;

        public void Awake()
        {
            ConfigurableFieldManager.AddMod(Config);
            TokenModifierManager.AddMod();

            instance = this;
            pluginInfo = Info;
            config = Config;

            Init();
            new SlipContent().Init();
        }

        private void Init()
        {
            Assets.Init();
            SlipConfig.Init(config);

            //new Buffs.Buffs().Init;
        }

    }
}