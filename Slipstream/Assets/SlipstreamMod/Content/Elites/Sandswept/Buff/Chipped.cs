using Moonstorm;
using Moonstorm.Components;
using R2API;
using RoR2;
using System.Linq;
using UnityEngine;
using EntityStates.Sandswept;
using EntityStates;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using Slipstream.Components;
using KinematicCharacterController;
using RoR2.Projectile;


namespace Slipstream.Buffs
{
    public class Chipped : BuffBase
    {
        public override BuffDef BuffDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<BuffDef>("Chipped");
        public static BuffDef buff;

        public static float chippedAmount = GenericUtils.ConvertPercentCursedToCurseInput(AffixSandswept.chippedPercentage);
        public static float nerfedChippedAmount = GenericUtils.ConvertPercentCursedToCurseInput(AffixSandswept.nerfedChippedPercentage);

        public static Material glassMaterial = SlipAssets.Instance.MainAssetBundle.LoadAsset<Material>("matIsGlass");
        private static float flashDuration = 1f;
        public override void Initialize()
        {
            buff = BuffDef;
        }

        public sealed class ChippedBehavior : BaseBuffBodyBehavior, IBodyStatArgModifier
        {
            [BuffDefAssociation(useOnClient = true, useOnServer = true)]
            private static BuffDef GetBuffDef() => SlipContent.Buffs.Chipped;
            private TemporaryOverlay temporaryOverlay;
            private TemporaryOverlay glassOverlay;
            private CharacterModel model;

            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                if (body.HasBuff(buff))
                {
                    switch (body.teamComponent.teamIndex)
                    {
                        case TeamIndex.Player:
                            args.baseCurseAdd += chippedAmount;
                            break;
                        default:
                            args.baseCurseAdd += nerfedChippedAmount;
                            break;
                    }
                    
                }
                    
            }

            public void Start()
            {
                model = body.modelLocator.modelTransform.GetComponent<CharacterModel>();
               
                if (model)
                {
                    temporaryOverlay = new TemporaryOverlay();
                    temporaryOverlay = body.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = flashDuration;
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.destroyObjectOnEnd = false;
                    temporaryOverlay.originalMaterial = glassMaterial;
                    temporaryOverlay.inspectorCharacterModel = model;
                    temporaryOverlay.alphaCurve = AnimationCurve.Linear(0f, 1f, flashDuration, 0f);
                    temporaryOverlay.animateShaderAlpha = true;

                    glassOverlay = new TemporaryOverlay();
                    glassOverlay = body.gameObject.AddComponent<TemporaryOverlay>();
                    glassOverlay.duration = flashDuration; //doesn't matter what you put here
                    glassOverlay.destroyComponentOnEnd = false;
                    glassOverlay.destroyObjectOnEnd = false;
                    glassOverlay.originalMaterial = glassMaterial;
                    glassOverlay.inspectorCharacterModel = model;
                    //GenericUtils.OverrideBodyMaterials(model, glassMaterial);
                }
            }

            public void OnDestroy()
            {
                if (glassOverlay)
                {
                    Destroy(glassOverlay);
                    glassOverlay = null;
                }
            }

        }
    }
}
