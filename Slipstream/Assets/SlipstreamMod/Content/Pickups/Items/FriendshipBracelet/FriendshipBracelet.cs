using MSU;
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
using MSU.Config;
using RoR2.ContentManagement;
using System.Collections;

namespace Slipstream.Items
{
    public class FriendshipBracelet : SlipItem
    {
        private const string TOKEN = "SLIP_ITEM_FRIENDSHIPBRACELET_DESC";
        public override ItemDef ItemDef => _itemDef;

        public override NullableRef<GameObject> ItemDisplayPrefab => throw new NotImplementedException();

        private ItemDef _itemDef;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Items Granted per Stack", ConfigDescOverride = "Number of items allies will gain from each stack.", ConfigSectionOverride = "FriendshipBracelet")]
        public static int itemGrantPerStack = 1;
        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Affects Engineer Turrets", ConfigDescOverride = "Should Friendship Bracelet grant extra items to Engineer Turrets?", ConfigSectionOverride = "FriendshipBracelet")]
        public static bool affectsEngineerTurrets = false;
        public override void Initialize()
        {
            //hook makes sure allies that experience item transformations are still tracking the proper item indices - so they get the proper items removed
            On.RoR2.CharacterMasterNotificationQueue.SendTransformNotification_CharacterMaster_ItemIndex_ItemIndex_TransformationType += CharacterMasterNotificationQueue_SendTransformNotification_CharacterMaster_ItemIndex_ItemIndex_TransformationType;
        }
        public override bool IsAvailable(ContentPack contentPack)
        {
            return true;
        }

