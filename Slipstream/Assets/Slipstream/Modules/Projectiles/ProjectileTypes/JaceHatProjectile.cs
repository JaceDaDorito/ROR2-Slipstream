using Moonstorm;
using R2API;
using RoR2.Projectile;
using UnityEngine;

namespace Slipstream.Projectiles
{
    [DisabledContent]
    public class JaceHatProjectile : ProjectileBase
    {
        public override GameObject ProjectilePrefab { get; set; } = PrefabAPI.InstantiateClone(SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("projJaceHat"), "HatProjectile");
        public static GameObject HatProj;
        public override void Initialize()
        {
            if (ProjectilePrefab)
            {

            }
        }
    }

}
