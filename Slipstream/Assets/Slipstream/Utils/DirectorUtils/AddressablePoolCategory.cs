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
using System.Linq;

namespace Slipstream.Scenes
{

	[Serializable]
	public class AddressablePoolCategory : ISerializationCallbackReceiver
	{
		public DccsPool.Category ToCategory()
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
				foreach (IAddressableKeyProvider<ExpansionDef> provider in includedIfConditionsMet)
				{
					SlipLogger.LogI("resolve expansiondef keys provider!");
					provider.Resolve();
				}
				foreach (IAddressableKeyProvider<DirectorCardCategorySelection> provider in includedIfNoConditionsMet)
				{
					provider.Resolve();
				}
				hasResolved = true;
			}
			return new DccsPool.Category
			{
				name = name,
				categoryWeight = categoryWeight,
				alwaysIncluded = alwaysIncluded.Cast<DccsPool.PoolEntry>().ToArray(),
				includedIfConditionsMet = includedIfConditionsMet.Cast<DccsPool.ConditionalPoolEntry>().ToArray(),
				includedIfNoConditionsMet = includedIfNoConditionsMet.Cast<DccsPool.PoolEntry>().ToArray(),
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
