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

namespace Slipstream.Scenes
{

    [CreateAssetMenu(menuName = "Slipstream/SlipDccsPool")]
    public class SlipDccsPool : DccsPool
    {
        public void ResolveAddressableCategories()
        {
            poolCategories= new Category[addressablePoolCategories.Length];
            for (int i = 0; i < poolCategories.Length; i++)
            {
				poolCategories[i] = addressablePoolCategories[i].ToCategory();
            }
        }

		[SerializeField]
		private AddressableCategory[] addressablePoolCategories;

		[Serializable]
		public class AddressablePoolEntry : DccsPool.PoolEntry, IAddressableKeyProvider<DirectorCardCategorySelection>
		{
			[Tooltip("An optional addressable key to load a vanilla dccs")]
			public string dccsKey;
            public string Key => dccsKey;
			public DirectorCardCategorySelection Addressable { set => dccs = value; }
        }

		[Serializable]
		public class AddressableConditionalPoolEntry : DccsPool.ConditionalPoolEntry, IAddressableKeyProvider<DirectorCardCategorySelection>, IAddressableKeyArrayProvider<ExpansionDef>
		{
			[Tooltip("An optional addressable key to load a vanilla dccs")]
			public string dccsKey;
			[Tooltip("Optional addressable keys to load vanilla expansions as additional required expansions")]
			public string[] requiredExpansionsKeys;
			public string Key => dccsKey;
			public DirectorCardCategorySelection Addressable { set => dccs = value; }
            string[] IAddressableKeyArrayProvider<ExpansionDef>.Key => requiredExpansionsKeys;
            ExpansionDef IAddressableKeyProvider<ExpansionDef>.Addressable { set => HG.ArrayUtils.ArrayAppend(ref requiredExpansions, value); }
        }
		[Serializable]
		public class AddressableCategory : ISerializationCallbackReceiver
		{
			public Category ToCategory()
            {
				if (!hasResolved)
				{
					foreach (IAddressableKeyProvider<DirectorCardCategorySelection> provider in alwaysIncluded)
					{
						provider.Resolve();
					}
					foreach (IAddressableKeyProvider<DirectorCardCategorySelection> provider in includedIfConditionsMet)
					{
						provider.Resolve();
					}
					foreach (IAddressableKeyProvider<DirectorCardCategorySelection> provider in includedIfNoConditionsMet)
					{
						provider.Resolve();
					}
					hasResolved = true;
				}
				return new Category
				{
					name = name,
					categoryWeight = categoryWeight,
					alwaysIncluded = alwaysIncluded,
					includedIfConditionsMet = includedIfConditionsMet,
					includedIfNoConditionsMet = includedIfNoConditionsMet,
					hasBeenSerialized = hasBeenSerialized
				};
			}
			public void OnBeforeSerialize()
			{
				this.hasBeenSerialized = true;
			}

			public void OnAfterDeserialize()
			{
				if (!this.hasBeenSerialized)
				{
					this.categoryWeight = 1f;
				}
			}

			[Tooltip("A name to help identify this addressable category")]
			public string name;

			[Tooltip("The weight of all entries in this addressable category relative to the sibling categories.")]
			public float categoryWeight = 1f;

			[Tooltip("These addressable entries are always considered.")]
			public AddressablePoolEntry[] alwaysIncluded;

			[Tooltip("These addressable entries are only considered if their individual conditions are met.")]
			public AddressableConditionalPoolEntry[] includedIfConditionsMet;

			[Tooltip("These addressable entries are considered only if no entries from 'includedIfConditionsMet' have been included.")]
			public AddressablePoolEntry[] includedIfNoConditionsMet;

			[HideInInspector]
			[SerializeField]
			protected bool hasBeenSerialized;

			private bool hasResolved;
		}
	}

}
