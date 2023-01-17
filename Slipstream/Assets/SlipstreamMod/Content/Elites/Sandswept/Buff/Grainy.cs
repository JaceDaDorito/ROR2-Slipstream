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
using Slipstream.Orbs;
using RoR2.Orbs;

namespace Slipstream.Buffs
{
    public class Grainy : BuffBase
    {
        public override BuffDef BuffDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<BuffDef>("Grainy");
        public static BuffDef buff;
        public override void Initialize()
        {
            buff = BuffDef;
        }
        public class GrainyBehaviour : BaseBuffBodyBehavior, IOnKilledOtherServerReceiver
        {
            [BuffDefAssociation(useOnClient = true, useOnServer = true)]
            public static BuffDef GetBuffDef() => SlipContent.Buffs.Grainy;

            public void OnKilledOtherServer(DamageReport damageReport)
            {
                CharacterBody victimBody = damageReport.victimBody;
                if(body.HasBuff(buff) && victimBody && victimBody.HasBuff(AffixSandswept.buff))
                {
                    SandsweptDeathOrb sandsweptDeathOrb = new SandsweptDeathOrb();
                    sandsweptDeathOrb.origin = victimBody.corePosition;
                    sandsweptDeathOrb.target = Util.FindBodyMainHurtBox(body);
                    OrbManager.instance.AddOrb(sandsweptDeathOrb);
                }
            }
        }
    }
}
