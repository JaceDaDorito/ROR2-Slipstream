using R2API;
using RoR2;
using UnityEngine;
using static On.RoR2.HealthComponent;
using static R2API.RecalculateStatsAPI;

namespace Slipstream.Items
{
    public class GlassEye
    {
        public ItemDef itemDef { get; set; } = Assets.mainAssetBundle.LoadAsset<ItemDef>("GlassEye");

        public void Init()
        {
            SlipstreamPlugin.ModItemDefs.Add(itemDef);
            //GlassEyeItem();
            AddLanguageTokens();
            Hooks();
        }

        /*private void GlassEyeItem()
        {
            itemDef.name = "GlassEye";
            itemDef.tier = ItemTier.Tier2;
            itemDef.pickupModelPrefab = SlipstreamPlugin.AssetBundle.LoadAsset<GameObject>("Assets/Import/GlassEye/GlassEye.prefab");
            itemDef.pickupIconSprite = SlipstreamPlugin.AssetBundle.LoadAsset<Sprite>("Assets/Import/GlassEye/GlassEye_icon.png");
            itemDef.nameToken = "GLASSEYE_NAME";
            itemDef.pickupToken = "GLASSEYE_PICKUP";
            itemDef.descriptionToken = "GLASSEYE_DESC";
            itemDef.loreToken = "GLASSEYE_LORE";
            itemDef.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Utility };

            //TODO: MAKE THIS SHIT ACTUALLY WORK
            var itemDisplayRules = new ItemDisplayRule[1];
            itemDisplayRules[0].followerPrefab = SlipstreamPlugin.AssetBundle.LoadAsset<GameObject>("Assets/Import/snake/snake.prefab");
            itemDisplayRules[0].childName = "Chest";
            itemDisplayRules[0].localScale = new Vector3(2f, 2f, 2f);

            
        }*/

        //we dont need any of the item defs anymore BUT im keeping them here just in case

        private void AddLanguageTokens()
        {
            LanguageAPI.Add("GLASSEYE_NAME", "Glass Eye");
            LanguageAPI.Add("GLASSEYE_PICKUP", "Increased critical strike damage while blue shields are active. Grants blue shields and critical strike chance.");
            LanguageAPI.Add("GLASSEYE_DESC", "Gain a shield equal to <style=cIsHealing>5%</style> of your maximum health. Increases critical strike chance by <style=cIsDamage>5%</style>, and critical strikes do <style=cIsDamage>10%</style> <style=cStack>(10% per stack)</style> extra damage while shields are active.");
            LanguageAPI.Add("GLASSEYE_LORE", "Like most technological marvels before it, bionics – touted as the greatest advancement in medical science since discovery of Penicillin – quickly became a topic of public obsession, to the point of hysteria across both Earth & Mars. \nMany people desiring bionic “upgrades” – for one reason or another – went to extreme lengths in order to reach their goals. Some repeatedly broke bones in order to qualify for titanium implants throughout their bodies, others arranged accidents in order to gain a mechanical arm or leg. \nIn one case, the famous ‘William R Smith’ – known as an early advocate for personal shielding technologies – reportedly gouged out their right eye with a pair of gardening sheers with the intent of replacing the eye with a self-made, bionic glass eye. \nUniquely, this bionic eye used an integrated shield generator in place of the standard mechanical lens used at the time, using the glass structure of the eye as a medium to control the shield-based lens instead. \n-    A history of early bionics, Chapter 2 – Public Perception");
        }

        private void Hooks()
        {
            GetStatCoefficients += GrantBaseShield;
            On.RoR2.HealthComponent.TakeDamage += CritBoost;
        }

        public void CritBoost(orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            GameObject damageSource;
            if (damageInfo.attacker != null)
            {
                damageSource = damageInfo.attacker;
            }
            else
            {
                orig(self, damageInfo);
                return;
            }
            CharacterBody body = damageSource.GetComponent<CharacterBody>();
            if (body != null)
            {
                if (body.inventory.GetItemCount(itemDef) > 0)
                {
                    if (body.GetComponent<HealthComponent>().shield > 0 && damageInfo.crit)
                    {
                        damageInfo.damage *= (float)(1 + (0.2 * body.inventory.GetItemCount(itemDef)));
                    }
                }
            }
            orig(self, damageInfo);
        }

        private void GrantBaseShield(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender.inventory.GetItemCount(itemDef) > 0)
            {
                HealthComponent healthC = sender.GetComponent<HealthComponent>();
                args.baseShieldAdd += healthC.fullHealth * 0.05f;
                args.critAdd += 5;
            }
        }
    }
}