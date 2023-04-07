using RoR2;
using UnityEngine;
using Moonstorm;

namespace Slipstream.Characters
{
    public sealed class BellFrog : MonsterBase
    {
        public override MSMonsterDirectorCard MonsterDirectorCard { get; } = SlipAssets.LoadAsset<MSMonsterDirectorCard>("msmdcBellFrog", SlipBundle.BasicMonsters);

        public override GameObject BodyPrefab { get; } = SlipAssets.LoadAsset<GameObject>("BellFrogBody", SlipBundle.BasicMonsters);

        public override GameObject MasterPrefab { get; } = SlipAssets.LoadAsset<GameObject>("BellFrogMaster", SlipBundle.BasicMonsters);
    }
}

