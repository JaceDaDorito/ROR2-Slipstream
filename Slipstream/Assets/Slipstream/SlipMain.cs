using BepInEx;
using BepInEx.Configuration;
using Slipstream.Modules;
using Moonstorm;
using R2API;
using R2API.Utils;
using System.Linq;
using System.Security;
using System.Security.Permissions;

#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618
[module: UnverifiableCode]

namespace Slipstream
{
    //Not all modules are implemented, I will continue making them. If you want to take it on yourself then reference LIT. Ask NebNeb questions too.

    //Dependencies
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.TeamMoonstorm.MoonstormSharedUtils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.TheMysticSword.AspectAbilities", BepInDependency.DependencyFlags.SoftDependency)]

    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(ItemDropAPI), nameof(LanguageAPI), nameof(ResourcesAPI), nameof(PrefabAPI), nameof(BuffAPI), nameof(LoadoutAPI), nameof(ProjectileAPI), nameof(RecalculateStatsAPI))]
    public class SlipMain : BaseUnityPlugin
    {
        //Literally copying LIT setup I'm so sorry. I'm studying the code I swear don't get mad at me Neb.

        internal const string GUID = "com.TeamSlipstream.Slipstream";
        internal const string MODNAME = "Slipstream";
        internal const string VERSION = "0.0.1";

        public static SlipMain instance;

        public static PluginInfo pluginInfo;

        public static ConfigFile config;

        public static bool DEBUG = false;

        public void Awake()
        {
            //Allows organized configurable fields of public static fields.
            ConfigurableFieldManager.AddMod(Config);
            //Updates in game tokens to reflect changed config options. For example changing damage of x to 8 would show that it does 8 damage in game. Relies onthe actual item token though too.
            TokenModifierManager.AddMod();

            instance = this;
            pluginInfo = Info;
            config = Config;
            SlipLogger.logger = Logger;

            Init();
            new SlipContent().Init();
        }

        private void Init()
        {
            Assets.Init();
            SlipLanguage.Initialize();
            SlipConfig.Init(config);

            new Modules.Projectiles().Init();
            new Pickups().Init();
            new Buffs.Buffs().Init();

            GetType().Assembly.GetTypes()
              .Where(type => typeof(EntityStates.EntityState).IsAssignableFrom(type))
              .ToList()
              .ForEach(state => HG.ArrayUtils.ArrayAppend(ref SlipContent.serializableContentPack.entityStateTypes, new EntityStates.SerializableEntityStateType(state)));
        }

    }
}