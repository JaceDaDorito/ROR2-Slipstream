//using Slipstream.Utils;
using Moonstorm.Loaders;
using Moonstorm;
using RoR2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.PostProcessing;
using Path = System.IO.Path;

namespace Slipstream
{
    //These are the bundle categories
    public enum SlipBundle
    {
        Invalid,
        All, //only use this keyword if you are loading something generically, like if you want to load all materials from every bundle.
        Main, //contains Content Pack, Expansion, and Run Behaviour (though run behaviour doesnt do anything atm)
        Base, //contains misc stuff like General Materials and Interfaces (has critical shield)
        Equipments, //contains equipment assets
        Items, //contains item assets
        Elites, //contains elite assets
        Scenes, //contains assets RELATING to scenes (not necessarily the scene itself)
        AridExpanse, //contains Arid Expanse
        Skills, //contains alt skill assets
        Skins, //contains skin assets
        BasicMonsters //contains all small monsters
    }
    public class SlipAssets : AssetsLoader<SlipAssets>
    {

        //Notice for Slipstream developers:
        //If you want to use assets in our bundles now, you have to do SlipAssets.LoadAsset<[Type]>([assetstring], SlipBundles.[bundle]);
        //So for example, if you want to load the PepperSpray item display material you do:
        //  SlipAssets.LoadAsset<Material>("matPepperSpray", SlipBundles.Items);
        //The material of PepperSpray is found with the other items so thats why its in the Item bundle.

        private const string ASSET_BUNDLE_FOLDER_NAME = "assetbundles";
        private const string MAIN = "slipmain"; 
        private const string BASE = "slipbase"; 
        private const string EQUIPS = "slipequipments"; 
        private const string ITEMS = "slipitems"; 
        private const string ELITES = "slipelites"; 
        private const string SCENES = "slipscenes"; 
        private const string ARIDEXPANSE = "sliparidexpanse"; 
        private const string SKILLS = "slipskills"; 
        private const string SKINS = "slipskins";
        private const string BASICMONSTERS = "slipbasicmonsters";

        private static Dictionary<SlipBundle, AssetBundle> assetBundles = new Dictionary<SlipBundle, AssetBundle>();
        [Obsolete("LoadAsset should not be used without specifying the SlipBundle")]
        public new static TAsset LoadAsset<TAsset>(string name) where TAsset : UnityEngine.Object
        {
#if DEBUG
            SlipLogger.LogW($"Method {GetCallingMethod()} is trying to load an asset of name {name} and type {typeof(TAsset).Name} without specifying what bundle to use for loading. This causes large performance loss as SS2Assets has to search thru the entire bundle collection. Avoid calling LoadAsset without specifying the AssetBundle.");
#endif
            return LoadAsset<TAsset>(name, SlipBundle.All);
        }

        [Obsolete("LoadAllAssetsOfType should not be used without specifying the SS2Bundle")]
        public new static TAsset[] LoadAllAssetsOfType<TAsset>() where TAsset : UnityEngine.Object
        {
#if DEBUG
            SlipLogger.LogW($"Method {GetCallingMethod()} is trying to load all assets of type {typeof(TAsset).Name} without specifying what bundle to use for loading. This causes large performance loss as SS2Assets has to search thru the entire bundle collection. Avoid calling LoadAsset without specifying the AssetBundle.");
#endif
            return LoadAllAssetsOfType<TAsset>(SlipBundle.All);
        }
        public static TAsset LoadAsset<TAsset>(string name, SlipBundle bundle) where TAsset : UnityEngine.Object
        {
            if (Instance == null)
            {
                SlipLogger.LogE("Cannot load asset when there's no isntance of SS2Assets!");
                return null;
            }
            return Instance.LoadAssetInternal<TAsset>(name, bundle);
        }
        public static TAsset[] LoadAllAssetsOfType<TAsset>(SlipBundle bundle) where TAsset : UnityEngine.Object
        {
            if (Instance == null)
            {
                SlipLogger.LogE("Cannot load asset when there's no instance of SS2Assets!");
                return null;
            }
            return Instance.LoadAllAssetsOfTypeInternal<TAsset>(bundle);
        }

#if DEBUG
        private static string GetCallingMethod()
        {
            var stackTrace = new StackTrace();

            for (int stackFrameIndex = 0; stackFrameIndex < stackTrace.FrameCount; stackFrameIndex++)
            {
                var frame = stackTrace.GetFrame(stackFrameIndex);
                var method = frame.GetMethod();

                if (method == null)
                    continue;

                var declaringType = method.DeclaringType;
                if (declaringType == typeof(SlipBundle))
                    continue;

                var fileName = frame.GetFileName();
                var fileLineNumber = frame.GetFileLineNumber();
                var fileColumnNumber = frame.GetFileColumnNumber();

                return $"{declaringType.FullName}.{method.Name}({GetMethodParams(method)}) (fileName: {fileName}, Location: L{fileLineNumber} C{fileColumnNumber})";
            }

            return "[COULD NOT GET CALLING METHOD]";
        }

