using KinematicCharacterController;
using Moonstorm;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using Slipstream;
using UnityEngine.UI;
using AddressablesHelper;
using UnityEngine.AddressableAssets;
using static RoR2.CharacterModel;
using R2API;

namespace Slipstream
{
    public static class GenericUtils
    {
        private static GameObject hauntedPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteHaunted/PickupEliteHaunted.prefab").WaitForCompletion();
        private static GameObject firePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteFire/PickupEliteFire.prefab").WaitForCompletion();
        public static GameObject CreateAffixModel(Color color, string eliteString, bool haunted)
        {
            GameObject gameObject = PrefabAPI.InstantiateClone(haunted ? hauntedPrefab : firePrefab, "PickupAffix" + eliteString, false);
            Material material = Object.Instantiate<Material>(gameObject.GetComponentInChildren<MeshRenderer>().material);
            material.color = color;
            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
                renderer.material = material;
            return gameObject;
        }
        public static void AddOverlay(CharacterModel model, Material overlayMaterial)
        {
            if (model.activeOverlayCount >= CharacterModel.maxOverlays || !overlayMaterial)
                return;
            Material[] array = model.currentOverlays;
            int num = model.activeOverlayCount;
            model.activeOverlayCount = num + 1;
            array[num] = overlayMaterial;
        }
        public static void OverrideBodyMaterials(CharacterModel model,  Material passedMat, List<CharacterModel.RendererInfo> rendererList = null)
        {
            for (int i = 0; i < model.baseRendererInfos.Length; i++)
            {
                var mat = model.baseRendererInfos[i].defaultMaterial;
                if (mat.shader.name.StartsWith("Hopoo Games/Deferred"))
                {
                    mat = passedMat;
                    model.baseRendererInfos[i].defaultMaterial = mat;
                    if(rendererList != null)
                        rendererList.Add(model.baseRendererInfos[i]);
                }
            }
        }

        public static void AddStacksOfBuff(CharacterBody body, BuffDef buffDef, int count)
        {
            for (int i = 0; i < count; i++)
            {
                body.AddBuff(buffDef);
            }
        }
        public static void RemoveStacksOfBuff(CharacterBody body, BuffDef buffDef, int count)
        {
            for(int i = 0; i < count; i++)
            {
                body.RemoveBuff(buffDef);
            }
        }

        public static float ConvertPercentCursedToCurseInput(float cursePercent)
        {
            float inverse = 1f - cursePercent;
            return (1f / inverse) - 1f;
        }

        public static CharacterBody GetBodyFromInventory(RoR2.Inventory inv)
        {
            CharacterMaster master = inv.GetComponent<CharacterMaster>();
            if (!master)
                return null;
            return master.GetBody();
        }
    }
}
