using Moonstorm;
using R2API.ScriptableObjects;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Slipstream.Modules
{
    public class Projectiles : ProjectileModuleBase
    {
        public static Projectiles Instance { get; set; }
        public override R2APISerializableContentPack SerializableContentPack => SlipContent.Instance.SerializableContentPack;
        public override void Initialize()
        {
            Instance = this;
            base.Initialize();
            SlipLogger.LogI($"Initializing Projectiles...");
            GetProjectileBases();
        }
        //sends projectiles to be iterated and populated.
        protected override IEnumerable<ProjectileBase> GetProjectileBases()
        {
            base.GetProjectileBases()
                .ToList()
                .ForEach(proj => AddProjectile(proj));
            return null;
        }
    }
}
