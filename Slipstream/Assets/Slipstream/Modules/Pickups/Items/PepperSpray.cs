using Moonstorm;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using On.RoR2;
using On.RoR2.UI;
using TMPro;
using UnityEngine.UI;

namespace Slipstream.Items
{
    public class PepperSpray: ItemBase
    {
        //Probably look at GlassEye.cs for your first reference of an item

        private const string token = "SLIP_ITEM_PEPPERSPRAY_DESC";
        public override RoR2.ItemDef ItemDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<RoR2.ItemDef>("PepperSpray");

        //public static string section;

        //Establishes the config fields to allow easy changes in values in certain calculations and such.

        [ConfigurableField(ConfigName = "Base Shield", ConfigDesc = "Shield percentage after having at least one stack.", ConfigSection = "PepperSpray")]
        [TokenModifier(token, StatTypes.Percentage, 0)]
        public static float baseShield = 0.06f;

        [ConfigurableField(ConfigName = "Shield Threshold", ConfigDesc = "Percentage of total shield in order to trigger the effect.", ConfigSection = "PepperSpray")]
        [TokenModifier(token, StatTypes.Percentage, 1)]
        public static float threshold = 0.5f;

        [ConfigurableField(ConfigName = "Base Radius", ConfigDesc = "Initial radius of the stun effect.", ConfigSection = "PepperSpray")]
        [TokenModifier(token, StatTypes.Default, 2)]
        public static float baseRadius = 13.0f;

        [ConfigurableField(ConfigName = "Radius Increase", ConfigDesc = "Amount of increased stun radius per stack.", ConfigSection = "PepperSpray")]
        [TokenModifier(token, StatTypes.Default, 3)]
        public static float radiusPerStack = 6.0f;

        [ConfigurableField(ConfigName = "Speed Increase", ConfigDesc = "Movement speed increase when Pepper Speed is active.", ConfigSection = "PepperSpray")]
        [TokenModifier(token, StatTypes.Percentage, 4)]
        public static float speedIncrease = 0.6f;

        [ConfigurableField(ConfigName = "Max Speed Duration", ConfigDesc = "The time on your buff if your entire healthbar is shield + Base Speed Duration Constant.", ConfigSection = "PepperSpray")]
        [TokenModifier(token, StatTypes.Default, 5)]
        public static float maxBuffTime = 20.0f;

        [ConfigurableField(ConfigName = "Base Speed Duration Constant", ConfigDesc = "Initial amount of speed with one stack.", ConfigSection = "PepperSpray")]
        //[TokenModifier(token, StatTypes.Default, 5)]
        public static float buffTimeConstant = 1.0f;

        private static Sprite lowShieldNormal;
        private static Sprite lowShieldVoid;

        //public static string explosionSoundString = "Fart";
        public override void Initialize()
        {
            base.Initialize();
            lowShieldNormal = SlipAssets.Instance.MainAssetBundle.LoadAsset<Sprite>("texCriticalShieldIndi");
            lowShieldVoid = SlipAssets.Instance.MainAssetBundle.LoadAsset<Sprite>("texCriticalVoidShiIndi");
            On.RoR2.UI.HealthBar.UpdateHealthbar += new On.RoR2.UI.HealthBar.hook_UpdateHealthbar(this.HealthBar_UpdateHealthbar);
        }

        //may move this somewhere else when I make the void item

        //Hooks onto the global healthbar update event
        private void HealthBar_UpdateHealthbar(On.RoR2.UI.HealthBar.orig_UpdateHealthbar orig, RoR2.UI.HealthBar self, float deltaTime)
        {
            //invoke orig before the UpdateBar call so that the bar is set above other HealthBar elements
            orig.Invoke(self, deltaTime);
            PepperSpray.PepperSprayBehavior component = null;
            if (self.source)
            {
                component = self.source.GetComponent<PepperSpray.PepperSprayBehavior>();
                if (component)
                    component.UpdateBar(self);
            }
            if (component)
                component.ApplyBar(self);
        }

        public class PepperSprayBehavior : BaseItemBodyBehavior, IBodyStatArgModifier
        {
            [ItemDefAssociation(useOnClient = true, useOnServer = true)]
            public static RoR2.ItemDef GetItemDef() => SlipContent.Items.PepperSpray;

            private Image image;

            private bool shouldTrigger = false;

            //This just adds an initial shield when you have atleast one stack.
            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                args.baseShieldAdd += body.healthComponent.fullHealth * baseShield;
            }

