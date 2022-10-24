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
    public interface IAddressableKeyProvider<T>
    {
        string Key { get; }
        T Addressable { set; }
        
    }
}
