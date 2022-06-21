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
        public virtual ItemDef[] CorruptCatalog { get; } = Array.Empty<ItemDef>();
        private ItemRelationshipType cachedType = LegacyResourcesAPI.Load<ItemRelationshipType>("ItemRelationships/ContagiousItem");
        public override void Initialize()
        {
            base.Initialize();
            SlipLogger.LogI($"Initializing Slipstream void relationships...");
            SlipLogger.LogI(CorruptCatalog[0]);
            if (CorruptCatalog.Length != 0)
            {
                ItemRelationshipProvider itemRelationshipProvider = ScriptableObject.CreateInstance<ItemRelationshipProvider>();
                itemRelationshipProvider.relationshipType = cachedType;
                itemRelationshipProvider.relationships = new ItemDef.Pair[CorruptCatalog.Length];
                for(int i = 0; i < CorruptCatalog.Length; i++)
                {
                    itemRelationshipProvider.relationships[i] = new ItemDef.Pair
                    {
                        itemDef1 = this.CorruptCatalog[i],
                        itemDef2 = this.ItemDef
                    };
                }
                bool notNull = itemRelationshipProvider;
                SlipLogger.LogI(notNull);
                SlipContent.ItemRelationshipProviders.contagiousRelationships.Add(itemRelationshipProvider);
            }
        }
    }
}