        private static string GetMethodParams(MethodBase methodBase)
        {
            var parameters = methodBase.GetParameters();
            if (parameters.Length == 0)
                return string.Empty;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var parameter in parameters)
            {
                stringBuilder.Append(parameter.ToString() + ", ");
            }
            return stringBuilder.ToString();
        }
#endif
        public override AssetBundle MainAssetBundle => GetAssetBundle(SlipBundle.Main);
        public string AssemblyDir => Path.GetDirectoryName(SlipMain.pluginInfo.Location);
        public AssetBundle GetAssetBundle(SlipBundle bundle)
        {
            return assetBundles[bundle];
        }
        internal void Init()
        {
            var bundlePaths = GetAssetBundlePaths();
            foreach (string path in bundlePaths)
            {
                var fileName = Path.GetFileName(path);
                switch (fileName)
                {
                    case MAIN: LoadBundle(path, SlipBundle.Main); break;
                    case BASE: LoadBundle(path, SlipBundle.Base); break;
                    case EQUIPS: LoadBundle(path, SlipBundle.Equipments); break;
                    case ITEMS: LoadBundle(path, SlipBundle.Items); break;
                    case ELITES: LoadBundle(path, SlipBundle.Elites); break;
                    case SCENES: LoadBundle(path, SlipBundle.Scenes); break;
                    case ARIDEXPANSE: LoadBundle(path, SlipBundle.AridExpanse); break;
                    case SKILLS: LoadBundle(path, SlipBundle.Skills); break;
                    case SKINS: LoadBundle(path, SlipBundle.Skins); break;
                    case BASICMONSTERS: LoadBundle(path, SlipBundle.BasicMonsters); break;
                    default: SlipLogger.LogW($"Invalid or Unexpected file in the AssetBundles folder (File name: {fileName}, Path: {path})"); break;
                }
            }

            void LoadBundle(string path, SlipBundle bundleEnum)
            {
                try
                {
                    AssetBundle bundle = AssetBundle.LoadFromFile(path);
                    if (!bundle)
                    {
                        throw new FileLoadException("AssetBundle.LoadFromFile did not return an asset bundle");
                    }

                    if (assetBundles.ContainsKey(bundleEnum))
                    {
                        throw new InvalidOperationException($"AssetBundle in path loaded succesfully, but the assetBundles dictionary already contains an entry for {bundleEnum}.");
                    }

                    assetBundles[bundleEnum] = bundle;
                }
                catch (Exception e)
                {
                    SlipLogger.LogE($"Could not load assetbundle at path {path} and assign to enum {bundleEnum}. {e}");
                }
            }
        }

        private TAsset LoadAssetInternal<TAsset>(string name, SlipBundle bundle) where TAsset : UnityEngine.Object
        {
            TAsset asset = null;
            if (bundle == SlipBundle.All)
            {
                asset = FindAsset<TAsset>(name, out SlipBundle foundInBundle);
#if DEBUG
                if (!asset)
                {
                    SlipLogger.LogW($"Could not find asset of type {typeof(TAsset).Name} with name {name} in any of the bundles.");
                }
                else
                {
                    SlipLogger.LogI($"Asset of type {typeof(TAsset).Name} was found inside bundle {foundInBundle}, it is recommended that you load the asset directly");
                }
#endif
                return asset;
            }

            asset = assetBundles[bundle].LoadAsset<TAsset>(name);
#if DEBUG
            if (!asset)
            {
                SlipLogger.LogW($"The  method \"{GetCallingMethod()}\" is calling \"LoadAsset<TAsset>(string, SlipBundle)\" with the arguments \"{typeof(TAsset).Name}\", \"{name}\" and \"{bundle}\", however, the asset could not be found.\n" +
                    $"A complete search of all the bundles will be done and the correct bundle enum will be logged.");
                return LoadAssetInternal<TAsset>(name, SlipBundle.All);
            }
#endif
            return asset;

            TAsset FindAsset<TAsset>(string assetName, out SlipBundle foundInBundle) where TAsset : UnityEngine.Object
            {
                foreach ((var enumVal, var assetBundle) in assetBundles)
                {
                    var loadedAsset = assetBundle.LoadAsset<TAsset>(assetName);
                    if (loadedAsset)
                    {
                        foundInBundle = enumVal;
                        return loadedAsset;
                    }
                }
                foundInBundle = SlipBundle.Invalid;
                return null;
            }
        }

