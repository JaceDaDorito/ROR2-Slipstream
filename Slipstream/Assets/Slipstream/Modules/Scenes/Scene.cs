using Moonstorm;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Slipstream.Modules
{
    public class Scenes : SceneModuleBase
    {
        public static Scenes Instance { get; set; }
        public override SerializableContentPack ContentPack { get; set; } = SlipContent.serializableContentPack;

        public override void Init()
        {
            Instance = this;
            base.Init();
            InitializeScenes();
        }

        public override IEnumerable<SceneBase> InitializeScenes()
        {
            base.InitializeScenes()
            .Where(scene => SlipMain.instance.Config.Bind<bool>($"{scene.SceneDef.cachedName}", $"Enable {scene.SceneDef.cachedName}", true, "Enable/Disable this Stage.").Value)
            .ToList()
            .ForEach(scene => AddScene(scene, ContentPack));
            return null;
        }
    }
}
