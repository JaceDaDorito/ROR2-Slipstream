using Moonstorm;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;

namespace Slipstream.Equipments
{
    [DisabledContent]
    public class Incubator : EquipmentBase
    {
        public override EquipmentDef EquipmentDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<EquipmentDef>("Incubator");

        public override bool FireAction(EquipmentSlot slot)
        {
            return true;
        }
    }
}
