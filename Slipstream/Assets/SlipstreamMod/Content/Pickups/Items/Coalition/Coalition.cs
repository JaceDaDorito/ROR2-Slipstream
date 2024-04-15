using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSU;
using RoR2;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using RoR2.Items;
using Slipstream.Buffs;
using RoR2.Orbs;
using RoR2.ContentManagement;
using MSU.Config;
using UnityEngine.UI;


namespace Slipstream.Items
{
    public class Coalition : SlipItem, IContentPackModifier
    {
        private const string TOKEN = "SLIP_ITEM_COALITION_DESC";
        public override ItemDef ItemDef => _itemDef;

        private ItemDef _itemDef;
        public ItemDef BlackHealthItemDef => _blackHealthItemDef;

        private ItemDef _blackHealthItemDef;

        public override NullableRef<GameObject> ItemDisplayPrefab => _itemDisplay;

        private GameObject _itemDisplay;

        private static GameObject effectStartPrefab;

        public static Color blackOverHealthColor = SlipUtils.ColorRGB(11f, 11f, 9f);
        public static Color blackHealingHealthColor = SlipUtils.ColorRGB(159f, 160, 159f);

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Allies are immunte to void death", ConfigDescOverride = "Prevents instant void explosion deaths (like the Void Reaver explosion) for allies.", ConfigSectionOverride = "Coalition")]
        //[FormatToken(TOKEN, StatTypes.Default, 0)]
        public static bool preventsVoidDeath = true;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Initial health threshold/Hyperbolic Scaling", ConfigDescOverride = "Initial health threshold percentage at one stack and hyperbolic staling", ConfigSectionOverride = "Coalition")]
        [FormatToken(TOKEN, FormatTokenAttribute.OperationTypeEnum.MultiplyByN, 100)]
        public static float amplificationPercentage = 0.25f;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Armor increase per stack", ConfigDescOverride = "Armor increase applied to allies per stack", ConfigSectionOverride = "Coalition")]
        public static float armorIncrease = 100f;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Max health threshold", ConfigDescOverride = "Hypothetical max health threshold percentage possible", ConfigSectionOverride = "Coalition")]
        [FormatToken(TOKEN, FormatTokenAttribute.OperationTypeEnum.MultiplyByN, 100)]
        public static float maxPercentage = 1f;


        public static RoR2.UI.HealthBarStyle.BarStyle coalitionBarStyle;
        public static DamageAPI.ModdedDamageType BlackBlood;

        public override void Initialize()
        {
            On.RoR2.UI.HealthBar.UpdateBarInfos += new On.RoR2.UI.HealthBar.hook_UpdateBarInfos(HealthBar_UpdateBarInfos);

            ExtraHealthbarSegment.AddType<CoalitionBarData>();
        }

        public override bool IsAvailable(ContentPack contentPack)
        {
            return true;
        }

        public override IEnumerator LoadContentAsync()
        {
            var request = SlipAssets.LoadAssetAsync<AssetCollection>("acCoalition", SlipBundle.Items);

            request.StartLoad();
            while (!request.IsComplete)
                yield return null;

            var collection = request.Asset;

            _itemDef = collection.FindAsset<ItemDef>("Coalition");
            _blackHealthItemDef = collection.FindAsset<ItemDef>("BlackHealth");
            _itemDisplay = collection.FindAsset<GameObject>("DisplayCoalition");
            effectStartPrefab = collection.FindAsset<GameObject>("CoalitionPreDetonation");
        }
        public void ModifyContentPack(ContentPack contentPack)
        {
            contentPack.itemDefs.AddSingle(_blackHealthItemDef);
        }
        private void HealthBar_UpdateBarInfos(On.RoR2.UI.HealthBar.orig_UpdateBarInfos orig, RoR2.UI.HealthBar self)
        {
            orig(self);
            CharacterBody characterBody = self.source?.body;
            if (characterBody)
            {
                Inventory inventory = characterBody.inventory;
                if (inventory && inventory.GetItemCount(BlackHealthItemDef) > 0)
                {
                    //going to change the visuals of this soon
                    self.barInfoCollection.trailingOverHealthbarInfo.color = blackOverHealthColor;
                    self.barInfoCollection.instantHealthbarInfo.color = blackHealingHealthColor;
                }
            }
        }

        public class CoalitionIntervalsBetweenDeaths : MonoBehaviour
        {
            public List<CharacterBody> deathList = new List<CharacterBody>();
            public List<GameObject> effectList = new List<GameObject>();
            
            private Run.FixedTimeStamp timer;
            private static float totalPhaseTime = 2f;
            private static float pauseTime = 0.5f;
            private float intervalTime;
            private int index = 0;
            private int deathIndex = 0;
            private void Start()
            {
                
                timer = Run.FixedTimeStamp.now;
                intervalTime = totalPhaseTime / deathList.Count;
                SlipLog.Debug("Timer on coalition started" + intervalTime);

            }

            //takes totalPhaseTime + pauseTime amount of time for each explosion to go off

