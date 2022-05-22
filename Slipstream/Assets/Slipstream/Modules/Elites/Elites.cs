//using Slipstream.Elites;
using Moonstorm;
using R2API.ScriptableObjects;
using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

/*
namespace Slipstream.Modules
{
    public sealed class Elites : EliteModuleBase
    {
        public static Elites Instance { get; set; }
        public static MSEliteDef[] LoadedSlipElites { get => SlipContent.Instance.SerializableContentPack.eliteDefs as MSEliteDef[]; }
        public override AssetBundle AssetBundle => SlipAssets.Instance.MainAssetBundle;
        public override R2APISerializableContentPack SerializableContentPack => SlipContent.Instance.SerializableContentPack;

        public override void Initialize()
        {
            Instance = this;
            base.Initialize();
            SlipLogger.LogI($"Initializing Slipstream Elites...");
            OnListCreated += LateEliteSetup;
        }
    }
}
*/