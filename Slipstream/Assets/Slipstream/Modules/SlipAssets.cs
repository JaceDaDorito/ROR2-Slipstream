//using Slipstream.Utils;
using Moonstorm.Loaders;
using Slipstream.PostProcess;
using R2API;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Path = System.IO.Path;

namespace Slipstream
{
    public class SlipAssets: AssetsLoader<SlipAssets>
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

        /*internal void LoadEffectDefs()
        {
            SlipContent.Instance.SerializableContentPack.effectDefs = LoadEffectDefsFromHolders(MainAssetBundle);
            SlipContent.Instance.SerializableContentPack.effectDefs = LoadEffectDefsFromPrefabs(MainAssetBundle);
        }*/

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


        //copied this part from SS2
        private void LoadPostProcessing()
        {
            var ppProfiles = MainAssetBundle.LoadAllAssets<PostProcessProfile>();
            foreach (var ppProfile in ppProfiles)
            {
                SlipRampFog tempFog;
                SlipSobelOutline tempOutline;
                SlipSobelRain tempRain;
                bool modified = false;
                if (ppProfile.TryGetSettings(out tempFog))
                {
                    var fog = ppProfile.AddSettings<RampFog>();
                    fog.enabled = tempFog.enabled;
                    fog.active = tempFog.active;
                    fog.fogIntensity = tempFog.fogIntensity;
                    fog.fogPower = tempFog.fogPower;
                    fog.fogZero = tempFog.fogZero;
                    fog.fogOne = tempFog.fogOne;
                    fog.fogHeightStart = tempFog.fogHeightStart;
                    fog.fogHeightEnd = tempFog.fogHeightEnd;
                    fog.fogHeightIntensity = tempFog.fogHeightIntensity;
                    fog.fogColorStart = tempFog.fogColorStart;
                    fog.fogColorMid = tempFog.fogColorMid;
                    fog.fogColorEnd = tempFog.fogColorEnd;
                    fog.skyboxStrength = tempFog.skyboxStrength;
                    ppProfile.RemoveSettings(typeof(SlipRampFog));
                    modified = true;
                }
                if (ppProfile.TryGetSettings(out tempOutline))
                {
                    var outline = ppProfile.AddSettings<SobelOutline>();
                    outline.enabled = tempOutline.enabled;
                    outline.active = tempOutline.active;
                    outline.outlineIntensity = tempOutline.outlineIntensity;
                    outline.outlineScale = tempOutline.outlineScale;
                    ppProfile.RemoveSettings(typeof(SlipSobelOutline));
                    modified = true;
                }
                if (ppProfile.TryGetSettings(out tempRain))
                {
                    var rain = ppProfile.AddSettings<SobelRain>();
                    rain.enabled = tempRain.enabled;
                    rain.active = tempRain.active;
                    rain.rainIntensity = tempRain.rainIntensity;
                    rain.outlineScale = tempRain.outlineScale;
                    rain.rainDensity = tempRain.rainDensity;
                    rain.rainTexture = tempRain.rainTexture;
                    rain.rainColor = tempRain.rainColor;
                    ppProfile.RemoveSettings(typeof(SlipSobelRain));
                    modified = true;
                }
                ppProfile.isDirty = modified;
            }
        }

        /*private void LoadSoundBank()
        {
            byte[] array = File.ReadAllBytes(SoundBankPath);
            SoundAPI.SoundBanks.Add(array);
        }*/
    }
}
