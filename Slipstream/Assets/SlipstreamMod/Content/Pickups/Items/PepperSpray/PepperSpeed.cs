using MSU;
using RoR2;
using UnityEngine.Networking;
using RoR2.Items;
using R2API;

namespace Slipstream.Buffs
{
    public class PepperSpeed : BuffBase
    {
        //Establishes buff PepperSpeed
        public override BuffDef BuffDef { get; } = SlipAssets.LoadAsset<BuffDef>("PepperSpeed", SlipBundle.Items);
        public static BuffDef buff;
        public override void Initialize()
        {
            buff = BuffDef;
        }

        public class PepperSpeedBehavior: BaseBuffBodyBehavior, IBodyStatArgModifier
        {
            [BuffDefAssociation(useOnClient = true, useOnServer = true)]
            public static BuffDef GetBuffDef() => SlipContent.Buffs.PepperSpeed;

            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                //Post Condition to make sure the buff is applied, for some reason the move speed is bugged unless I do this.
                if (body.HasBuff(buff))
                    args.moveSpeedMultAdd += Items.PepperSpray.speedIncrease;
            }
        }


        /*public override void Initialize()
        {
            buff = BuffDef;
        }

        public override void AddBehavior(ref CharacterBody body, int stack)
        {
            body.AddItemBehavior<PepperSpeedBehavior>(stack);
        }

        public class PepperSpeedBehavior : CharacterBody.ItemBehavior, IBodyStatArgModifier
        {
            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                //Post Condition to make sure the buff is applied, for some reason the move speed is bugged unless I do this.
                if(body.HasBuff(buff))
                    args.moveSpeedMultAdd += Items.PepperSpray.speedIncrease;
            }
        }*/

    }

}
