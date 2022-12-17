using KinematicCharacterController;
using Moonstorm;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using Slipstream;
using UnityEngine.UI;
using AddressablesHelper;
using UnityEngine.AddressableAssets;
using static RoR2.CharacterModel;
using R2API;
using Slipstream.Utils;
using Slipstream.Buffs;


namespace Slipstream.Equipments
{
    public class AffixSandswept : EliteEquipmentBase
    {
        public override List<MSEliteDef> EliteDefs { get; } = new List<MSEliteDef>
        {
            SlipAssets.Instance.MainAssetBundle.LoadAsset<MSEliteDef>("Sandswept"),
            SlipAssets.Instance.MainAssetBundle.LoadAsset<MSEliteDef>("SandsweptHonor")
        };

        public Material overlayMat = SlipAssets.Instance.MainAssetBundle.LoadAsset<Material>("matEliteSandOverlay");
        public override EquipmentDef EquipmentDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<EquipmentDef>("AffixSandswept");

        private Color eliteColor = ColorCatalog.ColorRGB(247f, 196f, 119f, 1f); //new Color(0.96862f, 0.76862f, 0.46666f, 1f);
        public override void Initialize()
        {
            On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
            //look guys I did IL


            Slipstream.SlipstreamEliteRamps.SlipstreamEliteRamp item = new SlipstreamEliteRamps.SlipstreamEliteRamp();
            item.eliteDef = EliteDefs[0];
            item.rampTexture = EliteDefs[0].eliteRamp;
            SlipstreamEliteRamps.eliteRamps.Add(item);

            EquipmentDef.pickupModelPrefab = EliteAffixHelper.CreateAffixModel(eliteColor, "Sand", false);

            //item.eliteDef = EliteDefs[1];
            //SlipstreamEliteRamps.eliteRamps.Add(item);
        }

        

        private void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig(self);
            if (!self.body)
                return;
            Buffs.AffixSandswept.AffixSandsweptBehavior component = self.body.GetComponent<Buffs.AffixSandswept.AffixSandsweptBehavior>();
            if (!component)
                return;
            if (self.myEliteIndex == EliteDefs[0].eliteIndex && !component.isGlass)
                EliteAffixHelper.AddOverlay(self, overlayMat);
        }

        public override bool FireAction(EquipmentSlot slot)
        {
            return false;
        }

        
        
    }
}