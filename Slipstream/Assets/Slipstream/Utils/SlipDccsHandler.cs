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
    public class SlipDccsHandler
    {
        public void Init()
        {
            SlipDccs[] allSlipDccs = SlipAssets.LoadAllAssetsOfType<SlipDccs>();
            foreach(SlipDccs slipDccs in allSlipDccs)
            {
                slipDccs.ResolveAddressableCategories();
            }
        }
        [CreateAssetMenu(menuName = "SlipStream/SlipDccs")]
        public class SlipDccs : DirectorCardCategorySelection
        {   
            public void ResolveAddressableCategories()
            {
                Array.Resize(ref categories, addressableCategories.Length);
                for(int i = 0; i < addressableCategories.Length; i++)
                {
                    categories[i] = addressableCategories[i].ToCategory();
                }
            }
            [Tooltip("Categories populated by addressable keys or spawn cards. Addressable keys are added as spawn cards at runtime.")]
            public AddressableCategory[] addressableCategories = Array.Empty<AddressableCategory>();
        }
        [Serializable]
        public struct AddressableCategory
        {
            public DirectorCardCategorySelection.Category ToCategory()
            {
                foreach(AddressableDirectorCard addressableDirectorCard in addressableCards)
                {
                    if (!string.IsNullOrEmpty(addressableDirectorCard.spawnCardKey))
                    {
                        addressableDirectorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(addressableDirectorCard.spawnCardKey).WaitForCompletion();
                        if (!addressableDirectorCard.spawnCard)
                        {
                            SlipLogger.LogW(addressableDirectorCard + ": Addressable key [" + addressableDirectorCard.spawnCardKey + "] was provided, but is null!");
                        }
                    }
                }
                return new DirectorCardCategorySelection.Category
                {
                    name = name,
                    cards = addressableCards,
                    selectionWeight = selectionWeight
                };
            }
            [Tooltip("A name to help identify this addressable category")]
            public string name;

            public AddressableDirectorCard[] addressableCards;

            public float selectionWeight;
        }
        [Serializable]
        public class AddressableDirectorCard : DirectorCard
        {
            [Tooltip("An optional addressable key to load a vanilla spawn card")]
            public string spawnCardKey;
        }
    }
}
