using KinematicCharacterController;
using Moonstorm;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using Slipstream;
using UnityEngine.UI;


namespace Slipstream.Equipments
{
    public class AffixSandswept : EliteEquipmentBase
    {
        public override List<MSEliteDef> EliteDefs { get; } = new List<MSEliteDef>
        {
            SlipAssets.Instance.MainAssetBundle.LoadAsset<MSEliteDef>("Sandswept"),
            SlipAssets.Instance.MainAssetBundle.LoadAsset<MSEliteDef>("SandsweptHonor")
        };
        public override EquipmentDef EquipmentDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<EquipmentDef>("AffixSandswept");
        public override void Initialize()
        {
            Slipstream.SlipstreamEliteRamps.SlipstreamEliteRamp item = new SlipstreamEliteRamps.SlipstreamEliteRamp();
            item.eliteDef = EliteDefs[0];
            item.rampTexture = EliteDefs[0].eliteRamp;
            SlipstreamEliteRamps.eliteRamps.Add(item);

            //item.eliteDef = EliteDefs[1];
            //SlipstreamEliteRamps.eliteRamps.Add(item);
        }

        public override bool FireAction(EquipmentSlot slot)
        {
            return false;
        }
    }
}