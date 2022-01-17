using Moonstorm;
using Slipstream.Buffs;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using System;

namespace Slipstream.Items
{
    [DisabledContent]
    public class JaceHat : ItemBase
    {
        private const string token = "SLIP_ITEM_JACEHAT_DESC";
        public override ItemDef ItemDef { get; set; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<ItemDef>("JaceHat");

        public static string section;

        [ConfigurableField(ConfigName = "Hat Percentage Damage", ConfigDesc = "Damage percent based off of base damage")]
        //[TokenModifier(token, StatTypes.Default, 0)]
        public static float hatDamage = 5f;

        [ConfigurableField(ConfigName = "Hat dmg per stack", ConfigDesc = "Damage percent increase per stack")]
        public static float dmgPerStack = 1f;

        [ConfigurableField(ConfigName = "Max Radius", ConfigDesc = "The farthest the hat will go before returning")]
        public static float maxRadius = 5f;

        public static GameObject ProjectilePrefab { get; set; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("projJaceHat");

        public override void AddBehavior(ref CharacterBody body, int stack)
        {
            //SlipLogger.LogD($"Initializing Jace Hat");
            body.AddItemBehavior<JaceHatBehavior>(stack);
        }

        public class JaceHatBehavior : CharacterBody.ItemBehavior
        {
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
                            CreateProjectile();
                            Chat.AddMessage("<color=#e77118>secondary activation</color>");
                        }
                    }
                }
            }
            public void CreateProjectile()
            {
                InputBankTest inputBank = body.inputBank;
                if(inputBank && body)
                {
                    Ray aimRay = new Ray(inputBank.aimOrigin, inputBank.aimDirection);
                    Quaternion rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
                    ProjectileManager.instance.FireProjectile(Projectiles.JaceHatProjectile.hatProj, aimRay.origin, rotation, body.gameObject, body.damage * 50, 0f, Util.CheckRoll(body.crit, body?.master), DamageColorIndex.Item, null);
                }

                /*
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    projectilePrefab = sawProj,
                    crit = body.RollCrit(),
                    damage = body.damage,
                    damageColorIndex = DamageColorIndex.Item,
                    force = 10f,
                    owner = gameObject,
                    position = body.corePosition,
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);*/
            }
        }
    }
}
