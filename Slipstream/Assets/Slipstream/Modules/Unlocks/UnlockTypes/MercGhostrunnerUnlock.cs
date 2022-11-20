using RoR2;
using Moonstorm;
using UnityEngine;
using RoR2.Achievements;

namespace Slipstream.Unlocks
{
    public class MercGhostrunnerUnlock : UnlockableBase
    {
        public override MSUnlockableDef UnlockableDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<MSUnlockableDef>("slip.skin.merc.ghostrunner");

        public override void Initialize()
        {
            //base.AddRequiredType<RoR2.CharacterBody.>
        }
        

        /*public override BodyIndex LookUpRequiredBodyIndex()
        {

        }

        public sealed class MercGhostrunnerAchievement : BaseAchievement
        {

        }*/
}
}
