//using Slipstream.Utils;
using Moonstorm.Loaders;
using R2API;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;
using Path = System.IO.Path;

namespace Slipstream
{
    public class SlipAssets: AssetsLoader<SlipAssets>
    {
        //Loads all assets such as models, effects, and soundbanks (not yet but soon)

        public static ReadOnlyCollection<AssetBundle> assetBundles;
        public override AssetBundle MainAssetBundle => assetBundles[0];
        public string AssemblyDir => Path.GetDirectoryName(SlipMain.pluginInfo.Location);
        //private string SoundBankPath { get => Path.Combine(AssemblyDir, "SlipSoundBnk.bnk"); }

        internal void Init()
        {
            List<AssetBundle> loadedBundles = new List<AssetBundle>();
            var bundlePaths = GetAssetBundlePaths();
            loadedBundles.Add(AssetBundle.LoadFromFile(bundlePaths));

            assetBundles = new ReadOnlyCollection<AssetBundle>(loadedBundles);

            //LoadSoundBank(); I need to make another sound bank

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

        private string GetAssetBundlePaths()
        {
            return Path.Combine(AssemblyDir, "assetbundles", "slipassets");
        }

        /*private void LoadSoundBank()
        {
            byte[] array = File.ReadAllBytes(SoundBankPath);
            SoundAPI.SoundBanks.Add(array);
        }*/
    }
}