            //The trigger should only happen once until you recharge, not after everytime you get hit below the threshold
            private void FixedUpdate()
            {
                //Checks if the body is at full shield. shouldTrigger is just a switch to make sure that the effect doesn't trigger more than once below the shield threshold.
                if (body.healthComponent.shield == body.healthComponent.fullShield && !shouldTrigger)
                        shouldTrigger = true;

                    //Checks if the body is lower than the shield threshold percentage.
                if (body.healthComponent.shield < body.healthComponent.fullShield * threshold && shouldTrigger)
                {
                    if (NetworkServer.active)
                    {
                        shouldTrigger = false;
                        FireStunSpray();
                        //not sure what addtimedbuffauthority is
                        body.AddTimedBuff(SlipContent.Buffs.PepperSpeed.buffIndex, maxBuffTime * (body.healthComponent.fullShield / (body.healthComponent.fullShield + body.healthComponent.fullHealth)) + buffTimeConstant);
                        //Util.PlaySound(explosionSoundString, gameObject);
                        RoR2.Util.PlaySound(EntityStates.Bison.PrepCharge.enterSoundString, gameObject);
                    }
                }
            }

            private void FireStunSpray()
            {
                //Establishes a gameobject for the explosion
                Vector3 corePosition = RoR2.Util.GetCorePosition(body.gameObject);
                float radius = body.radius + baseRadius + radiusPerStack * (stack - 1f);
                GameObject hitBoxStun = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GenericDelayBlast"), corePosition, Quaternion.identity);
                hitBoxStun.transform.localScale = new Vector3(radius, radius, radius);
                RoR2.DelayBlast sprayAttack = hitBoxStun.GetComponent<RoR2.DelayBlast>();

                //Fills characteristics of the explosion such as the damage type stunning
                sprayAttack.position = corePosition;
                sprayAttack.radius = radius;
                sprayAttack.attacker = body.gameObject;
                sprayAttack.falloffModel = RoR2.BlastAttack.FalloffModel.None;
                sprayAttack.maxTimer = 0f;
                sprayAttack.damageType = DamageType.Stun1s;
                sprayAttack.explosionEffect = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("PepperSprayExplosion");
                //sprayAttack.delayEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/AffixWhiteExplosion");
                hitBoxStun.GetComponent<RoR2.TeamFilter>().teamIndex = RoR2.TeamComponent.GetObjectTeam(body.gameObject);

                //Fires the explosion
                NetworkServer.Spawn(hitBoxStun);
            }

            private void OnDestroy()
            {
                //PepperSpray.PepperSprayBehavior component = body.GetComponent<PepperSpray.PepperSprayBehavior>();
                if (image)
                {
                    if (Application.isPlaying)
                        Destroy(image.gameObject);
                    else
                        DestroyImmediate(image.gameObject);
                    image = null;
                }
            }

            //thanks groove
            public void UpdateBar(RoR2.UI.HealthBar healthBar)
            {
                if (!healthBar.barAllocator.containerTransform.gameObject.scene.IsValid())
                {
                    return;
                }
                PepperSpray.PepperSprayBehavior component = healthBar.source.GetComponent<PepperSpray.PepperSprayBehavior>();
                if (!shouldTrigger)
                {
                    //destroy image gameObject if the item triggers.
                    if (image)
                    {
                        if (Application.isPlaying)
                            Destroy(image.gameObject);
                        else
                            DestroyImmediate(image.gameObject);
                        image = null;
                    }
                    return;
                }
                if (!image)
                {
                    //reinstantiate image if the image doesn't exist (which it should)
                    image = Instantiate(healthBar.barAllocator.elementPrefab, healthBar.barAllocator.containerTransform).GetComponent<Image>();
                    GameObject gameObject = image.gameObject;
                    if (healthBar.barAllocator.markElementsUnsavable)
                        gameObject.hideFlags |= (HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild);
                    gameObject.SetActive(true);
                    int length = healthBar.barAllocator.elements.Count;
                    healthBar.barAllocator.AllocateElements(length - 1);
                }
            }

            public void ApplyBar(RoR2.UI.HealthBar healthBar)
            {
                if(image && healthBar.source)
                {
                    //image.type = healthBar.style.
                    RoR2.HealthComponent.HealthBarValues healthBarValues = healthBar.source.GetHealthBarValues();
                    image.sprite = healthBarValues.hasVoidShields ? lowShieldVoid : lowShieldNormal;
                    image.color = new Color(1f, 1f, 1f, 1f);

                    //starts at health end
                    float minPos = healthBarValues.healthFraction;

                    //starts at shield bar threshold
                    //can't use healthBarValues.shieldFraction because that only takes account to your current shield, not full shield
                    float fullShieldFraction = ((body.healthComponent.fullShield * threshold) / (body.healthComponent.fullShield + body.healthComponent.fullHealth)) * (1f - healthBarValues.curseFraction);
                    float maxPos = healthBarValues.healthFraction + (fullShieldFraction);

                    SetRectPosition((RectTransform)image.transform, minPos, maxPos, 1f);
                }
            }

            public static void SetRectPosition(RectTransform rectTransform, float xMin, float xMax, float sizeDelta)
            {
                rectTransform.anchorMin = new Vector2(xMin, 0f);
                rectTransform.anchorMax = new Vector2(xMax, 1f);
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = new Vector2(sizeDelta * 0.5f + 1f, sizeDelta + 1f);
            }
        }
    }
}
