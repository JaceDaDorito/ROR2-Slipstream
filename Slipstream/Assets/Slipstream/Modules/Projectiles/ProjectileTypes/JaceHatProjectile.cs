using Moonstorm;
using R2API;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;


namespace Slipstream.Projectiles
{
    //finish this another time

    //how the fuck do i do projectiles again? (MSU updated)

    //[DisabledContent]
    
    public class JaceHatProjectile : ProjectileBase
    {
        //public override GameObject ProjectilePrefab { get; set; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("projJaceHat");
        public override GameObject ProjectilePrefab { get; } = PrefabAPI.InstantiateClone(SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("JaceHatProjectile"), "HatProjectile", false);

        //public override GameObject ProjectileGhost { get; } = PrefabAPI.InstantiateClone(SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("JaceHatGhost"), "HatProjectile", false);

        public static GameObject hatProj;

        //var model = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("projJaceHat");
        //model.
        public override void Initialize()
        {
            if (ProjectilePrefab)
            {
                hatProj = ProjectilePrefab;
            }
        }
    }

}
