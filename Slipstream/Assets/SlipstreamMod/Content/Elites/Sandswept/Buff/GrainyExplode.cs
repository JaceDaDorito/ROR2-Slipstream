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
    public class GrainyExplode : BuffBase
    {
        public override BuffDef BuffDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<BuffDef>("GrainyExplode");
        public static DotController.DotIndex index;
        public static BuffDef buff;
        public override void Initialize()
        {
            index = DotAPI.RegisterDotDef(3f, 4f, DamageColorIndex.Item, BuffDef);
            buff = BuffDef;
        }

        public sealed class GrainyExplodeBehavior : BaseBuffBodyBehavior, IOnTakeDamageServerReceiver
        {
            [BuffDefAssociation]
            private static BuffDef GetBuffDef() => SlipContent.Buffs.GrainyExplode;

            public void OnTakeDamageServer(DamageReport damageReport)
            {
                DotController.DotIndex dotType = damageReport.dotType;
                if(dotType == index && !AffixSandswept.BlacklistedBodyIndices.Contains(body.bodyIndex))
                {
                    KinematicCharacterMotor indexKCC = body.GetComponent<KinematicCharacterMotor>();
                    if (indexKCC)
                        indexKCC.ForceUnground();

                    PhysForceInfo physInfo = new PhysForceInfo()
                    {
                        force = Vector3.up * AffixSandswept.debuffUpwardForce,
                        ignoreGroundStick = true,
                        disableAirControlUntilCollision = false,
                        massIsOne = false//MassIsOne just means if mass will have a factor in the force. I set it to false because I want the mass to contribute.
                    };
                    body.characterMotor.ApplyForceImpulse(physInfo);
                }
            }
        }
    }
}
