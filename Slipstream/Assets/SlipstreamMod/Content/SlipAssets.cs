using R2API;
using MSU;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Path = System.IO.Path;
using UObject = UnityEngine.Object;
using System.Collections;


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
    public class SlipAssets
    {
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


        private static string SoundBankPath { get => Path.Combine(Path.GetDirectoryName(SlipMain.instance.Info.Location), "soundbanks", "SlipBNK.bnk"); }
        private static string AssetBundleFolderPath => Path.Combine(Path.GetDirectoryName(SlipMain.instance.Info.Location), ASSET_BUNDLE_FOLDER_NAME);

        private static Dictionary<SlipBundle, AssetBundle> _assetBundles = new Dictionary<SlipBundle, AssetBundle>();
        private static AssetBundle[] _streamedSceneBundles = Array.Empty<AssetBundle>();

        public static ResourceAvailability AssetsAvailability = new ResourceAvailability();


        public static AssetBundle GetAssetBundle(SlipBundle bundle)
        {
            return _assetBundles[bundle];
        }

        public static TAsset LoadAsset<TAsset>(string name, SlipBundle bundle) where TAsset : UObject
        {
            TAsset asset = null;
            if (bundle == SlipBundle.All)
            {
                return FindAsset<TAsset>(name);
            }

            asset = _assetBundles[bundle].LoadAsset<TAsset>(name);

#if DEBUG
            if (!asset)
            {
                SlipLog.Warning($"The method \"{GetCallingMethod()}\" is calling \"LoadAsset<TAsset>(string, SlipBundle)\" with the arguments \"{typeof(TAsset).Name}\", \"{name}\" and \"{bundle}\", however, the asset could not be found.\n" +
                    $"A complete search of all the bundles will be done and the correct bundle enum will be logged.");

                return LoadAsset<TAsset>(name, SlipBundle.All);
            }
#endif
            return asset;
        }

        public static SlipAssetRequest<TAsset> LoadAssetAsync<TAsset>(string name, SlipBundle bundle) where TAsset : UObject
        {
            return new SlipAssetRequest<TAsset>(name, bundle);
        }

        public static TAsset[] LoadAllAssets<TAsset>(SlipBundle bundle) where TAsset : UObject
        {
            TAsset[] loadedAssets = null;
            if (bundle == SlipBundle.All)
            {
                return FindAssets<TAsset>();
            }
            loadedAssets = _assetBundles[bundle].LoadAllAssets<TAsset>();

#if DEBUG
            if (loadedAssets.Length == 0)
            {
                SlipLog.Warning($"Could not find any asset of type {typeof(TAsset).Name} inside the bundle {bundle}");
            }
#endif
            return loadedAssets;
        }

        public static SlipAssetRequest<TAsset> LoadAllAssetsAsync<TAsset>(SlipBundle bundle) where TAsset : UObject
        {
            return new SlipAssetRequest<TAsset>(bundle);
        }

        internal static IEnumerator Initialize()
        {
            SlipLog.Info($"Initializing Assets");

            ParallelCoroutineHelper helper1 = new ParallelCoroutineHelper();

            helper1.Add(LoadAssetBundles);
            helper1.Add(LoadSoundbank);

            helper1.Start();
            while (!helper1.IsDone()) yield return null;

            ParallelCoroutineHelper helper2 = new ParallelCoroutineHelper();
            helper2.Add(SwapShaders);
            helper2.Add(SwapAddressableShaders);

            helper2.Start();
            while (!helper2.IsDone())
                yield return null;

            AssetsAvailability.MakeAvailable();
            yield break;
        }

        private static IEnumerator LoadAssetBundles()
        {
            ParallelCoroutineHelper helper = new ParallelCoroutineHelper();

            List<(string path, SlipBundle bundleEnum, AssetBundle loadedBundle)> pathsAndBundles = new List<(string path, SlipBundle bundleEnum, AssetBundle loadedBundle)>();

            string[] paths = GetAssetBundlePaths();
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                helper.Add(LoadFromPath, pathsAndBundles, path, i, paths.Length);
            }

            helper.Start();
            while (!helper.IsDone())
                yield return null;

            foreach ((string path, SlipBundle bundleEnum, AssetBundle assetBundle) in pathsAndBundles)
            {
                if (bundleEnum == SlipBundle.Invalid)
                {
                    HG.ArrayUtils.ArrayAppend(ref _streamedSceneBundles, assetBundle);
                }
                else
                {
                    _assetBundles[bundleEnum] = assetBundle;
                }
            }
        }

        private static IEnumerator LoadSoundbank()
        {
            byte[] soundBankBytes;

            using (FileStream stream = File.Open(SoundBankPath, FileMode.Open))
            {
                soundBankBytes = new byte[stream.Length];
                var task = stream.ReadAsync(soundBankBytes, 0, soundBankBytes.Length);

                while (!task.IsCompleted)
                    yield return null;
            }

            SoundAPI.SoundBanks.Add(soundBankBytes);
        }

        private static IEnumerator LoadFromPath(List<(string path, SlipBundle bundleEnum, AssetBundle loadedBundle)> list, string path, int index, int totalPaths)
        {
            string fileName = Path.GetFileName(path);
            SlipBundle? slipBundleEnum = null;

            switch (fileName)
            {
                case BASE: slipBundleEnum = SlipBundle.Main; break;
                case EQUIPS: slipBundleEnum = SlipBundle.Equipments; break;
                case ITEMS: slipBundleEnum = SlipBundle.Items; break;
                case ELITES: slipBundleEnum = SlipBundle.Elites; break;
                case SCENES: slipBundleEnum = SlipBundle.Scenes; break;
                case ARIDEXPANSE: slipBundleEnum = SlipBundle.AridExpanse; break;
                case SKILLS: slipBundleEnum = SlipBundle.Skills; break;
                case SKINS: slipBundleEnum = SlipBundle.Skins; break;
                case BASICMONSTERS: slipBundleEnum = SlipBundle.BasicMonsters; break;
                default: slipBundleEnum = SlipBundle.Invalid; break;
            }

            var request = AssetBundle.LoadFromFileAsync(path);
            while (!request.isDone)
                yield return null;

            AssetBundle bundle = request.assetBundle;
            //Throw if no bundle was loaded
            if (!bundle)
            {
                throw new FileLoadException($"AssetBundle.LoadFromFile did not return an asset bundle. (Path={path})");
            }

            //The switch statement considered this a streamed scene bundle
            if (slipBundleEnum == SlipBundle.Invalid)
            {
                //supposed bundle is not streamed scene? throw exception.
                if (!bundle.isStreamedSceneAssetBundle)
                {
                    throw new Exception($"AssetBundle in specified path is not a streamed scene bundle, but its file name was not found in the Switch statement. have you forgotten to setup the enum and file name in your assets class? (Path={path})");
                }
                else
                {
                    //bundle is streamed scene, add to the list and break.
                    list.Add((path, SlipBundle.Invalid, bundle));
                    yield break;
                }
            }

            //The switch statement considered this to not be a streamed scene bundle, but an assets bundle.
            list.Add((path, slipBundleEnum.Value, bundle));
            yield break;
        }


        private static string[] GetAssetBundlePaths()
        {
            return Directory.GetFiles(AssetBundleFolderPath)
               .Where(filePath => !filePath.EndsWith(".manifest"))
               .ToArray();
        }

        private static IEnumerator SwapShaders()
        {
            return ShaderUtil.SwapStubbedShadersAsync(_assetBundles.Values.ToArray());
        }

        private static IEnumerator SwapAddressableShaders()
        {
            return ShaderUtil.LoadAddressableMaterialShadersAsync(_assetBundles.Values.ToArray());
        }

        private static TAsset FindAsset<TAsset>(string name) where TAsset : UnityEngine.Object
        {
            TAsset loadedAsset = null;
            SlipBundle foundInBundle = SlipBundle.Invalid;
            foreach ((var enumVal, var assetBundle) in _assetBundles)
            {
                loadedAsset = assetBundle.LoadAsset<TAsset>(name);

                if (loadedAsset)
                {
                    foundInBundle = enumVal;
                    break;
                }
            }

#if DEBUG
            if (loadedAsset)
                SlipLog.Info($"Asset of type {typeof(TAsset).Name} with name {name} was found inside bundle {foundInBundle}, it is recommended that you load the asset directly.");
            else
                SlipLog.Warning($"Could not find asset of type {typeof(TAsset).Name} with name {name} in any of the bundles.");
#endif

            return loadedAsset;
        }

        private static TAsset[] FindAssets<TAsset>() where TAsset : UnityEngine.Object
        {
            List<TAsset> assets = new List<TAsset>();
            foreach ((_, var bundles) in _assetBundles)
            {
                assets.AddRange(bundles.LoadAllAssets<TAsset>());
            }

#if DEBUG
            if (assets.Count == 0)
                SlipLog.Warning($"Could not find any asset of type {typeof(TAsset).Name} in any of the bundles");
#endif

            return assets.ToArray();
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
                if (declaringType.IsGenericType && declaringType.DeclaringType == typeof(SlipAssets))
                    continue;

                if (declaringType == typeof(SlipAssets))
                    continue;

                var fileName = frame.GetFileName();
                var fileLineNumber = frame.GetFileLineNumber();
                var fileColumnNumber = frame.GetFileColumnNumber();

                return $"{declaringType.FullName}.{method.Name}({GetMethodParams(method)}) (fileName={fileName}, Location=L{fileLineNumber} C{fileColumnNumber})";
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
    }

    public class SlipAssetRequest<TAsset> where TAsset : UObject
    {
        public TAsset Asset => _asset;
        private TAsset _asset;

        public IEnumerable<TAsset> Assets => _assets;
        private List<TAsset> _assets;

        public SlipBundle TargetBundle => _targetBundle;
        private SlipBundle _targetBundle;

        public NullableRef<string> AssetName => _assetName;
        private NullableRef<string> _assetName;

        private bool _singleAssetLoad = true;

        public bool IsComplete => !_internalCoroutine.MoveNext();
        private IEnumerator _internalCoroutine;

        public void StartLoad()
        {
            if (_singleAssetLoad)
            {
                _internalCoroutine = LoadSingleAsset();
            }
            else
            {
                _internalCoroutine = LoadMultipleAsset();
            }
        }

        private IEnumerator LoadSingleAsset()
        {
            AssetBundleRequest request = null;
            if (_targetBundle == SlipBundle.All)
            {
                foreach (SlipBundle enumVal in Enum.GetValues(typeof(SlipBundle)))
                {
                    if (enumVal == SlipBundle.Invalid || enumVal == SlipBundle.All)
                        continue;

                    var bundle = SlipAssets.GetAssetBundle(enumVal);
                    request = bundle.LoadAssetAsync<TAsset>(AssetName);
                    while (!request.isDone)
                    {
                        yield return null;
                    }

                    _asset = (TAsset)request.asset;
                    if (Asset)
                    {
                        _targetBundle = enumVal;
                        yield break;
                    }
                }

#if DEBUG
                if (!Asset)
                {
                    _targetBundle = SlipBundle.Invalid;
                    SlipLog.Warning($"Could not find asset of type {typeof(TAsset).Name} with name {AssetName} in any of the bundles.");
                }
                else
                {
                    SlipLog.Info($"Asset of type {typeof(TAsset).Name} with name {AssetName} was found inside bundle {TargetBundle}, it is recommended that you load the asset directly.");
                }
#endif
                yield break;
            }

            request = SlipAssets.GetAssetBundle(TargetBundle).LoadAssetAsync<TAsset>(AssetName);
            while (!request.isDone)
                yield return null;

            _asset = (TAsset)request.asset;
#if DEBUG
            if (!_asset)
            {
                SlipLog.Warning($"The method \"{GetCallingMethod()}\" is calling a CommissionAssetRequest.StartLoad() while the class has the values \"{typeof(TAsset).Name}\", \"{AssetName}\" and \"{TargetBundle}\", however, the asset could not be found.\n" +
        $"A complete search of all the bundles will be done and the correct bundle enum will be logged.");

                _targetBundle = SlipBundle.All;
                this._internalCoroutine = LoadSingleAsset();
                yield break;
            }
#endif
        }

        private IEnumerator LoadMultipleAsset()
        {
            _assets.Clear();

            AssetBundleRequest request = null;
            if (TargetBundle == SlipBundle.All)
            {
                foreach (SlipBundle enumVal in Enum.GetValues(typeof(SlipBundle)))
                {
                    if (enumVal == SlipBundle.All || enumVal == SlipBundle.Invalid)
                        continue;

                    request = SlipAssets.GetAssetBundle(enumVal).LoadAllAssetsAsync<TAsset>();
                    while (!request.isDone)
                        yield return null;

                    _assets.AddRange(request.allAssets.OfType<TAsset>());
                }

#if DEBUG
                if (_assets.Count == 0)
                {
                    SlipLog.Warning($"Could not find any asset of type {typeof(TAsset).Name} in any of the bundles");
                }
#endif
                yield break;
            }

            request = SlipAssets.GetAssetBundle(TargetBundle).LoadAllAssetsAsync<TAsset>();
            while (!request.isDone) yield return null;

            _assets.AddRange(request.allAssets.OfType<TAsset>());

#if DEBUG
            if (_assets.Count == 0)
            {
                SlipLog.Warning($"Could not find any asset of type {typeof(TAsset)} inside the bundle {TargetBundle}");
            }
#endif

            yield break;
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
                if (declaringType.IsGenericType && declaringType.DeclaringType == typeof(SlipAssets))
                    continue;

                if (declaringType == typeof(SlipAssets))
                    continue;

                var fileName = frame.GetFileName();
                var fileLineNumber = frame.GetFileLineNumber();
                var fileColumnNumber = frame.GetFileColumnNumber();

                return $"{declaringType.FullName}.{method.Name}({GetMethodParams(method)}) (fileName={fileName}, Location=L{fileLineNumber} C{fileColumnNumber})";
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

        internal SlipAssetRequest(string name, SlipBundle bundle)
        {
            _singleAssetLoad = true;
            _assetName = name;
            _targetBundle = bundle;
        }

        internal SlipAssetRequest(SlipBundle bundle)
        {
            _singleAssetLoad = false;
            _assetName = new NullableRef<string>();
            _assets = new List<TAsset>();
            _targetBundle = bundle;
        }
    }
}