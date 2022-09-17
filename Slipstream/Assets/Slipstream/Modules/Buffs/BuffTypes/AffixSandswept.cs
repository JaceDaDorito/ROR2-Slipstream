using Moonstorm;
using Moonstorm.Components;
using R2API;
using RoR2;
using System.Linq;
using UnityEngine;

namespace Slipstream.Buffs
{
    public class AffixSandswept : BuffBase
    {
        public override BuffDef BuffDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<BuffDef>("AffixSandswept");

        public class AffixSandsweptBehavior : BaseBuffBodyBehavior, IOnIncomingDamageOtherServerReciever
        {
            [BuffDefAssociation(useOnClient = true, useOnServer = true)]
            public static BuffDef GetBuffDef() => SlipContent.Buffs.AffixSandswept;


            public void OnIncomingDamageOther(HealthComponent victimHealthComponent, DamageInfo damageInfo)
            {
                damageInfo.damageType = DamageType.Stun1s;
            }
        }
    }
}