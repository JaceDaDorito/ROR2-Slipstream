using System;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using Moonstorm;

namespace Slipstream.Items
{
    public abstract class VoidItemBase : ItemBase
    {
        //credit to groove as this is how he loads item relationships
        //this is temporary but this will do for now
        public virtual ItemDef[] ItemsToCorrupt { get; } = Array.Empty<ItemDef>();
        private ItemRelationshipType cachedType = LegacyResourcesAPI.Load<ItemRelationshipType>("ItemRelationships/ContagiousItem");
        public override void Initialize()
        {
            base.Initialize();
            SlipLogger.LogI($"Initializing Slipstream void relationships...");
            SlipLogger.LogI(ItemsToCorrupt[0]);
            if (ItemsToCorrupt.Length != 0)
            {
                ItemRelationshipProvider itemRelationshipProvider = ScriptableObject.CreateInstance<ItemRelationshipProvider>();
                itemRelationshipProvider.relationshipType = cachedType;
                itemRelationshipProvider.relationships = new ItemDef.Pair[ItemsToCorrupt.Length];
                for (int i = 0; i < ItemsToCorrupt.Length; i++)
                {
                    itemRelationshipProvider.relationships[i] = new ItemDef.Pair
                    {
                        itemDef1 = ItemsToCorrupt[i],
                        itemDef2 = ItemDef
                    };
                }
                bool notNull = itemRelationshipProvider;
                SlipLogger.LogI(notNull);
                SlipContent.ItemRelationshipProviders.contagiousRelationships.Add(itemRelationshipProvider);
            }
        }
    }
}
