using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonstorm;
using RoR2;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using RoR2.Items;
using Slipstream.Buffs;
using RoR2.Orbs;

namespace Slipstream.Items
{
    public class ChargedFungus : ItemBase
    {
        private const string token = "SLIP_ITEM_CHUNGUS_DESC";
        public override ItemDef ItemDef => SlipAssets.LoadAsset<ItemDef>("ChargedFungus", SlipBundle.Items);

       

        [ConfigurableField(ConfigName = "Healing Percentage", ConfigDesc = "Amount healed per second while equipment is charged", ConfigSection = "ChargedFungus")]
        [TokenModifier(token, StatTypes.MultiplyByN, 3, "100")]
        public static float HealingPercentage = 0.03f;

       

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



                            /*if (healtimer <= 0)
                            {
                                body.healthComponent.HealFraction(HealingPercentage, default(ProcChainMask));
                                healtimer = 1;
                            }*/
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
}