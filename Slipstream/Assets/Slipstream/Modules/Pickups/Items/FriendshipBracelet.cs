using Moonstorm;
using UnityEngine.AddressableAssets;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using On.RoR2.UI;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using RoR2.Items;
using System.Linq;

namespace Slipstream.Items
{
    public class FriendshipBracelet : ItemBase
    {
        private const string token = "SLIP_ITEM_FRIENDSHIPBRACELET_DESC";
        public override ItemDef ItemDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<ItemDef>("FriendshipBracelet");
        [ConfigurableField(ConfigName = "Items Granted per Stack", ConfigDesc = "Number of items allies will gain from each stack.", ConfigSection = "FriendshipBracelet")]
        [TokenModifier(token, StatTypes.Default, 0)]
        public static int itemGrantPerStack = 2;
        public override void Initialize()
        {
            base.Initialize();
            //hook makes sure allies that experience item transformations are still tracking the proper item indices - so they get the proper items removed
            On.RoR2.CharacterMasterNotificationQueue.SendTransformNotification_CharacterMaster_ItemIndex_ItemIndex_TransformationType += CharacterMasterNotificationQueue_SendTransformNotification_CharacterMaster_ItemIndex_ItemIndex_TransformationType;
        }

        private void CharacterMasterNotificationQueue_SendTransformNotification_CharacterMaster_ItemIndex_ItemIndex_TransformationType(On.RoR2.CharacterMasterNotificationQueue.orig_SendTransformNotification_CharacterMaster_ItemIndex_ItemIndex_TransformationType orig, CharacterMaster characterMaster, ItemIndex oldIndex, ItemIndex newIndex, CharacterMasterNotificationQueue.TransformationType transformationType)
        {
            orig(characterMaster, oldIndex, newIndex, transformationType);
            if (NetworkServer.active && characterMaster)
            {
                FriendshipBraceletAllyServer allyItemHandler = characterMaster.GetComponent<FriendshipBraceletAllyServer>();
                if (allyItemHandler)
                {
                    for (int i = 0; i < allyItemHandler.braceletItemAcquisitionOrder.Count; i++)
                    {
                        if (allyItemHandler.braceletItemAcquisitionOrder[i] == oldIndex)
                        {
                            allyItemHandler.braceletItemAcquisitionOrder[i] = newIndex;
                            //SlipLogger.LogI(string.Format("Changed {0} to {1} for {2}", ItemCatalog.GetItemDef(oldIndex).name, ItemCatalog.GetItemDef(allyItemHandler.braceletItemAcquisitionOrder[i]).name, characterMaster));
                        }
                    }
                }
            }
        }
        //attached to all allies of players w/ the item, tracks the items already given to them individually
        public class FriendshipBraceletAllyServer : MonoBehaviour
        {
            public List<ItemIndex> braceletItemAcquisitionOrder = new List<ItemIndex>();
            public CharacterMaster master;
        }
        public class FriendshipBraceletBehaviourServer : BaseItemBodyBehavior
        {
            [ItemDefAssociation(useOnClient = false, useOnServer = true)]

            public static ItemDef GetItemDef() => SlipContent.Items.FriendshipBracelet;

