using Moonstorm;
using UnityEngine.AddressableAssets;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Slipstream.Scenes
{
    [Serializable]
    public class AddressableDirectorCard : DirectorCard, IAddressableKeyProvider<SpawnCard>
    {
        
        [Tooltip("An optional addressable key to load a vanilla spawn card")]
        public string spawnCardKey;

        public string Key => spawnCardKey;

        public SpawnCard Addressable { set => spawnCard = value; }
    }

}
