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

namespace Slipstream.Items
{
    public class ChargedFungus : SlipItem, IContentPackModifier
    {
        private const string TOKEN = "SLIP_ITEM_CHUNGUS_DESC";
        public override ItemDef ItemDef => _itemDef;

        private ItemDef _itemDef;

        private AssetCollection _assetCollection;
        public override NullableRef<GameObject> ItemDisplayPrefab => null;

        [ConfigureField(SlipConfig.ITEMS, ConfigNameOverride = "Healing Percentage", ConfigDescOverride = "Amount healed per second while equipment is charged", ConfigSectionOverride = "ChargedFungus")]
        [FormatToken(TOKEN, FormatTokenAttribute.OperationTypeEnum.MultiplyByN, 100)]
        public static float HealingPercentage = 0.03f;


        public void ModifyContentPack(ContentPack contentPack)
        {
            contentPack.AddContentFromAssetCollection(_assetCollection);
        }
        public override void Initialize()
        {
        }
        public override bool IsAvailable(ContentPack contentPack)
        {
            return true;
        }

        public override IEnumerator LoadContentAsync()
        {
            var request = SlipAssets.LoadAssetAsync<AssetCollection>("acChargedFungus", SlipBundle.Items);

            request.StartLoad();
            while (!request.IsComplete)
                yield return null;

            _assetCollection = request.Asset;

            _itemDef = _assetCollection.FindAsset<ItemDef>("ChargedFungus");
            yield break;

        }

        public class ChungusBehavior : BaseItemBodyBehavior
        {
            public DamageAPI.ModdedDamageType chungusStrike = DamageAPI.ReserveDamageType();
            public float healtimer;

            

            [ItemDefAssociation(useOnServer = true, useOnClient = true)]
            public static RoR2.ItemDef GetItemDef() => SlipContent.Items.ChargedFungus;

            public void FixedUpdate()
            {

                if (body)
                {

                    if(body.equipmentSlot)
                    {
                       
                        if (body.equipmentSlot.stock > 0 && !body.HasBuff(SlipContent.Buffs.ChungusBuff))
                        {
                            
                            OrbManager.instance.AddOrb(new SimpleLightningStrikeOrb
                            {
                                attacker = body.gameObject,
                                target = body.mainHurtBox,
                                damageValue = 0f,
                                isCrit = false,


                            });
                            body.AddBuff(SlipContent.Buffs.ChungusBuff);

                        }

                        if (body.equipmentSlot.stock == 0 && body.HasBuff(SlipContent.Buffs.ChungusBuff))
                        {
                            body.RemoveBuff(SlipContent.Buffs.ChungusBuff);
                        }
                    }

                }
            }     
        }
    }
    public class ChungusBuffBehavior : BuffBehaviour
    {
        [BuffDefAssociation]
        public static BuffDef GetBuffDef() => SlipContent.Buffs.ChungusBuff;
        public float healingtime = 1f;


        private void FixedUpdate()
        {
            healingtime -= Time.fixedDeltaTime;


            if (healingtime <= 0f)
            {
                healingtime = 1f;
                CharacterBody.healthComponent.HealFraction(ChargedFungus.HealingPercentage, default);


            }
        }
    }
}