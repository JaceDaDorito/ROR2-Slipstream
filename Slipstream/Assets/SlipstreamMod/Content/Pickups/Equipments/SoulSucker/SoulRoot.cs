using MSU;
using RoR2;
using UnityEngine.Networking;
using RoR2.Items;
using R2API;

namespace Slipstream.Buffs
{
    public class SoulRoot : BuffBase
    {
        public override BuffDef BuffDef { get; } = SlipAssets.LoadAsset<BuffDef>("SoulRoot", SlipBundle.Equipments);

    }

}
