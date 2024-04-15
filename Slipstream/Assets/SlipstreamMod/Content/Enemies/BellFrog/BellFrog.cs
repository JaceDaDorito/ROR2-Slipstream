using RoR2;
using UnityEngine;
using MSU;
using EntityStates;

namespace Slipstream.Characters
{
    public sealed class BellFrog : MonsterBase
    {
        public override GameObject BodyPrefab { get; } = SlipAssets.LoadAsset<GameObject>("BellFrogBody", SlipBundle.BasicMonsters);

        public override GameObject MasterPrefab { get; } = SlipAssets.LoadAsset<GameObject>("BellFrogMaster", SlipBundle.BasicMonsters);
    }

    internal class SpawnState : GenericCharacterSpawnState{}

    public class DeathState : GenericCharacterDeath
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }
    }
}
