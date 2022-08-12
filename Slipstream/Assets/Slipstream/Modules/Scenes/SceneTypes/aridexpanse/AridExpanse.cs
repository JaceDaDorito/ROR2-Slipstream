using Moonstorm;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

namespace Slipstream.Scenes
{
    public class AridExpanse : SceneBase
    {
        public override SceneDef SceneDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<SceneDef>("aridexpanse");

        private static MusicTrackDef music = Addressables.LoadAssetAsync<MusicTrackDef>("RoR2/Base/Common/muSong13.asset").WaitForCompletion();
        private static MusicTrackDef bossMusic = Addressables.LoadAssetAsync<MusicTrackDef>("RoR2/Base/Common/muSong05.asset").WaitForCompletion();
        private static SceneCollection sceneEntryGroup = Addressables.LoadAssetAsync<SceneCollection>("RoR2/Base/SceneGroups/sgStage1.asset").WaitForCompletion(); //Stage 1s
        private static SceneCollection sceneDestinations = Addressables.LoadAssetAsync<SceneCollection>("RoR2/Base/SceneGroups/sgStage2.asset").WaitForCompletion(); // Stage 2s

        /*private static DirectorCardCategorySelection interactablesNoDLC { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<DirectorCardCategorySelection>("dccsAridExpanseInteractables");
        private static DirectorCardCategorySelection interactablesDLC1 { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<DirectorCardCategorySelection>("dccsAridExpanseInteractablesDLC1");
        private static DirectorCardCategorySelection monstersNoDLC { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<DirectorCardCategorySelection>("dccsAridExpanseMonsters");
        private static DirectorCardCategorySelection monstersDLC1 { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<DirectorCardCategorySelection>("dccsAridExpanseMonstersDLC1");

        private static DccsPool interactablesPool { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<DccsPool>("dpAridExpanseInteractables");*/
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

        }
    }

}
