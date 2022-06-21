using Moonstorm;
using RoR2;
using UnityEngine.Networking;
using RoR2.Items;
using Moonstorm.Components;
using R2API;

namespace Slipstream.Buffs
{
    public class BrineBuff : BuffBase
    {
        //Establishes buff PepperSpeed
        public override BuffDef BuffDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<BuffDef>("BrineBuff");
        public static BuffDef buff;
        public override void Initialize()
        {
            buff = BuffDef;
        }

        public class BrineBuffBehavior : BaseBuffBodyBehavior, IBodyStatArgModifier
        {
            [BuffDefAssociation(useOnClient = true, useOnServer = true)]
            public static BuffDef GetBuffDef() => SlipContent.Buffs.BrineBuff;

            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                //Post Condition to make sure the buff is applied, for some reason the move speed is bugged unless I do this.
                if (body.HasBuff(buff))
                {
                    args.armorAdd += Items.BrineSwarm.armorIncrease;
                    args.damageMultAdd += Items.BrineSwarm.damageIncrease;
                }
            }
        }
    }
}