        public override IEnumerator LoadContentAsync()
        {
            var request = SlipAssets.LoadAssetAsync<ItemDef>("FriendshipBracelet", SlipBundle.Items);

            request.StartLoad();
            while (!request.IsComplete)
                yield return null;

            _itemDef = request.Asset;
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
                        }
                    }
                }
            }
        }

        //attached to all allies of players w/ the item, tracks the items already given to them individually
        public class FriendshipBraceletAllyServer : MonoBehaviour
        {
            public static float effectDelay = 0.75f;

            public List<ItemIndex> braceletItemAcquisitionOrder = new List<ItemIndex>();
            public CharacterMaster master;
            private Queue<ItemIndex> effectQueue = new Queue<ItemIndex>();
            private float effectTimer;
            public void GrantItem(ItemIndex itemIndex)
            {
                if (!NetworkServer.active)
                {
                    return;
                }
                if (master && master.inventory)
                {
                    braceletItemAcquisitionOrder.Add(itemIndex);
                    master.inventory.GiveItem(itemIndex);
                    effectQueue.Enqueue(itemIndex);
                }
            }
            private void Update()
            {
                if (!NetworkServer.active)
                {
                    return;
                }
                if (effectTimer <= 0)
                {
                    if (effectQueue.Count > 0)
                    {
                        CharacterBody characterBody = master.GetBody();
                        if (characterBody)
                        {
                            effectTimer += effectDelay;
                            EffectData effectData = new EffectData
                            {
                                origin = characterBody.corePosition,
                                genericFloat = 1.5f,
                                genericUInt = Util.IntToUintPlusOne((int)effectQueue.Dequeue())
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
                    return;

                }
                effectTimer -= Time.fixedDeltaTime;
            }
        }
        public class FriendshipBraceletBehaviourServer : BaseItemMasterBehavior
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
            //allies to be set up next frame
            private Queue<CharacterMaster> pendingAllies = new Queue<CharacterMaster>();
            private void Start()
            {
                ulong seed = Run.instance.seed ^ (ulong)(Run.instance.stageClearCount);
                this.itemGrantRng = new Xoroshiro128Plus(seed);
                
                //build available item indices the first time
                OnInventoryChanged();

                //queue pre-existing allies
                foreach (CharacterMaster characterMaster in CharacterMaster.instancesList)
                {
                    if (characterMaster.minionOwnership && characterMaster.minionOwnership.ownerMaster == master)
                    {
                        pendingAllies.Enqueue(characterMaster);
                    }
                }
            }
            private void FixedUpdate()
            {
                //set up pending allies
                if(pendingAllies.Count > 0)
                {
                    CharacterMaster characterMaster = pendingAllies.Dequeue();
                    if(characterMaster && AllyCanBeGrantedItemsFilter(characterMaster))
                    {
                        FriendshipBraceletAllyServer allyItemHandler = AddNewAlly(characterMaster);
                        TryUpdateAllyItemsServer(allyItemHandler);
                    }
                }
            }
            private void OnDestroy()
            {
                //clean up ally handlers
                foreach(FriendshipBraceletAllyServer allyItemHandler in allyItemHandlers)
                {
                    TryUpdateAllyItemsServer(allyItemHandler);
                    Destroy(allyItemHandler);
                }
                allyItemHandlers.Clear();
            }
            private void OnEnable()
            {
                master.inventory.onInventoryChanged += Inventory_onInventoryChanged;  
                MasterSummon.onServerMasterSummonGlobal += MasterSummon_onServerMasterSummonGlobal;
            }

 

            private void OnDisable()
            {
                master.inventory.onInventoryChanged -= Inventory_onInventoryChanged;
                MasterSummon.onServerMasterSummonGlobal -= MasterSummon_onServerMasterSummonGlobal;
            }

            private void Inventory_onInventoryChanged()
            {
                OnInventoryChanged();
            }
            public void OnInventoryChanged()
            {
                availableItemIndices = master.inventory.itemAcquisitionOrder.Where(ItemIndexFilter).ToArray(); 
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
                if (master == report.leaderMasterInstance)
                {
                    CharacterMaster summonMasterInstance = report.summonMasterInstance;
                    if (summonMasterInstance)
                    {
                        pendingAllies.Enqueue(summonMasterInstance);
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
                    ItemIndex[] shuffledAvailableItems = availableItemIndices.Where((ItemIndex index) => !ally.braceletItemAcquisitionOrder.Contains(index)).ToArray();

                    Util.ShuffleArray(shuffledAvailableItems, itemGrantRng);
                    for(int i = 0; i < Mathf.Min(currentItemGrantCount - count, shuffledAvailableItems.Length); i++)
                    {
                        ally.GrantItem(shuffledAvailableItems[i]);
                    }
                    return;
                }
                //to run if ally has too many itemss
                for(int i = count - 1; i >= currentItemGrantCount; i--)
                {
                    Inventory inventory = ally.master?.inventory;
                    ItemIndex itemIndex = ally.braceletItemAcquisitionOrder[i];
                    //SlipLog.Info("Attempting removal of " + ItemCatalog.GetItemDef(itemIndex).name);
                    if (inventory && inventory.GetItemCount(itemIndex) > 0)
                    {
                        inventory.RemoveItem(itemIndex);
                        //SlipLog.Info(string.Format("Removed item {0} from {1}", ItemCatalog.GetItemDef(itemIndex).name, ally.master));
                    }
                    ally.braceletItemAcquisitionOrder.RemoveAt(i);
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
                return itemDef && itemDef != SlipContent.Items.FriendshipBracelet && itemDef != DLC1Content.Items.VoidMegaCrabItem && itemDef.tier != ItemTier.NoTier && !itemDef.hidden && !blacklistedItemTags.Any((ItemTag tag) => itemDef.ContainsTag(tag));
            }
            private bool AllyCanBeGrantedItemsFilter(CharacterMaster characterMaster)
            {
                Deployable deployable = characterMaster.GetComponent<Deployable>();
                if(!affectsEngineerTurrets && deployable)
                {
                    for(int i = 0; i < master.deployablesList.Count; i++)
                    {
                        DeployableInfo deployableInfo = master.deployablesList[i];
                        if(deployableInfo.slot == DeployableSlot.EngiTurret && deployableInfo.deployable == deployable)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}