            private void FixedUpdate()
            {
                if(timer.timeSince > 0)
                {
                    if (timer.timeSince <= totalPhaseTime)
                    {
                        if (timer.timeSince >= index * intervalTime)
                        {
#if DEBUG
                            SlipLog.Debug($"Spawned Effect at index " + index);
#endif
                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(effectStartPrefab, deathList[index].coreTransform);
                            gameObject.transform.localPosition = Vector3.zero;
                            gameObject.transform.localRotation = Quaternion.identity;
                            gameObject.transform.localScale = gameObject.transform.localScale * deathList[index].bestFitRadius;

                            effectList.Add(gameObject);

                            index++;
                        }
                    }
                    else if (timer.timeSince <= totalPhaseTime + pauseTime)
                    {
                        return;
                    }
                    else if (timer.timeSince <= totalPhaseTime * 2f + pauseTime)
                    {
                        if (timer.timeSince >= deathIndex * intervalTime + totalPhaseTime + pauseTime)
                        {
#if DEBUG
                            SlipLog.Debug($"Start destruction " + deathIndex);
#endif
                            Destroy(effectList[0]);
                            effectList.RemoveAt(0);
#if DEBUG
                            SlipLog.Debug($"Destroyed Effect at index " + deathIndex);
#endif
                            deathList[0].healthComponent.Suicide();
                            deathList.RemoveAt(0);
                            deathIndex++;
#if DEBUG
                            SlipLog.Debug($"Finished " + deathIndex);
#endif
                        }
                    }
                    else if (timer.timeSince >= totalPhaseTime * 2f + pauseTime)
                    {
                        SlipLog.Debug("deathcomponent destroyed");
                        Destroy(this);
                        
                    }
                }
            }

        }
        public class CoalitionBehavior : BaseItemBodyBehavior
        {
            [ItemDefAssociation(useOnClient = false, useOnServer = true)]
            public static ItemDef GetItemDef() => SlipContent.Items.Coalition;
            private MinionOwnership.MinionGroup minionGroup;
            private int previousStack;

            private void OnEnable()
            {
                MasterSummon.onServerMasterSummonGlobal += MasterSummon_onServerMasterSummonGlobal;
                On.RoR2.HealthComponent.UpdateLastHitTime += HealthComponent_UpdateLastHitTime;

                UpdateAllMinions(stack);
            }

            private void OnDisable()
            {
                MasterSummon.onServerMasterSummonGlobal -= MasterSummon_onServerMasterSummonGlobal;
                On.RoR2.HealthComponent.UpdateLastHitTime -= HealthComponent_UpdateLastHitTime;

                UpdateAllMinions(0);
            }

            private void FixedUpdate()
            {
                if (previousStack != stack)
                    UpdateAllMinions(stack);
            }
            private void MasterSummon_onServerMasterSummonGlobal(MasterSummon.MasterSummonReport report)
            {
                //If allies are gained while having Coalition, give them Blackhealth
                if (body.master && body.master == report.leaderMasterInstance)
                {
                    CharacterMaster summonMasterInstance = report.summonMasterInstance;
                    if (summonMasterInstance)
                    {
                        if (summonMasterInstance.GetBody())
                        {
                            UpdateMinionGroup(body);
                            UpdateMinionInventory(summonMasterInstance.inventory, stack);
                        }
                    }
                }
            }

            private void UpdateMinionGroup(CharacterBody charBody)
            {
                minionGroup = MinionOwnership.MinionGroup.FindGroup(body.master.netId);
            }

            private void UpdateAllMinions(int newStack)
            {
                if((body != null) ? body.master : null)
                {
                    UpdateMinionGroup(body);
                    if(minionGroup != null)
                    {
                        foreach(MinionOwnership minionOwnership in minionGroup.members)
                        {
                            if (minionOwnership)
                            {
                                CharacterMaster minionMaster = minionOwnership.GetComponent<CharacterMaster>();
                                CharacterBody minionBody = minionMaster.GetBody();
                                if (minionMaster && minionMaster.inventory && minionBody)
                                {
                                    UpdateMinionInventory(minionMaster.inventory, newStack);
                                }
                            }
                        }
                        previousStack = newStack;
                    }
                }
            }

            private void UpdateMinionInventory(Inventory inventory, int newStack)
            {
                if(inventory && newStack > 0)
                {
                    int itemCount = inventory.GetItemCount(SlipContent.Items.BlackHealth);
                    if (itemCount < stack)
                        inventory.GiveItem(SlipContent.Items.BlackHealth, stack - itemCount);
                    else if (itemCount > stack)
                        inventory.RemoveItem(SlipContent.Items.BlackHealth, itemCount - stack);
                }
                else
                {
                    inventory.ResetItem(SlipContent.Items.BlackHealth);
                }
            }

            


