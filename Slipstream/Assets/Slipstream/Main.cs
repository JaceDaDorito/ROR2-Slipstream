using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.ContentManagement;
using Slipstream.Modules;
using StubbedConverter;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Path = System.IO.Path;
using static R2API.RecalculateStatsAPI;

namespace Slipstream
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.valex.ShaderConverter", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.TeamMoonstorm.MoonstormSharedUtils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(ItemDropAPI), nameof(LanguageAPI), nameof(ResourcesAPI), nameof(PrefabAPI), nameof(BuffAPI), nameof(LoadoutAPI), nameof(ProjectileAPI), nameof(RecalculateStatsAPI))]
    public class SlipstreamPlugin : BaseUnityPlugin
    {
        //public static List<ItemDef> ModItemDefs = new List<ItemDef>();

        //internal ContentPack contentPack = new ContentPack();

        //public static AssetBundle AssetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Slipstream.slipstreamassets"));

        private const string ModVer = "0.0.1";
        private const string ModName = "Slipstream";
        public const string ModGuid = "com.TeamSlipstream.Slipstream";
        public static string MODPREFIX = "@Slipstream:";
        public string identifier => "com.TeamSlipstream.Slipstream";

        public static PluginInfo pluginInfo;
        private void Awake() //Awake() is run when the mod loads upon starting your game
        {
            //You have to call the Init() method of any items you add to here


            pluginInfo = this.Info;
            Assets.PopulateAssets();

            ContentPackProvider.Initialize();

            RoR2Application.isModded = true;

            Logger.LogMessage(ModName + " Version " + ModVer + " Loaded"); //Text that gets written to the console, this is mainly here just to make sure the mod loads successfully

            if (Assets.mainAssetBundle != null)
                Logger.LogMessage("Assetbundle was loaded!");

            if (ContentPackProvider.serializedContentPack != null)
                Logger.LogMessage("ContentPack was loaded!");

            #region Item Inits

            //Whites


            //var SnakeEyesItem = new SnakeEyes();
            //SnakeEyesItem.Init(); 

            var PepperSprayItem = new PepperSpray();
            PepperSprayItem.Init();

            //Greens
            var GlassEyeItem = new GlassEye();
            GlassEyeItem.Init();


            //Reds
            //var ChungusItem = new Chungus();
            //ChungusItem.Init();

            //Yellows
            var RottingPerforatorItem = new RottingPerforator();
            RottingPerforatorItem.Init();

            var SubzeroPerforatorItem = new SubzeroPerforator();
            SubzeroPerforatorItem.Init();

            //Blues

            //Equipment

            //Other Stuff
            #endregion Item Inits

        }
    }

     public static class Assets
    {
        public static AssetBundle mainAssetBundle = null;
        //the filename of your assetbundle
        internal static string assetBundleName = "slipstreamassets";

        internal static string assemblyDir
        {
            get
            {
                return Path.GetDirectoryName(SlipstreamPlugin.pluginInfo.Location);
            }
        }

        public static void PopulateAssets()
        {
            mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assemblyDir, assetBundleName));
            ShaderConverter.ConvertStubbedShaders(Assets.mainAssetBundle, true);
            ContentPackProvider.serializedContentPack = mainAssetBundle.LoadAsset<SerializableContentPack>(ContentPackProvider.contentPackName);

        }
    }
    public class ContentPackProvider : IContentPackProvider
    {
        public static SerializableContentPack serializedContentPack;
        public static ContentPack contentPack;
        //Should be the same names as your SerializableContentPack in the asset bundle
        public static string contentPackName = "SlipstreamContentPack";

        public string identifier
        {
            get
            {
                //If I see this name while loading a mod I will make fun of you
                return "com.TeamSlipstream.Slipstream";
            }
        }

        internal static void Initialize()
        {
            contentPack = serializedContentPack.CreateContentPack();
            ContentManager.collectContentPackProviders += AddCustomContent;
        }

        private static void AddCustomContent(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new ContentPackProvider());
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}