            public static ItemTag[] blacklistedItemTags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotCopy,
                ItemTag.EquipmentRelated,
                ItemTag.HoldoutZoneRelated,
                ItemTag.InteractableRelated,
                ItemTag.ObliterationRelated,
                ItemTag.SprintRelated,
                ItemTag.Scrap,
                ItemTag.PriorityScrap,
            };
            public int currentItemGrantCount
            {
                get
                {
                    return stack * itemGrantPerStack;
                }
            }
            //rng isnt really necessary but egocentrism does it soooo
            private Xoroshiro128Plus itemGrantRng;
            //updated every time the body's inventory is updated, filters out all the items 
            public ItemIndex[] availableItemIndices;
            //ally handlers specific to this body
            public List<FriendshipBraceletAllyServer> allyItemHandlers = new List<FriendshipBraceletAllyServer>();
            private void Start()
            {
                ulong seed = Run.instance.seed ^ (ulong)(Run.instance.stageClearCount);
                this.itemGrantRng = new Xoroshiro128Plus(seed);
                //give pre-existing allies item handlers
                CharacterMaster master = body.master;
                if (master)
                {
                    foreach (CharacterMaster characterMaster in CharacterMaster.instancesList)
                    {
                        if (characterMaster.minionOwnership && characterMaster.minionOwnership.ownerMaster == master)
                        {
                            AddNewAlly(characterMaster);
                        }
                    }
                }
                //force one inventory change
                OnInventoryChanged();
            }
            private void OnDestroy()
            {
                //clean up ally handlers
                stack = 0;
                foreach(FriendshipBraceletAllyServer allyItemHandler in allyItemHandlers)
                {
                    TryUpdateAllyItemsServer(allyItemHandler);
                    Destroy(allyItemHandler);
                }
                allyItemHandlers.Clear();
            }
            private void OnEnable()
            {
                body.onInventoryChanged += Body_onInventoryChanged;
                MasterSummon.onServerMasterSummonGlobal += MasterSummon_onServerMasterSummonGlobal;
            }
            private void OnDisable()
            {
                body.onInventoryChanged -= Body_onInventoryChanged;
                MasterSummon.onServerMasterSummonGlobal -= MasterSummon_onServerMasterSummonGlobal;
            }
            private void Body_onInventoryChanged()
            {
                //have to force update stack because of bad ordering
                stack = body.inventory.GetItemCount(SlipContent.Items.FriendshipBracelet);
                OnInventoryChanged();
            }
            public void OnInventoryChanged()
            {
                availableItemIndices = body.inventory.itemAcquisitionOrder.Where(ItemIndexFilter).ToArray(); /*(from itemIndex in body.inventory.itemAcquisitionOrder
                                        where ItemIndexFilter(itemIndex)
                                        select itemIndex).ToArray();*/
                if(allyItemHandlers.Count > 0)
                {
                    for(int i = allyItemHandlers.Count - 1; i >= 0; i--)
                    {
                        FriendshipBraceletAllyServer allyItemHandler = allyItemHandlers[i];
                        if (!allyItemHandler)
                        {
                            allyItemHandlers.RemoveAt(i);
                        }
                        else
                        {
                            TryUpdateAllyItemsServer(allyItemHandlers[i]);
                        }
                    }
                }

            }
            //set up newly added allies
            private void MasterSummon_onServerMasterSummonGlobal(MasterSummon.MasterSummonReport report)
            {
                if (body.master && body.master == report.leaderMasterInstance)
                {
                    CharacterMaster summonMasterInstance = report.summonMasterInstance;
                    if (summonMasterInstance)
                    {
                        FriendshipBraceletAllyServer allyItemHandler = AddNewAlly(summonMasterInstance);
                        TryUpdateAllyItemsServer(allyItemHandler);
                    }
                }
            }
            //ensure allies have as many items as possible, and no extra items
            public void TryUpdateAllyItemsServer(FriendshipBraceletAllyServer ally)
            {
                int count = ally.braceletItemAcquisitionOrder.Count;
                if (!NetworkServer.active || count == currentItemGrantCount)
                {
                    return;
                }
                //to run if ally doesnt have enough items
                if(count < currentItemGrantCount)
                {
                    ItemIndex[] shuffledAvailableItems = availableItemIndices.Where((ItemIndex index) => !ally.braceletItemAcquisitionOrder.Contains(index)).ToArray();/* (from itemIndex in availableItemIndices
                                                          where !ally.braceletItemAcquisitionOrder.Contains(itemIndex)
                                                          select itemIndex
                                                          ).ToArray();*/
                    Util.ShuffleArray(shuffledAvailableItems, itemGrantRng);
                    for(int i = 0; i < Mathf.Min(currentItemGrantCount - count, shuffledAvailableItems.Length); i++)
                    {
                        GrantFriendshipBraceletItemServer(ally, shuffledAvailableItems[i]);
                    }
                    return;
                }
                //to run if ally has too many itemss
                for(int i = count - 1; i >= currentItemGrantCount; i--)
                {
                    Inventory inventory = ally.master?.inventory;
                    ItemIndex itemIndex = ally.braceletItemAcquisitionOrder[i];
                    //SlipLogger.LogI("Attempting removal of " + ItemCatalog.GetItemDef(itemIndex).name);
                    if (inventory && inventory.GetItemCount(itemIndex) > 0)
                    {
                        inventory.RemoveItem(itemIndex);
                        //SlipLogger.LogI(string.Format("Removed item {0} from {1}", ItemCatalog.GetItemDef(itemIndex).name, ally.master));
                    }
                    ally.braceletItemAcquisitionOrder.RemoveAt(i);
                }
            }
            //pretty unnecessary and will probably get refactored in some form when proper effects are added
            private void GrantFriendshipBraceletItemServer(FriendshipBraceletAllyServer ally, ItemIndex itemIndex)
            {
                if (NetworkServer.active && ally.master && ally.master.inventory)
                {
                    ally.braceletItemAcquisitionOrder.Add(itemIndex);
                    ally.master.inventory.GiveItem(itemIndex);
                    CharacterBody characterBody = ally.master.GetBody();
                    if (characterBody)
                    {
                        EffectData effectData = new EffectData
                        {
                            origin = body.corePosition,
                            genericFloat = 1f,
                            genericUInt = Util.IntToUintPlusOne((int)itemIndex)
                        };
                        if (characterBody.mainHurtBox)
                        {
                            effectData.SetHurtBoxReference(characterBody.mainHurtBox);
                        }
                        else
                        {
                            effectData.SetNetworkedObjectReference(characterBody.gameObject);
                        }
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OrbEffects/ItemTransferOrbEffect"), effectData, true);
                    }
                }
            }
            //happens in 2 different places so wanted to keep it consistent:
            public FriendshipBraceletAllyServer AddNewAlly(CharacterMaster characterMaster)
            {
                FriendshipBraceletAllyServer friendshipBraceletAllyServer = characterMaster.gameObject.AddComponent<FriendshipBraceletAllyServer>();
                friendshipBraceletAllyServer.master = characterMaster;
                allyItemHandlers.Add(friendshipBraceletAllyServer);
                return friendshipBraceletAllyServer;
            }
            private bool ItemIndexFilter(ItemIndex itemIndex)
            {
                RoR2.ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                return itemDef && itemDef != SlipContent.Items.FriendshipBracelet && itemDef.tier != ItemTier.NoTier && !itemDef.hidden && !blacklistedItemTags.Any((ItemTag tag) => itemDef.ContainsTag(tag));
            }
        }
    }
}
