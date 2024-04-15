using KinematicCharacterController;
using MSU;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using Slipstream;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using static RoR2.CharacterModel;
using R2API;
using Slipstream.Buffs;


namespace Slipstream.Equipments
{
    public class AffixSandswept : EliteEquipmentBase
    {
        public override List<MSEliteDef> EliteDefs { get; } = new List<MSEliteDef>
        {
            SlipAssets.LoadAsset<MSEliteDef>("Sandswept", SlipBundle.Elites),
            SlipAssets.LoadAsset<MSEliteDef>("SandsweptHonor", SlipBundle.Elites)
        };
        public override EquipmentDef EquipmentDef { get; } = SlipAssets.LoadAsset<EquipmentDef>("AffixSandswept", SlipBundle.Elites);

        private Color eliteColor = SlipUtils.ColorRGB(247f, 196f, 119f, 1f); //new Color(0.96862f, 0.76862f, 0.46666f, 1f);
        public override void Initialize()
        {
            EquipmentDef.pickupModelPrefab = SlipUtils.CreateAffixModel(eliteColor, "Sand", false);
        }
        public override bool FireAction(EquipmentSlot slot)
        {
            return false;
        }

        
        
    }
}