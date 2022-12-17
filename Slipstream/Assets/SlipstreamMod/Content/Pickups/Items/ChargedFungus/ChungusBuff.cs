using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonstorm;
using RoR2;
using R2API;
using Moonstorm.Components;
using RoR2.Items;
using Slipstream.Items;
namespace Slipstream.Buffs
{
    public class ChungusBuff : BuffBase
    {
        public override BuffDef BuffDef => SlipAssets.Instance.MainAssetBundle.LoadAsset<BuffDef>("ChungusBuff");

        public static BuffDef buff;

        public override void Initialize()
        {
            buff = BuffDef;
        }
        public class ChungusBuffBehavior : BaseBuffBodyBehavior
        {
            [BuffDefAssociation(useOnClient = true, useOnServer = true)]
            public static BuffDef GetBuffDef() => SlipContent.Buffs.ChungusBuff;
            public float healingtime = 1f;
            

            private void FixedUpdate()
            {
                healingtime -= Time.fixedDeltaTime;
                

                if (healingtime <= 0f)
                {
                    healingtime = 1f;
                    body.healthComponent.HealFraction(ChargedFungus.HealingPercentage, default);
                    

                }
            }
        }
    }
    
    
}