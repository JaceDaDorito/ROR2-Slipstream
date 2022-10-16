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
	[Serializable]
	public class AddressablePoolEntry : DccsPool.PoolEntry, IBasicAddressableKeyProvider<DirectorCardCategorySelection>
	{
		[Tooltip("An optional addressable key to load a vanilla dccs")]
		public string dccsKey;
		public string Key => dccsKey;
		public DirectorCardCategorySelection Addressable { set => dccs = value; }
	}
}