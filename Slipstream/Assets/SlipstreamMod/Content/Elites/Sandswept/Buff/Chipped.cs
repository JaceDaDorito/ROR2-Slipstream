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
        public override void Initialize()
        {
            buff = BuffDef;
        }

        public sealed class ChippedBehavior : BaseBuffBodyBehavior, IBodyStatArgModifier
        {
            [BuffDefAssociation]
            private static BuffDef GetBuffDef() => SlipContent.Buffs.Chipped;

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

        }
    }
}
