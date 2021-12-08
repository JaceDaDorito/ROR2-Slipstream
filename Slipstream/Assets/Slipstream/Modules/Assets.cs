using Moonstorm;
using R2API;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Path = System.IO.Path;

namespace Slipstream
{
    internal static class Assets
    {
        internal static string AssemblyDir
        {
            get
            {
                return Path.GetDirectoryName(SlipMain.pluginInfo.Location);
            }
        }

        private static string MainAssetBundlePath { get => Path.Combine(AssemblyDir, mainAssetBundle); }

        private const string mainAssetBundle = "SlipAssets";

        public static AssetBundle LITAssets { get; } = AssetBundle.LoadFromFile(MainAssetBundlePath);

        public static Material[] cloudRemaps = Array.Empty<Material>();

        internal static void Init()
        {

        }
    }
}
