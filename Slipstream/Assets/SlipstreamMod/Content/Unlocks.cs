using Moonstorm;
using R2API.ScriptableObjects;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Slipstream.Modules
{
    public class Unlocks : UnlockablesModuleBase
    {
        public static Unlocks Instance { get; set; }
        public override R2APISerializableContentPack SerializableContentPack => SlipContent.Instance.SerializableContentPack;

        public override void Initialize()
        {
            Instance = this;
            base.Initialize();
            SlipLogger.LogI($"Initializing Unlocks...");
            GetUnlockableBases();
        }

        protected override IEnumerable<UnlockableBase> GetUnlockableBases()
        {
            base.GetUnlockableBases()
                .ToList()
                .ForEach(Unlock => AddUnlockable(Unlock));
            return null;
        }

    }
}