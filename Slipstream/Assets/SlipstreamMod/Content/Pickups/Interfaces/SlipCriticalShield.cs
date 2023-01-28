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
using Slipstream.Utils;

namespace Slipstream.Items
{
    public class SlipCriticalShield
    {

        [ConfigurableField(ConfigName = "Shield Threshold", ConfigDesc = "Percentage of total shield in order to trigger effects like Pepper Spray and Brine Swarm", ConfigSection = "CriticalShield")]
        public static float threshold = 0.5f;


        private static Sprite lowShieldNormal;
        private static Sprite lowShieldVoid;

        public static Dictionary<CharacterBody, bool> shouldTrigger = new Dictionary<CharacterBody, bool>();



        RoR2.UI.HealthBarStyle.BarStyle critShieldBarStyle;

        public void Init()
        {
            lowShieldNormal = SlipAssets.Instance.MainAssetBundle.LoadAsset<Sprite>("texCriticalShieldIndi");
            lowShieldVoid = SlipAssets.Instance.MainAssetBundle.LoadAsset<Sprite>("texCriticalVoidShiIndi");

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

            public override RoR2.UI.HealthBarStyle.BarStyle GetStyle()
            {
                var style = bar.style.barrierBarStyle;
                style.sizeDelta = bar.style.lowHealthOverStyle.sizeDelta;
                return style;
            }

            public override void CheckInventory(ref RoR2.UI.HealthBar.BarInfo info, RoR2.CharacterBody body)
            {
                base.CheckInventory(ref info, body);

                //if there are any critical shield items in the inventory, enable healthbar

                enabled = body.gameObject.GetComponent<ICriticalShield>() != null;
            }

            public override void UpdateInfo(ref RoR2.UI.HealthBar.BarInfo info, HealthComponent healthSource)
            {
                info.enabled = enabled && shouldTrigger[healthSource.body];
                var healthBarValues = healthSource.GetHealthBarValues();
                info.sprite = healthBarValues.hasVoidShields? lowShieldVoid : lowShieldNormal;

                float minPos = healthBarValues.healthFraction;
                info.normalizedXMin = minPos;
                float fullShieldFraction = ((healthSource.fullShield * threshold) / (healthSource.fullShield + healthSource.fullHealth)) * (1f - healthBarValues.curseFraction);
                info.normalizedXMax = minPos + (fullShieldFraction);

                base.UpdateInfo(ref info, healthSource);
            }


        }
    }
}