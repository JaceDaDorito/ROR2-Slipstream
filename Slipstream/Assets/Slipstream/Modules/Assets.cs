using Slipstream.Utils;
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

        private const string mainAssetBundle = "slipassets";

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
            var cloudMat = Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/LightningStrikeOrbEffect").transform.Find("Ring").GetComponent<ParticleSystemRenderer>().material;
            materials.ForEach(Material =>
            {
                if (Material.shader.name.StartsWith("StubbedShader"))
                {
                    Material.shader = Resources.Load<Shader>("shaders" + Material.shader.name.Substring(13));
                    if (Material.shader.name.Contains("Cloud Remap"))
                    {
                        var eatShit = new RuntimeCloudMaterialMapper(Material);
                        Material.CopyPropertiesFromMaterial(cloudMat);
                        eatShit.SetMaterialValues(ref Material);
                        HG.ArrayUtils.ArrayAppend(ref cloudRemaps, Material);
                    }
                }
            });
        }


    }
}
