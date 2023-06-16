using Moonstorm;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using RoR2.UI;

namespace Slipstream.Items
{
    public class SlipCriticalShield
    {

        [ConfigurableField(ConfigName = "Shield Threshold", ConfigDesc = "Percentage of total shield in order to trigger effects like Pepper Spray and Brine Swarm", ConfigSection = "CriticalShield")]
        public static float threshold = 0.5f;


        private static Texture lowShieldNormal = SlipAssets.LoadAsset<Texture>("texCriticalShieldIndi", SlipBundle.Base);
        private static Texture lowShieldVoid = SlipAssets.LoadAsset<Texture>("texCriticalVoidShiIndi", SlipBundle.Base);

        public static Dictionary<CharacterBody, bool> shouldTrigger = new Dictionary<CharacterBody, bool>();
        public static Color critShieldBaseColor = new Color(1f, 1f, 1f, 1f);
        

        private static float timeAmount = 0.4f;
        

        //need to add to this list for the healthbar to work, for some reason checking for the interface instances happens after the check inventory hook
        public static List<ItemDef> critShieldItems = new List<ItemDef>();

        public static RoR2.UI.HealthBarStyle.BarStyle critShieldBarStyle;

        public void Init()
        {
            CharacterBody.onBodyAwakeGlobal += CharacterBody_onBodyAwakeGlobal;
            CharacterBody.onBodyDestroyGlobal += CharacterBody_onBodyDestroyGlobal;
            On.RoR2.HealthComponent.FixedUpdate += HealthComponent_FixedUpdate;

            ExtraHealthbarSegment.AddType<CriticalShieldData>();
        }

        //this is disgusting but im not sure how else to do it
        private void HealthComponent_FixedUpdate(On.RoR2.HealthComponent.orig_FixedUpdate orig, HealthComponent self)
        {
            orig(self);

            if(self.fullShield > 0)
            {
                if (self.shield == self.fullShield && !shouldTrigger[self.body])
                    shouldTrigger[self.body] = true;

                //Checks if the body is lower than the shield threshold percentage.
                else if (self.shield < self.fullShield * threshold && shouldTrigger[self.body])
                {
                    shouldTrigger[self.body] = false;
                    List<ICriticalShield> components = GetComponentsCache<ICriticalShield>.GetGameObjectComponents(self.body.gameObject);
                    foreach (ICriticalShield criticalShield in components)
                        criticalShield.Trigger();
                    GetComponentsCache<ICriticalShield>.ReturnBuffer(components);
                }
            }
            else
                shouldTrigger[self.body] = true;

        }

        private void CharacterBody_onBodyDestroyGlobal(CharacterBody obj)
        {
            shouldTrigger.Remove(obj);
        }

        private void CharacterBody_onBodyAwakeGlobal(CharacterBody obj)
        {
            shouldTrigger.Add(obj, true);
        }



        public interface ICriticalShield
        {
			void Trigger();
        }

        public class CriticalShieldData : ExtraHealthbarSegment.BarData
        {
            private bool enabled;
            private bool cachedEnabled = false;
            private Material barMat = SlipAssets.LoadAsset<Material>("matCriticalShield", SlipBundle.Base);
            private float currentVel;
            private Vector2 scale = new Vector2(2f, 0.2f);

            private Run.TimeStamp timer;

            public static Dictionary<HealthComponent, Material> materialShield= new Dictionary<HealthComponent, Material>();


            public override RoR2.UI.HealthBarStyle.BarStyle GetStyle()
            {
                var style = critShieldBarStyle;
                style.sizeDelta = bar.style.lowHealthOverStyle.sizeDelta;
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

                bool hasItem = false;
                foreach(ItemDef critShieldItemDef in critShieldItems)
                {
                    if (body.GetItemCount(critShieldItemDef) > 0)
                    {
                        hasItem = true;
                        break;
                    }
                }
                if (!body || !body.healthComponent || !hasItem /*|| body.gameObject.GetComponent<ICriticalShield>() == null*/)
                {
                    if (materialShield.ContainsKey(body.healthComponent))
                        materialShield.Remove(body.healthComponent);
                    return;
                }                
                enabled = true;


            }

            public override void UpdateInfo(ref RoR2.UI.HealthBar.BarInfo info, HealthComponent healthSource)
            {
                //instanceMat = image.canvasRenderer.GetMaterial();

                base.UpdateInfo(ref info, healthSource);

                info.enabled = enabled && shouldTrigger[healthSource.body]; //&& healthSource.shield > 0;

                var healthBarValues = healthSource.GetHealthBarValues();

                if(materialShield.ContainsKey(healthSource))
                    materialShield[healthSource].SetTexture("_RemapTex", healthBarValues.hasVoidShields ? lowShieldVoid : lowShieldNormal);

                

                float minPos = healthBarValues.healthFraction;
                info.normalizedXMin = minPos;

                if (cachedEnabled != info.enabled && info.enabled && healthSource.shield > 0) //if the bar just got enabled, start timer
                {
                    info.normalizedXMax = minPos;
                    timer = Run.TimeStamp.now;
                }
                cachedEnabled = info.enabled;

                float fullShieldFraction = ((healthSource.fullShield * threshold) / (healthSource.fullShield + healthSource.fullHealth)) * (1f - healthBarValues.curseFraction);

                /*Vector2 newScale = new Vector2(scale.x * fullShieldFraction * 2, scale.y);
                materialShield[healthSource].SetTextureScale("_Cloud1Tex", newScale);*/

                if (timer != null && timer.timeSince <= timeAmount && enabled)
                {
                    info.normalizedXMax = Mathf.SmoothDamp(info.normalizedXMax, minPos + (fullShieldFraction), ref currentVel, timeAmount, Mathf.Infinity, timer.timeSince) /*(minPos + (fullShieldFraction)) * timer.timeSince*/;
                    if (materialShield.ContainsKey(healthSource))
                        materialShield[healthSource].SetFloat("_Boost", 1f + currentVel);
                }  
                else if (timer.timeSince > timeAmount && enabled)
                {
                    info.normalizedXMax = minPos + (fullShieldFraction);
                    if (materialShield.ContainsKey(healthSource))
                        materialShield[healthSource].SetFloat("_Boost", 1f);
                    //timer = Run.TimeStamp.zero;
                }

                Vector2 newScale = new Vector2(scale.x * (info.normalizedXMax - info.normalizedXMin), scale.y);
                if (materialShield.ContainsKey(healthSource))
                    materialShield[healthSource].SetTextureScale("_Cloud1Tex", newScale);

            }

            public override void ApplyBar(ref HealthBar.BarInfo info, Image image, HealthComponent source, ref int i)
            {
                if(!materialShield.ContainsKey(source))
                    materialShield.Add(source, UnityEngine.Object.Instantiate(barMat));

                if (source.GetHealthBarValues().hasVoidShields) materialShield[source].SetTexture("_RemapTex", lowShieldVoid);
                image.material = materialShield[source];
                base.ApplyBar(ref info, image, source, ref i);
            }


        }
    }
}