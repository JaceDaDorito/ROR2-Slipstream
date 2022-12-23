using RoR2;
using Moonstorm;
using UnityEngine;
using RoR2.Achievements;

namespace Slipstream.Unlocks
{
    public class MercGhostrunnerUnlock : UnlockableBase
    {
        public override MSUnlockableDef UnlockableDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<MSUnlockableDef>("slip.skin.merc.ghostrunner");

        public sealed class MercGhostrunnerAchievement : BaseAchievement
        {
            public override BodyIndex LookUpRequiredBodyIndex()
            {
                return BodyCatalog.FindBodyIndex("MercBody");
            }

            public override void OnBodyRequirementMet()
            {
                base.OnBodyRequirementMet();
                base.SetServerTracked(true);
            }
            public override void OnBodyRequirementBroken()
            {
                base.SetServerTracked(false);
                base.OnBodyRequirementBroken();
            }

            private static readonly int requirement = 2;

            private class MercGhostrunnerServerAchievement : BaseServerAchievement
            {

                private bool increment;

                public override void OnInstall()
                {
                    base.OnInstall();
                    counter = 0;
                    increment = true;
                    GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeathGlobal;
                    Stage.onServerStageComplete += Stage_onServerStageComplete;
                    Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
                }


                public override void OnUninstall()
                {
                    GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeathGlobal;
                    Stage.onServerStageComplete -= Stage_onServerStageComplete;
                    Run.onRunDestroyGlobal -= Run_onRunDestroyGlobal;
                    base.OnUninstall();
                }
                private void OnCharacterDeathGlobal(DamageReport damageReport)
                {
                    if(damageReport.attackerMaster && networkUser.master == damageReport.attackerMaster && damageReport.victim && !damageReport.victimIsBoss)
                    {
                        counter = 0;
                        increment = false;
                    }
                }
                private void Stage_onServerStageComplete(Stage obj)
                {
                    if(increment)
                        counter++;
                    increment = true;
                    //SlipLogger.LogD("&Current Ghostrunner Count: " + counter);
                    if (counter >= 2)
                        base.Grant();
                }

                private void Run_onRunDestroyGlobal(Run obj)
                {
                    counter = 0;
                    increment = true;
                }

                private int counter;
            }
        }
    }
}
