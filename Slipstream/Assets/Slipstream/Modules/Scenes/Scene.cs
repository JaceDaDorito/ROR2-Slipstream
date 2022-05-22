using Moonstorm;
using R2API.ScriptableObjects;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Slipstream.Modules
{
    public class Scenes : SceneModuleBase
    {
        public static Scenes Instance { get; set; }
        public override R2APISerializableContentPack SerializableContentPack => SlipContent.Instance.SerializableContentPack;

        public override void Initialize()
        {
            Instance = this;
            base.Initialize();
            SlipLogger.LogI($"Initializing Scenes...");
            GetSceneBases();
        }

        public override IEnumerable<SceneBase> GetSceneBases()
        {
            base.GetSceneBases()
                .ToList()
                .ForEach(Scenes => AddScene(Scenes));
            return null;
        }
    }
}