            private void HealthComponent_UpdateLastHitTime(On.RoR2.HealthComponent.orig_UpdateLastHitTime orig, HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker)
            {
                //call orig to make sure that Elixr is activated first. This only matters at 1 stack though.
                orig(self, damageValue, damagePosition, damageIsSilent, attacker);

                //Kill all allies when player is below the current threshold.
                float threshold = MSU.MSUtil.InverseHyperbolicScaling(amplificationPercentage, amplificationPercentage, maxPercentage, stack);
                if (body == self.body && (body.healthComponent.health + body.healthComponent.shield)/body.healthComponent.fullCombinedHealth < threshold && body.gameObject.GetComponent<CoalitionIntervalsBetweenDeaths>() == null) //Character body check and ratio of curent health compared to the threshold
                {
                    CharacterMaster master = body.master;
                    CoalitionIntervalsBetweenDeaths deathComponent = body.gameObject.AddComponent<CoalitionIntervalsBetweenDeaths>();
                    foreach (MinionOwnership minionOwnership in minionGroup.members)
                    {
                        if (minionOwnership)
                        {
                            CharacterBody minionBody = minionOwnership.GetComponent<CharacterMaster>().GetBody();
                            deathComponent.deathList.Add(minionBody);
                        }
                    }
                }
            }
        }

        public class BlackHealthBehavior : BaseItemBodyBehavior, IBodyStatArgModifier, IOnKilledServerReceiver
        {
            [ItemDefAssociation(useOnClient = true, useOnServer = true)]

            public static RoR2.ItemDef GetItemDef() => SlipContent.Items.BlackHealth;

            private bool changedFlag;

            public void OnEnable()
            {
                //Makes owners of Blackhealth completely immune to void explosions (when its enabled)
                changedFlag = false;
                if ((body.bodyFlags & CharacterBody.BodyFlags.ImmuneToVoidDeath) == CharacterBody.BodyFlags.None && preventsVoidDeath)
                {
                    changedFlag = true;
                    body.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath;
                }

            }

            public void OnDisable()
            {
                //Removes tag if the immunity was given (that way allies somehow initially having the ImmuneToVoidDeath flag don't get screwed over)
                if (changedFlag)
                    body.bodyFlags &= ~CharacterBody.BodyFlags.ImmuneToVoidDeath;
            }

            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                args.armorAdd += stack * armorIncrease;
            }

            public void OnKilledServer(DamageReport damageReport)
            {
                Util.PlaySound("Play_nullifier_death_vortex_explode", base.gameObject);

                Transform transform = body.modelLocator.modelTransform;
                if (transform)
                {
                    Destroy(transform.gameObject);
                    transform = null;
                }
            }
        }

        public class CoalitionBarData : ExtraHealthbarSegment.BarData
        {
            private bool enabled;
            private Material barMat = SlipAssets.LoadAsset<Material>("matCoalitionBloodBar", SlipBundle.Items);

            private float threshold;
            private static float width = 0.05f;
            //private static float height = 0.2f;

            public static Dictionary<HealthComponent, Material> materialBlood = new Dictionary<HealthComponent, Material>();

            public override RoR2.UI.HealthBarStyle.BarStyle GetStyle()
            {
                var style = coalitionBarStyle;
                style.sizeDelta = bar.style.barrierBarStyle.sizeDelta;
                style.baseColor = ExtraHealthbarSegment.defaultBaseColor;
                style.imageType = Image.Type.Simple;
                style.enabled = true;

                return style;
            }

            public override void CheckInventory(ref RoR2.UI.HealthBar.BarInfo info, RoR2.CharacterBody body)
            {
                base.CheckInventory(ref info, body);

                //if there are any critical shield items in the inventory, enable healthbar

                //enabled = body.gameObject.GetComponent<ICriticalShield>() != null;

                enabled = false;

                int count = body.GetItemCount(SlipContent.Items.Coalition);
                if (count > 0)
                {
                    enabled = true;
                    threshold = MSU.MSUtil.InverseHyperbolicScaling(amplificationPercentage, amplificationPercentage, 1, count);
                }
                else
                {
                    if (materialBlood.ContainsKey(body.healthComponent))
                        materialBlood.Remove(body.healthComponent);
                    return;
                }
                enabled = true;
            }

            public override void UpdateInfo(ref RoR2.UI.HealthBar.BarInfo info, HealthComponent healthSource)
            {
                //instanceMat = image.canvasRenderer.GetMaterial();

                base.UpdateInfo(ref info, healthSource);

                info.enabled = enabled; //&& healthSource.shield > 0;

                info.normalizedXMin = threshold - (width / 2f);
                info.normalizedXMax = threshold + (width / 2f);
            }

            public override void ApplyBar(ref RoR2.UI.HealthBar.BarInfo info, Image image, HealthComponent source, ref int i)
            {

                //image.material = barMat;
                if (!materialBlood.ContainsKey(source))
                    materialBlood.Add(source, UnityEngine.Object.Instantiate(barMat));

                image.material = materialBlood[source];
                base.ApplyBar(ref info, image, source, ref i);
            }


        }
    }
}