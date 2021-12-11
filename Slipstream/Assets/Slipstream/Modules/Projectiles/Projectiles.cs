using Moonstorm;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Slipstream.Modules
{
    public class Projectiles : ProjectileModuleBase
    {
        public static Projectiles Instance { get; set; }
        public override SerializableContentPack ContentPack { get; set; } = SlipContent.serializableContentPack;
        public override AssetBundle AssetBundle { get; set; } = Assets.SlipAssets;
        public override void Init()
        {
            Instance = this;
            base.Init();
            InitializeProjectiles();
        }
        //sends projectiles to be iterated and populated.
        public override IEnumerable<ProjectileBase> InitializeProjectiles()
        {
            base.InitializeProjectiles().ToList().ForEach(proj => AddProjectile ( proj, ContentPack));
            return null;
        }
    }
}
