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

        public static AssetBundle SlipAssets { get; } = AssetBundle.LoadFromFile(MainAssetBundlePath);

        public static Material[] cloudRemaps = Array.Empty<Material>();

        internal static void Init()
        {
            SlipContent.serializableContentPack = SlipAssets.LoadAsset<SerializableContentPack>("ContentPack");
            SwapShaders(SlipAssets.LoadAllAssets<Material>().ToList());

            //Effect and Soundbank loading goes here.
        }


        private static void SwapShaders(List<Material> materials)
        {
            materials.ForEach(Material =>
            {
                if (Material.shader.name.StartsWith("StubbedShader"))
                {
                    Material.shader = Resources.Load<Shader>("shaders" + Material.shader.name.Substring(13));
                    /*if (Material.shader.name.Contains("Cloud Remap"))
                    {
                        this relates to cloudremap shader, don't worry about it just yet I will figure this out trust me.
                    }*/
                }
            });
        }


    }
}
