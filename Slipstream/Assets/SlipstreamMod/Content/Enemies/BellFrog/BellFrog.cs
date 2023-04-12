using RoR2;
using UnityEngine;
using Moonstorm;
using EntityStates;

namespace Slipstream.Characters
{
    public sealed class BellFrog : MonsterBase
    {
        public override MSMonsterDirectorCard MonsterDirectorCard { get; } = SlipAssets.LoadAsset<MSMonsterDirectorCard>("msmdcBellFrog", SlipBundle.BasicMonsters);

        public override GameObject BodyPrefab { get; } = SlipAssets.LoadAsset<GameObject>("BellFrogBody", SlipBundle.BasicMonsters);

        public override GameObject MasterPrefab { get; } = SlipAssets.LoadAsset<GameObject>("BellFrogMaster", SlipBundle.BasicMonsters);
    }

    internal class SpawnState : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("Body", "Spawn");
            Util.PlaySound("Play_bellBody_spawn", base.gameObject);
        }
    }
}
