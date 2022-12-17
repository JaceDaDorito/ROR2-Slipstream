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

namespace Slipstream.Utils
{
    public class EliteAffixHelper 
    {
        //The bool determines if its the haunted material or not

        private static GameObject hauntedPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteHaunted/PickupEliteHaunted.prefab").WaitForCompletion();
        private static GameObject firePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteFire/PickupEliteFire.prefab").WaitForCompletion();
        public static GameObject CreateAffixModel(Color color, string eliteString, bool haunted)
        {
            GameObject gameObject = PrefabAPI.InstantiateClone(haunted ?  hauntedPrefab : firePrefab, "PickupAffix" + eliteString, false) ;
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
    }
}