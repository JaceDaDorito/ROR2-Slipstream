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
        public static SceneDef scene;
        private static MusicTrackDef music = Resources.Load<MusicTrackDef>("musictrackdefs/muGameplayBase_09");

        public override void Initialize()
        {
            scene = SceneDef;
            scene.mainTrack = music;
        }
    }

}
