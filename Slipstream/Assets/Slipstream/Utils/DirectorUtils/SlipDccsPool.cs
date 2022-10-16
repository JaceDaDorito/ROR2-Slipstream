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
using RoR2.ExpansionManagement;
using UnityEngine.Serialization;
using System.Linq;

namespace Slipstream.Scenes
{

    [CreateAssetMenu(menuName = "Slipstream/SlipDccsPool")]
    public class SlipDccsPool : DccsPool
    {
        public void ResolveAddressableCategories()
        {
            poolCategories = new Category[addressablePoolCategories.Length];
            for (int i = 0; i < poolCategories.Length; i++)
            {
                poolCategories[i] = addressablePoolCategories[i].ToCategory();
            }
        }

        [SerializeField]
        private AddressablePoolCategory[] addressablePoolCategories;
    }
}
