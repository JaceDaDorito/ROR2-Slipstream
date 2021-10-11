/*using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.ContentManagement;
using Slipstream.Items.Other;
using Slipstream.Items.White;
using Slipstream.Items.Yellow;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Slipstream
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(ItemDropAPI), nameof(LanguageAPI), nameof(ResourcesAPI), nameof(PrefabAPI), nameof(BuffAPI), nameof(LoadoutAPI), nameof(ProjectileAPI), nameof(RecalculateStatsAPI))]
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
                return "Slipstream";
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

    public static class Assets
    {
        public static AssetBundle mainAssetBundle = null;
        //the filename of your assetbundle
        internal static string assetBundleName = "SlipstreamAssets";

        internal static string assemblyDir
        {
            get
            {
                return Path.GetDirectoryName(Main.Info.Location);
            }
        }

        public static void PopulateAssets()
        {
            mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assemblyDir, assetBundleName));
            ContentPackProvider.serializedContentPack = mainAssetBundle.LoadAsset<SerializableContentPack>(ContentPackProvider.contentPackName);
        }
    }

    public class SlipstreamPlugin : BaseUnityPlugin
    {
        public static List<ItemDef> ModItemDefs = new List<ItemDef>();

        internal ContentPack contentPack = new ContentPack();

        public static AssetBundle AssetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Slipstream.slipstreamassets"));

        private const string ModVer = "0.0.1";
        private const string ModName = "Slipstream";
        public const string ModGuid = "com.TeamSlipstream.Slipstream";
        public static string MODPREFIX = "@Slipstream:";
        public string identifier => "com.TeamSlipstream.Slipstream";

        public static PluginInfo pluginInfo;

        public void Awake() //Awake() is run when the mod loads upon starting your game
        {
            pluginInfo = this.Info;
            //You have to call the Init() method of any items you add to here
            #region Item Inits

            //Whites
            SnakeEyes.Init();
            //var CowboyHatItem = new CowboyHat();
            //CowboyHatItem.Init();

            //Greens

            //Reds

            //Yellows
            RottingPerforator.Init();
            SubzeroPerforator.Init();

            //Blues

            //Equipment

            //Other Stuff
            MalachiteUrchinOnKill.Init();

            #endregion Item Inits


            RoR2Application.isModded = true;
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
            Logger.LogMessage("The Slipstream has broken.\n\nRun."); //Text that gets written to the console, this is mainly here just to make sure the mod loads successfully
        }
    }
}*/