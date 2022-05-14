using Moonstorm;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using RoR2.Projectile;

namespace Slipstream.Items
{
    public class JaceHat : ItemBase
    {
        private const string token = "SLIP_ITEM_JACEHAT_DESC";
        public override ItemDef ItemDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<ItemDef>("JaceHat");

        public static string section;

        [ConfigurableField(ConfigName = "Hat Percentage Damage", ConfigDesc = "Damage percent based off of base damage")]
        //[TokenModifier(token, StatTypes.Default, 0)]
        public static float hatDamage = 5f;

        [ConfigurableField(ConfigName = "Hat dmg per stack", ConfigDesc = "Damage percent increase per stack")]
        public static float dmgPerStack = 1f;

        [ConfigurableField(ConfigName = "Max Radius", ConfigDesc = "The farthest the hat will go before returning")]
        public static float maxRadius = 5f;

        //public static GameObject ProjectilePrefab { get; set; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("projJaceHat");

        public class JaceHatBehavior : BaseItemBodyBehavior
        {
            [ItemDefAssociation(useOnClient = true, useOnServer = true)]
            public static ItemDef GetItemDef() => SlipContent.Items.JaceHat;

            public static GameObject projectile;
            public void Start()
            {
                body.onSkillActivatedServer += OnSkillActivated;
            }

            public void OnDestroy()
            {
                body.onSkillActivatedServer -= OnSkillActivated;
            }
            public void OnSkillActivated(GenericSkill skill)
            {
                if (NetworkServer.active)
                {
                    if (body.skillLocator.FindSkill(skill.skillName))
                    {
                        if (body.skillLocator.secondary.Equals(skill))
                        {
                            //CreateProjectile();
                            Chat.AddMessage("<color=#e77118>secondary activation</color>");
                        }
                    }
                }
            }

            //finish this later

            /*public void CreateProjectile()
            {
                InputBankTest inputBank = body.inputBank;
                if(inputBank && body)
                {
                    Ray aimRay = new Ray(inputBank.aimOrigin, inputBank.aimDirection);
                    Quaternion rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
                    ProjectileManager.instance.FireProjectile(Projectiles.JaceHatProjectile.hatProj, aimRay.origin, rotation, body.gameObject, body.damage * 50, 0f, Util.CheckRoll(body.crit, body?.master), DamageColorIndex.Item, null);
                }

            }*/
        }
    }
}
