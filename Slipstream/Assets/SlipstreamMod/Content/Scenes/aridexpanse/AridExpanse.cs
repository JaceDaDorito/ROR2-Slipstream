using Moonstorm;
using Moonstorm.Components;
using Slipstream.Buffs;
using RoR2;
using RoR2.ExpansionManagement;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;
using System.Collections;
using RoR2.ContentManagement;

namespace Slipstream.Scenes
{
    public class AridExpanse : SceneBase
    {
        public override SceneDef SceneDef { get; } = SlipAssets.LoadAsset<SceneDef>("aridexpanse", SlipBundle.Scenes);


        private static MusicTrackDef music = Addressables.LoadAssetAsync<MusicTrackDef>("RoR2/Base/Common/muFULLSong02.asset").WaitForCompletion();
        private static MusicTrackDef bossMusic = Addressables.LoadAssetAsync<MusicTrackDef>("RoR2/Base/Common/muSong05.asset").WaitForCompletion();
        private static SceneCollection sceneEntryGroup = Addressables.LoadAssetAsync<SceneCollection>("RoR2/Base/SceneGroups/sgStage1.asset").WaitForCompletion(); //Stage 1s
        private static SceneCollection sceneDestinations = Addressables.LoadAssetAsync<SceneCollection>("RoR2/Base/SceneGroups/sgStage2.asset").WaitForCompletion(); // Stage 2s
        private static DccsPool intPool { get; } = SlipAssets.LoadAsset<DccsPool>("dpAridExpanseInteractables", SlipBundle.Scenes);
        private static DccsPool monsterPool { get; } = SlipAssets.LoadAsset<DccsPool>("dpAridExpanseMons", SlipBundle.Scenes);

        private static Material golemPlainsBazaarMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/bazaar/matBazaarSeerGolemplains.mat").WaitForCompletion();
        private static Material bazaarMat;
        public override void Initialize()
        {
            SceneDef.mainTrack = music;
            SceneDef.bossTrack = bossMusic;

            //Iterates through all stages on the same "tier" as this stage an adds this stage to that "tier"
            var sceneEntries = sceneEntryGroup._sceneEntries.ToList();
            for (int i = 0; i < sceneEntries.Count; i++)
            {
                var sceneEntry = sceneEntries[i];
                sceneEntry.weightMinusOne = -0.75f;
                sceneEntries[i] = sceneEntry;
            }
            sceneEntries.Add(new SceneCollection.SceneEntry { sceneDef = SceneDef, weightMinusOne = 0 });
            sceneEntryGroup._sceneEntries = sceneEntries.ToArray();


            //Adds destinations to this stage
            SceneDef.destinationsGroup = sceneDestinations;

            bazaarMat = UnityEngine.Object.Instantiate(golemPlainsBazaarMat);
            bazaarMat.mainTexture = SlipAssets.LoadAsset<Texture>("Capture", SlipBundle.Base);
            SceneDef.portalMaterial = bazaarMat;

            //Makes dlc a requirement for dlc content
            ExpansionDef[] arrayExpansions = { Addressables.LoadAssetAsync<ExpansionDef>("RoR2/DLC1/Common/DLC1.asset").WaitForCompletion() };

            ref DccsPool.Category intStandard = ref intPool.poolCategories[0];
            intStandard.includedIfConditionsMet[0].requiredExpansions = arrayExpansions;

            /*ref DccsPool.Category monStandard = ref monsterPool.poolCategories[0];
            monStandard.includedIfConditionsMet[0].requiredExpansions = arrayExpansions;*/

            //SceneDef.dioramaPrefab.AddComponent<MaterialControllerComponents.>();
        }
    }

}
