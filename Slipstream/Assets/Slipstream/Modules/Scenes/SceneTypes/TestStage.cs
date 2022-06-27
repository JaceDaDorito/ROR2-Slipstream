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

namespace Slipstream.Scenes
{
    public class TestStage : SceneBase
    {
        public override SceneDef SceneDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<SceneDef>("TestStage");
        private static MusicTrackDef music = Addressables.LoadAssetAsync<MusicTrackDef>("RoR2/Base/Common/muGameplayBase_09.asset").WaitForCompletion();

        public override void Initialize()
        {
            SceneDef.mainTrack = music;
        }
    }

}
