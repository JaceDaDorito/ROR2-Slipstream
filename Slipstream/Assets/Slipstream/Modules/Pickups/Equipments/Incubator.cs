using Moonstorm;
using On.RoR2;
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
        public override RoR2.EquipmentDef EquipmentDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<RoR2.EquipmentDef>("Incubator");

        public override void Initialize()
        {
            base.Initialize();
            On.RoR2.EquipmentSlot.UpdateTargets += new On.RoR2.EquipmentSlot.hook_UpdateTargets(EquipmentSlot_UpdateTargets);
        }

        private void EquipmentSlot_UpdateTargets(On.RoR2.EquipmentSlot.orig_UpdateTargets orig, RoR2.EquipmentSlot self, RoR2.EquipmentIndex targetingEquipmentIndex, bool userShouldAnticipateTarget)
        {
            if(targetingEquipmentIndex == EquipmentDef.equipmentIndex && userShouldAnticipateTarget)
            {
                //EquipmentSlot.
            }
        }

        public override bool FireAction(RoR2.EquipmentSlot slot)
        {
            return true;
        }
    }
}
