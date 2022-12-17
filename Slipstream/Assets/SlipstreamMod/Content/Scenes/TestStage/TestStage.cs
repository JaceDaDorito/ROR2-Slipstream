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
    
    public class TestStage : SceneBase
    {
        //Commented code is what you would do if you wanted to add the stage to a group of stages and add destinations.
        public override SceneDef SceneDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<SceneDef>("TestStage");
        private static MusicTrackDef music = Addressables.LoadAssetAsync<MusicTrackDef>("RoR2/Base/Common/muGameplayBase_09.asset").WaitForCompletion();
        private static MusicTrackDef bossMusic = Addressables.LoadAssetAsync<MusicTrackDef>("RoR2/DLC1/Common/muBossfightDLC1_10.asset").WaitForCompletion(); //change later
        //private static SceneCollection stageThrees = Addressables.LoadAssetAsync<SceneCollection>("RoR2/Base/SceneGroups/sgStage3.asset").WaitForCompletion();
        //private static SceneCollection stageFours = Addressables.LoadAssetAsync<SceneCollection>("RoR2/Base/SceneGroups/sgStage4.asset").WaitForCompletion();
        public override void Initialize()
        {
            SceneDef.mainTrack = music;
            SceneDef.bossTrack = bossMusic;

            /*var sceneEntries = stageThrees._sceneEntries.ToList();
            for (int i = 0; i < sceneEntries.Count; i++)
            {
                var sceneEntry = sceneEntries[i];
                sceneEntry.weightMinusOne = -0.75f;
                sceneEntries[i] = sceneEntry;
            }
            sceneEntries.Add(new SceneCollection.SceneEntry { sceneDef = SceneDef, weightMinusOne = 0 });
            stageThrees._sceneEntries = sceneEntries.ToArray();

            SceneDef.destinationsGroup = stageFours;*/
        }
    }

}