        private TAsset[] LoadAllAssetsOfTypeInternal<TAsset>(SlipBundle bundle) where TAsset : UnityEngine.Object
        {
            List<TAsset> loadedAssets = new List<TAsset>();
            if (bundle == SlipBundle.All)
            {
                FindAssets<TAsset>(loadedAssets);
#if DEBUG
                if (loadedAssets.Count == 0)
                {
                    SlipLogger.LogW($"Could not find any asset of type {typeof(TAsset)} inside any of the bundles");
                }
#endif
                return loadedAssets.ToArray();
            }

            var assetBundle = assetBundles[bundle];
            if(assetBundle.isStreamedSceneAssetBundle)
            {
#if DEBUG
                SlipLogger.LogW($"The  method \"{GetCallingMethod()}\" is calling \"LoadAllAssets<TAsset>(SlipBundle)\" with the argument \"{bundle}\", however, the assetbundle assigned to {bundle} is a streamed scene AssetBundle, which is not valid.\n" +
    $"Do not call LoadAllAssetts<TAsset>(SlipBundle) while assigning the enum to a Streamed Scene AssetBundle, as this causes exceptions and errors.");
#endif
                return loadedAssets.ToArray();
            }
            loadedAssets = assetBundles[bundle].LoadAllAssets<TAsset>().ToList();
#if DEBUG
            if (loadedAssets.Count == 0)
            {
                SlipLogger.LogW($"Could not find any asset of type {typeof(TAsset)} inside the bundle {bundle}");
            }
#endif
            return loadedAssets.ToArray();

            void FindAssets<TAsset>(List<TAsset> output) where TAsset : UnityEngine.Object
            {
                foreach ((var _, var bndl) in assetBundles)
                {
                    if(!bndl.isStreamedSceneAssetBundle)
                        output.AddRange(bndl.LoadAllAssets<TAsset>());
                }
                return;
            }
        }

        internal void SwapMaterialShaders()
        {
            SwapShadersFromMaterials(LoadAllAssetsOfType<Material>(SlipBundle.All).Where(mat => mat.shader.name.StartsWith("Stubbed")));
        }

        internal void FinalizeCopiedMaterials()
        {
            foreach (var (_, bundle) in assetBundles)
            {
                FinalizeMaterialsWithAddressableMaterialShader(bundle);
            }
        }

        private string[] GetAssetBundlePaths()
        {
            return Directory.GetFiles(Path.Combine(AssemblyDir, ASSET_BUNDLE_FOLDER_NAME))
               .Where(filePath => !filePath.EndsWith(".manifest"))
               .ToArray();
        }
    }

    /*public class SlipAssets : AssetsLoader<SlipAssets>
    {
        //Loads all assets such as models, effects, and soundbanks (not yet but soon)
        private const string assetBundleFolderName = "assetbundles";
        private const string mainAssetBundleName = "slipassets";

        public static ReadOnlyCollection<AssetBundle> assetBundles;
        public override AssetBundle MainAssetBundle => assetBundles[0];
        public string AssemblyDir => Path.GetDirectoryName(SlipMain.pluginInfo.Location);
        //private string SoundBankPath { get => Path.Combine(AssemblyDir, "SlipSoundBnk.bnk"); }

        internal void Init()
        {
            List<AssetBundle> loadedBundles = new List<AssetBundle>();
            var bundlePaths = GetAssetBundlePaths();
            for (int i = 0; i < bundlePaths.Length; i++)
            {
                loadedBundles.Add(AssetBundle.LoadFromFile(bundlePaths[i]));
            }
            assetBundles = new ReadOnlyCollection<AssetBundle>(loadedBundles);

            //LoadPostProcessing();
        }

        internal void SwapMaterialShaders()
        {
            SwapShadersFromMaterialsInBundle(MainAssetBundle);
        }

        private string[] GetAssetBundlePaths()
        {
            return Directory.GetFiles(Path.Combine(AssemblyDir, assetBundleFolderName))
               .Where(filePath => !filePath.EndsWith(".manifest"))
               .OrderByDescending(path => Path.GetFileName(path).Equals(mainAssetBundleName))
               .ToArray();
        }
    }*/
}
