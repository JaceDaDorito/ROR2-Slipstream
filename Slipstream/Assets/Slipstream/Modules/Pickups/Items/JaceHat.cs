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
    /*[DisabledContent]
    
    public class JaceHat : ItemBase
    {
        private const string token = "SLIP_ITEM_JACEHAT_DESC";
        public override ItemDef ItemDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<ItemDef>("JaceHat");

        public static string section;

        //[ConfigurableField(ConfigName = "Hat Percentage Damage", ConfigDesc = "Damage percent based off of base damage")]
        public static float hatDamageMult = 5f;

        //[ConfigurableField(ConfigName = "Hat dmg per stack", ConfigDesc = "Damage percent increase per stack")]
        public static float dmgPerStack = 1f;

        //[ConfigurableField(ConfigName = "Max Radius", ConfigDesc = "The farthest the hat will go before returning")]

        public static float initialRadius = 0.25f; 
        public static float maxRadius = 5f;

        private float projectileTimer;



        public class JaceHatBehavior : BaseItemBodyBehavior
        {
            [ItemDefAssociation(useOnClient = true, useOnServer = true)]
            public static ItemDef GetItemDef() => SlipContent.Items.JaceHat;
            private InputBankTest inputBank;

            //public static GameObject projectile;
            //public void InitializeOrbiter(ProjectileOwnerOrbiter orbiter, )
            public void Start()
            {
                body.onSkillActivatedServer += OnSkillActivated;
                inputBank = body.GetComponent<InputBankTest>();
            }

            public void OnDestroy()
            {
                body.onSkillActivatedServer -= OnSkillActivated;
            }
            public void OnSkillActivated(GenericSkill skill)
            {
                if (NetworkServer.active)
                {
                    if (body.skillLocator.FindSkill(skill.skillName) && body.skillLocator.secondary.Equals(skill))
                    {
                        FireHat();
                        Chat.AddMessage("<color=#e77118>secondary activation</color>");
                    }
                }
            }

            //im going insane over this projectile
            private void FireHat()
            {
                Vector3 aimRay = GetAimRay().direction;
                Vector3 vector = Vector3.Cross(aimRay, Vector3.up);
                Vector3 vector2 = Vector3.Cross(vector, Vector3.up);

                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    projectilePrefab = Projectiles.JaceHatProjectile.hatProj,
                    crit = body.RollCrit(),
                    damage = body.damage * hatDamageMult,
                    damageColorIndex = DamageColorIndex.Item,
                    force = 0f,
                    owner = base.gameObject,
                    position = body.transform.position,
                    rotation = Quaternion.identity //Util.QuaternionSafeLookRotation(aimRay)
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            }

            private Ray GetAimRay()
            {
                if (inputBank)
                {
                    return new Ray(inputBank.aimOrigin, inputBank.aimDirection);
                }
                return new Ray(base.transform.position, base.transform.forward);
            }

            //private Ray GetAimRay

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

            }
        }
    }*/
}
