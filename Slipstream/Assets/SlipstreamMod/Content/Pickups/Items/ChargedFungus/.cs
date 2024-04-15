using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSU;
using RoR2;
using R2API;
using Moonstorm.Components;
using RoR2.Items;
using Slipstream.Items;
namespace Slipstream.Buffs
{
    public class ChungusBuff : BuffBase
    {
        public override BuffDef BuffDef => SlipAssets.LoadAsset<BuffDef>("ChungusBuff", SlipBundle.Items);

        public static BuffDef buff;

        public override void Initialize()
        {
            buff = BuffDef;
        }
            }
    
    
}