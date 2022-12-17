using Moonstorm;
using RoR2;
using UnityEngine.Networking;
using RoR2.Items;
using Moonstorm.Components;
using R2API;

namespace Slipstream.Buffs
{
    public class SoulRoot : BuffBase
    {
        public override BuffDef BuffDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<BuffDef>("SoulRoot");

    }

}
